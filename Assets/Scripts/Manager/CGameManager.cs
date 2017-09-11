using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SimpleSingleton;
using FSM;

namespace WarOfSlinger {
	public class CGameManager : CMonoSingleton<CGameManager>, IContext {

		#region Fields

		[Header("Game Data")]
		[SerializeField]	protected TextAsset m_VillageTextAsset;
		[SerializeField]	protected CVillageData m_VillageData;

		[Header ("UITalk")]
		[SerializeField]	protected CUITalk m_UITalk;

		[Header("Game object detected")]
		[SerializeField]	protected Camera m_Camera;
		[SerializeField]	protected CameraFollower m_CameraFollower;
		[SerializeField]	protected LayerMask m_TouchDetectLayerMask;
		[SerializeField]	protected CUIJob m_UIJob;

		[Header("Game mode")]
		[SerializeField]	protected EGameMode m_GameMode;
		[SerializeField]	protected TextAsset m_GameModeTextAsset;
		[SerializeField]	protected string m_FSMStateName;

		[Header("Building points")]
		[SerializeField]	protected GameObject[] m_BuildingPoints;

		protected bool m_IsUITouched = false;

		protected Dictionary<string, List<CObjectController>> m_VillageObjects;
		public Dictionary<string, List<CObjectController>> villageObjects {
			get { return this.m_VillageObjects; }
			protected set { this.m_VillageObjects = value; }
		}
		protected FSMManager m_FSMManager;

		public Action<GameObject> OnEventTouchedGameObject;
		public Action<Vector2> OnEventBeginTouch;
		public Action<Vector2> OnEventTouched;
		public Action<Vector2> OnEventEndTouch;

		public bool canChangeMode {
			get;
			set;
		}

		public enum EGameMode: byte {
			FREE = 0,
			LOADING = 1,
			PLAYING = 2,
			BUILDING = 3,
			PVP = 4,
			PVE = 5
		}

		#endregion

		#region Implementation Monobehaviour

		protected override void Awake ()
		{
			base.Awake ();
			this.m_VillageObjects = new Dictionary<string, List<CObjectController>> ();
			this.m_VillageData = TinyJSON.JSON.Load (m_VillageTextAsset.text).Make<CVillageData>();
			// LOAD FSM
			this.m_FSMManager = new FSMManager ();
			this.m_FSMManager.LoadFSM (this.m_GameModeTextAsset.text);
			// FSM STATE
			this.m_FSMManager.RegisterState ("GameFreeModeState", 		new FSMGameFreeModeState(this));
			this.m_FSMManager.RegisterState ("GameLoadingModeState", 	new FSMGameLoadingModeState(this));
			this.m_FSMManager.RegisterState ("GamePlayingModeState",	new FSMGamePlayingModeState(this));
			this.m_FSMManager.RegisterState ("GameBuildingModeState",	new FSMGameBuildingModeState(this));
			this.m_FSMManager.RegisterState ("GamePVPModeState", 		new FSMGamePVPModeState(this));
			this.m_FSMManager.RegisterState ("GamePVEModeState", 		new FSMGamePVEModeState(this));
			// FSM CONDITION
			this.m_FSMManager.RegisterCondition ("IsFreeMode", 			this.IsFreeMode);
			this.m_FSMManager.RegisterCondition ("IsLoadingMode",		this.IsLoadingMode);
			this.m_FSMManager.RegisterCondition ("IsPlayingMode",		this.IsPlayingMode);
			this.m_FSMManager.RegisterCondition ("IsBuildingMode",		this.IsBuildingMode);
			this.m_FSMManager.RegisterCondition ("IsPVPMode", 			this.IsPVPMode);
			this.m_FSMManager.RegisterCondition ("IsPVEMode", 			this.IsPVEMode);
		}

		protected virtual void Start() {
			this.canChangeMode = true;
			this.m_GameMode = EGameMode.LOADING;
			// LOAD BUILDING
			this.LoadVillageObjects<CBuildingController> (this.m_VillageData.villageBuildings, () => {
				CUIGameManager.Instance.SetUpResource(this.m_VillageData);
				// LOAD CHARACTER
				this.LoadVillageObjects<CCharacterController> (this.m_VillageData.villageCharacters, () => {
					// LOAD NPC
					this.LoadVillageObjects<CNPCController> (this.m_VillageData.villageNPCs, () => {
						// LOAD OBJECT
						this.LoadVillageObjects<CEnvironmentObjectController> (this.m_VillageData.villageObjects, () => { 
							this.m_GameMode = EGameMode.PLAYING;
						}, null);
					}, null);
				}, (index, objCtrl) => {
					this.m_VillageData.currentPopulation += 1;
				});
			}, (index, objCtrl) => {
				var buildingData = this.m_VillageData.villageBuildings[index];
				this.m_VillageData.maxPopulation += buildingData.maxResident;
			});
		}

		protected virtual void Update() {
			this.m_FSMManager.UpdateState (Time.deltaTime);
			this.m_FSMStateName = this.m_FSMManager.currentStateName;
		}

		#endregion

		#region Detect object method

		public void OnMouseDetectStandalone() {
			var worldPos = this.m_Camera.ScreenToWorldPoint(Input.mousePosition);
			if (Input.GetMouseButtonDown (0)) {
				if (!EventSystem.current.IsPointerOverGameObject()) {
					this.OnTouchedObject ();
				}
				if (this.OnEventBeginTouch != null) {
					this.OnEventBeginTouch (worldPos);
				}
			}
			if (Input.GetMouseButton (0)) {
				if (this.OnEventTouched != null) {
					this.OnEventTouched (worldPos);
				}
			}
			if (Input.GetMouseButtonUp (0)) {
				if (this.OnEventEndTouch != null) {
					this.OnEventEndTouch (worldPos);
				}
			}
		}

		public void OnMouseDetectMobile() {
			if (Input.touchCount != 1)
				return;
			var fingerTouch = Input.GetTouch (0);
			var worldPos = this.m_Camera.ScreenToWorldPoint(fingerTouch.position);
			switch (fingerTouch.phase) {
			case TouchPhase.Began:
				this.m_IsUITouched = EventSystem.current.IsPointerOverGameObject (fingerTouch.fingerId);
				if (this.OnEventBeginTouch != null) {
					this.OnEventBeginTouch (worldPos);
				}
				break;
			case TouchPhase.Moved:
				if (this.OnEventTouched != null) {
					this.OnEventTouched (worldPos);
				}
				break;
			case TouchPhase.Canceled:
			case TouchPhase.Ended:
				if (this.OnEventEndTouch != null) {
					this.OnEventEndTouch (worldPos);
				}
				break;
			}
			if (this.m_IsUITouched == false) {
				this.OnTouchedObject ();
			}
		}

		protected virtual void OnTouchedObject() {
			GameObject detectedGo = null;
			if (this.OnDectedGameObject (ref detectedGo)) {
				if (this.OnEventTouchedGameObject != null) {
					this.OnEventTouchedGameObject (detectedGo);
				}
			}
		}

		public virtual bool OnDectedGameObject(ref GameObject detectGo) {
			var mouseWorldPoint = this.m_Camera.ScreenToWorldPoint (Input.mousePosition);
			var rayHit2Ds = Physics2D.RaycastAll (mouseWorldPoint, Vector2.zero, Mathf.Infinity);
			if (rayHit2Ds.Length > 0) {
				for (int i = 0; i < rayHit2Ds.Length; i++) {
					var objectLayer = rayHit2Ds [i].collider.gameObject.layer;
					var isDetected = this.m_TouchDetectLayerMask == (this.m_TouchDetectLayerMask | (1 << objectLayer));
					if (isDetected == true) {
						detectGo = rayHit2Ds [i].collider.gameObject;
						return true;
					}
				}
			}
			return false;
		}

		public virtual void OnOpenUIJobPanel(GameObject detectedGo) {
			var parentRoot = detectedGo.transform.root;
			var objController = parentRoot.GetComponent<CObjectController> ();
			if (objController != null && objController.IsObjectWorking) {
				var objData = objController.GetData ();
				if (objData != null) {
					var uiPos = objController.uiJobPoint;
					this.m_UIJob.ShowJobs (uiPos, (currentJob) => {
						// EXCUTE
						objController.ExcuteJobOwner (currentJob.jobExcute);
					}, (currentJob) => {
						// CLEAR
						objController.ClearJobOwner (currentJob.jobExcute);
					}, objData.objectJobs);
				}
			}
		}

		#endregion

		#region Talk

		public virtual void ShowTalk(Transform parent, string value) {
			if (this.m_UITalk == null)
				return;
			this.m_UITalk.ShowTalkItem (parent, value);
		}

		#endregion

		#region Load village building

		public virtual void LoadVillageObjects<T> (CObjectData[] villageObjects, Action completed, Action<int, CObjectController> processing) where T : CObjectController {
			StartCoroutine (this.HandleLoadVillageObjects<T> (villageObjects, completed, processing));
		}

		public virtual void LoadVillageObject<T> (CObjectData villageObject, Vector3 position, Action<CObjectController> completed) where T : CObjectController {
			StartCoroutine (this.HandleLoadVillageObject<T> (villageObject, position, completed));
		}

		protected virtual IEnumerator HandleLoadVillageObjects<T>(CObjectData[] villageObjects, Action completed, Action<int, CObjectController> processing) where T : CObjectController {
			for (int i = 0; i < villageObjects.Length; i++) {
				// INSTANTIATE OBJECT
				var objectData = villageObjects [i];
				var objectCtrl = Instantiate (Resources.Load<T>(objectData.objectModel));
				yield return objectCtrl != null;
				objectCtrl.SetData (objectData);
				objectCtrl.SetPosition (objectData.objectV3Position);
				objectCtrl.Init ();
				// SETUP VILLAGE OBJECT
				var villageObjectType = objectData.objectType;
				if (this.m_VillageObjects.ContainsKey (villageObjectType) == false) {
					this.m_VillageObjects [villageObjectType] = new List<CObjectController> ();
				}
				if (this.m_VillageObjects [villageObjectType].Contains(objectCtrl) == false) {
					this.m_VillageObjects [villageObjectType].Add (objectCtrl);
				}
				// EVENTS
				if (processing != null) {
					processing (i, objectCtrl);
				}
				if (i >= villageObjects.Length - 1) {
					if (completed != null) {
						completed ();
					}
				}
			}
		}

		protected virtual IEnumerator HandleLoadVillageObject<T>(CObjectData objectData, Vector3 position, Action<CObjectController> completed) where T : CObjectController {
			var objectCtrl = Instantiate (Resources.Load<T>(objectData.objectModel));
			yield return objectCtrl != null;
			objectCtrl.SetData (objectData);
			objectCtrl.SetPosition (position);
			objectCtrl.Init ();
			// SETUP VILLAGE OBJECT
			var villageObjectType = objectData.objectType;
			if (this.m_VillageObjects.ContainsKey (villageObjectType) == false) {
				this.m_VillageObjects [villageObjectType] = new List<CObjectController> ();
			}
			if (this.m_VillageObjects [villageObjectType].Contains(objectCtrl) == false) {
				this.m_VillageObjects [villageObjectType].Add (objectCtrl);
			}
			if (completed != null) {
				completed (objectCtrl);
			}
		}

		#endregion

		#region Main methods

		public virtual void OpenShop(string shopName) {
			CUIGameManager.Instance.OpenShop (shopName, (itemData) => {
				CUIGameManager.Instance.OpenBuildingControl();
				CUIGameManager.Instance.CloseShop (shopName);
				this.SetMode (EGameMode.BUILDING);
			});
		}

		public virtual void FollowObject(Transform obj, float timer = -1f) {
			this.m_CameraFollower.FollowUntil (obj, timer);
		}

		#endregion

		#region Data

		public virtual void UpdateVillageData() {
			CUIGameManager.Instance.SetUpResource(this.m_VillageData);
		}

		public virtual CVillageData GetData() {
			return this.m_VillageData;
		}

		#endregion

		#region Mode handle

		public void SetMode(EGameMode value) {
			if (this.canChangeMode == false)
				return;
			this.m_GameMode = value;
		}

		#endregion

		#region FSM 

		public virtual bool IsFreeMode() {
			return this.m_GameMode == EGameMode.FREE;
		}

		public virtual bool IsLoadingMode() {
			return this.m_GameMode == EGameMode.LOADING;
		}

		public virtual bool IsPlayingMode() {
			return this.m_GameMode == EGameMode.PLAYING;
		}

		public virtual bool IsBuildingMode() {
			return this.m_GameMode == EGameMode.BUILDING;
		}

		public virtual bool IsPVEMode() {
			return this.m_GameMode == EGameMode.PVE;
		}

		public virtual bool IsPVPMode() {
			return this.m_GameMode == EGameMode.PVP;
		}

		#endregion
		
	}
}
