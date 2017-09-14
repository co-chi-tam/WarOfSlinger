using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using SimpleSingleton;
using FSM;
using SceneTask;

namespace WarOfSlinger {
	public class CGameManager : CMonoSingleton<CGameManager>, IContext {

		#region Fields

		[Header("Game Data")]
		[SerializeField]	protected TextAsset m_VillageTextAsset;
		[SerializeField]	protected CVillageData m_VillageData;

		[Header("Game object detected")]
		[SerializeField]	protected Camera m_Camera;
		[SerializeField]	protected CameraFollower m_CameraFollower;
		[SerializeField]	protected LayerMask m_TouchDetectLayerMask;

		[Header("Game mode")]
		[SerializeField]	protected EGameMode m_GameMode;
		[SerializeField]	protected TextAsset m_GameModeTextAsset;
		[SerializeField]	protected string m_FSMStateName;

		[Header("Building points")]
		[SerializeField]	protected GameObject[] m_BuildingPoints;

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
			get; set;
		}

		public enum EGameMode: byte {
			FREE = 0,
			LOADING = 1,
			PLAYING = 2,
			BUILDING = 3,
			PVP = 4,
			PVE = 5
		}

		protected bool m_IsUITouched = false;

		#endregion

		#region Implementation Monobehaviour

		protected override void Awake ()
		{
			base.Awake ();

			PlayerPrefs.DeleteAll ();

			this.m_VillageObjects = new Dictionary<string, List<CObjectController>> ();
			// LOAD DATA
			var villageDataText = PlayerPrefs.GetString (CTaskUtil.VILLAGE_DATA_SAVE_01, this.m_VillageTextAsset.text);
			this.m_VillageData = TinyJSON.JSON.Load (villageDataText).Make<CVillageData>();
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
			// RESET POPULAR
			this.m_VillageData.currentPopulation = 0;
			this.m_VillageData.maxPopulation = 0;
			// LOAD BUILDING
			this.LoadVillageObjects<CBuildingController> (true, this.m_VillageData.villageBuildings, () => {
				CUIGameManager.Instance.SetUpResource(this.m_VillageData);
				// LOAD CHARACTER
				this.LoadVillageObjects<CCharacterController> (true, this.m_VillageData.villageCharacters, () => {
					// LOAD OBJECT
					this.LoadVillageObjects<CEnvironmentObjectController> (true, this.m_VillageData.villageObjects, () => { 
						this.m_GameMode = EGameMode.PLAYING;
					}, null);
				}, (index, objCtrl) => {
					// SET IS FREE LABOR
					CJobManager.ReturnFreeLabor (objCtrl as CCharacterController);
					this.m_VillageData.currentPopulation += 1;
				});
			}, (index, objCtrl) => {
				Debug.Log (index);
				var buildingData = this.m_VillageData.villageBuildings[index];
				this.m_VillageData.maxPopulation += buildingData.maxResident;
			});
		}

		protected virtual void Update() {
			this.m_FSMManager.UpdateState (Time.deltaTime);
			this.m_FSMStateName = this.m_FSMManager.currentStateName;
		}

		protected virtual void OnDestroy() {
		
		}

		protected virtual void OnApplicationQuit() {
			this.SaveVillage ();
		}

#if !UNITY_EDITOR
		protected virtual void OnApplicationFocus(bool value) {
			if (value == false) {
				this.SaveVillage ();
			}
		}

		protected virtual void OnApplicationPause(bool value) {
			if (value) {
				this.SaveVillage ();
			}
		}
#endif

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
			if (Input.touchCount == 0)
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
			if (objController != null && objController.IsObjectWorking && objController.GetActive()) {
				var objData = objController.GetData ();
				if (objData != null) {
					var uiPos = objController.uiJobPoint;
					CUIGameManager.Instance.OpenJob (uiPos, (currentJob) => {
						// EXCUTE
						objController.ExcuteJobOwner (currentJob.jobExcute);
					}, (currentJob) => {
						// CLEAR
						objController.ClearJobOwner (currentJob.jobExcute);
					}, objData.objectJobs);
				}
			}
		}

		public virtual void OnOpenUIInfoPanel(GameObject detectedGo) {
			var parentRoot = detectedGo.transform.root;
			var objController = parentRoot.GetComponent<CObjectController> ();
			if (objController != null) {
				CUIGameManager.Instance.OpenInfo (objController);
			}
		}

		#endregion

		#region Talk

		public virtual void ShowTalk(Transform parent, string value) {
			CUIGameManager.Instance.OpenTalk (parent, value);
		}

		#endregion

		#region Load village building

		public virtual CObjectController[] FindObjectWith(string type) {
			if (this.m_VillageObjects.ContainsKey (type)) {
				var result = this.m_VillageObjects [type];
				return result.ToArray ();
			}
			return null;
		}

		public virtual void LoadVillageObjects<T> (bool needRegistry, CObjectData[] villageObjects, Action completed, Action<int, CObjectController> processing) where T : CObjectController {
			StartCoroutine (this.HandleLoadVillageObjects<T> (needRegistry, villageObjects, completed, processing));
		}

		public virtual void LoadVillageObject<T> (bool needRegistry, CObjectData villageObject, Vector3 position, Action<CObjectController> completed) where T : CObjectController {
			StartCoroutine (this.HandleLoadVillageObject<T> (needRegistry, villageObject, position, completed));
		}

		protected virtual IEnumerator HandleLoadVillageObjects<T>(bool needRegistry, CObjectData[] villageObjects, Action completed, Action<int, CObjectController> processing) where T : CObjectController {
			for (int i = 0; i < villageObjects.Length; i++) {
				// INSTANTIATE OBJECT
				var objectData = villageObjects [i];
				if (objectData == null || string.IsNullOrEmpty (objectData.objectModel)) 
					continue;
				var objectCtrl = Instantiate (Resources.Load<T>(objectData.objectModel));
				yield return objectCtrl != null;
				objectCtrl.SetData (objectData);
				objectCtrl.SetPosition (objectData.objectV3Position);
				objectCtrl.Init ();
				if (needRegistry) {
					// SETUP VILLAGE OBJECT
					var villageObjectType = objectData.objectType;
					if (this.m_VillageObjects.ContainsKey (villageObjectType) == false) {
						this.m_VillageObjects [villageObjectType] = new List<CObjectController> ();
					}
					if (this.m_VillageObjects [villageObjectType].Contains (objectCtrl) == false) {
						this.m_VillageObjects [villageObjectType].Add (objectCtrl);
					}
				}
				// EVENTS
				if (processing != null) {
					processing (i, objectCtrl);
				}
			}
			if (completed != null) {
				completed ();
			}
		}

		protected virtual IEnumerator HandleLoadVillageObject<T>(bool needRegistry, CObjectData objectData, Vector3 position, Action<CObjectController> completed) where T : CObjectController {
			var objectCtrl = Instantiate (Resources.Load<T>(objectData.objectModel));
			yield return objectCtrl != null;
			objectCtrl.SetData (objectData);
			objectCtrl.SetPosition (position);
			objectCtrl.Init ();
			if (needRegistry) {
				// SETUP VILLAGE OBJECT
				var villageObjectType = objectData.objectType;
				if (this.m_VillageObjects.ContainsKey (villageObjectType) == false) {
					this.m_VillageObjects [villageObjectType] = new List<CObjectController> ();
				}
				if (this.m_VillageObjects [villageObjectType].Contains (objectCtrl) == false) {
					this.m_VillageObjects [villageObjectType].Add (objectCtrl);
				}
			}
			if (completed != null) {
				completed (objectCtrl);
			}
		}

		#endregion

		#region Main methods

		public virtual void CalculatePopular() {
			this.m_VillageData.currentPopulation = 0;
			this.m_VillageData.maxPopulation = 0;
			// CHARACTER DATA 
			var listCharacters = this.m_VillageObjects ["character"];
			for (int i = 0; i < listCharacters.Count; i++) {
				if (listCharacters [i].GetActive () == false)
					continue;
				var data = listCharacters [i].GetData () as CCharacterData;
				this.m_VillageData.currentPopulation++;
			}
			// BUILDING DATA
			var listObjectBuildings = this.m_VillageObjects ["building"];
			for (int i = 0; i < listObjectBuildings.Count; i++) {
				if (listObjectBuildings [i].GetActive () == false || listObjectBuildings [i].IsObjectWorking == false)
					continue;
				var data = listObjectBuildings [i].GetData () as CBuildingData;
				this.m_VillageData.maxPopulation += data.maxResident;
			}
		}

		public virtual void OpenShop(string shopName) {
			CUIGameManager.Instance.OpenShop (shopName, (itemData) => {
				if (CVillageManager.IsEnoughResources(itemData.itemCost)) {
					CUIGameManager.Instance.OpenBuildingControl();
					CUIGameManager.Instance.CloseShop (shopName);
					this.SetMode (EGameMode.BUILDING);
					switch (shopName) {
					case "building-shop":
						this.CreateItemFromData (itemData, (building) => {
							building.SetEnabledPhysic (false);
							// TEST
							var buildingData = building.GetData() as CBuildingData;
							this.m_VillageData.maxPopulation += buildingData.maxResident;
						});
						if (CVillageManager.IsSubstractResources (itemData.itemCost)) {
							CUIGameManager.Instance.SetUpResource();
						}
						break;
					}
				} else {
					CUIGameManager.Instance.CloseShop (shopName);
				}
			});
		}

		public virtual void OpenInventory() {
			CUIGameManager.Instance.OpenInventory (this.m_VillageData.villageInventories, (itemData) => {
				CUIGameManager.Instance.OpenBuildingControl();
				CUIGameManager.Instance.CloseInventory();
				this.SetMode (EGameMode.BUILDING);
				this.CreateItemFromData (itemData, (building) => {
					building.SetEnabledPhysic (false);
					var itemIndex = Array.IndexOf (this.m_VillageData.villageInventories, itemData);
					this.m_VillageData.villageInventories[itemIndex] = null;
					// TEST
					var buildingData = building.GetData() as CBuildingData;
					this.m_VillageData.maxPopulation += buildingData.maxResident;
				});
			});
		}

		public virtual void CreateResident(Vector3 position) {
			if (this.m_VillageData.currentPopulation + 1 > this.m_VillageData.maxPopulation)
				return;
			var characters 		= Resources.LoadAll<CCharacterController> ("Character/Prefabs");
			var characterDatas 	= Resources.LoadAll<TextAsset>("Character/Data");
			// RANDOM
			var random 		= (int)Time.time % characters.Length;
			var newLabor 	= GameObject.Instantiate (characters[random]);
			var newData 	= TinyJSON.JSON.Load (characterDatas[0].text).Make<CCharacterData>();
			newData.objectName = "NEW NAME";
			newData.objectModel = "Character/Prefabs/" + characters[random].name;
			newLabor.SetTargetPosition (position);
			newLabor.SetPosition (position);
			newLabor.SetData (newData);
			newLabor.Init ();
			// SET IS FREE LABOR
			CJobManager.ReturnFreeLabor (newLabor);
			var currentPopulation = CVillageManager.GetCurrentPopulation ();
			CVillageManager.SetCurrentPopulation (currentPopulation + 1);
			// SETUP VILLAGE OBJECT
			var villageObjectType = newData.objectType;
			if (this.m_VillageObjects.ContainsKey (villageObjectType) == false) {
				this.m_VillageObjects [villageObjectType] = new List<CObjectController> ();
			}
			if (this.m_VillageObjects [villageObjectType].Contains (newLabor as CObjectController) == false) {
				this.m_VillageObjects [villageObjectType].Add (newLabor as CObjectController);
			}
		}

		public virtual void CreateItemFromData (CInventoryItemData itemData, Action<CObjectController> completed) {
			var itemSource = Resources.Load<TextAsset>(itemData.itemSource);
			var buildingData = TinyJSON.JSON.Load(itemSource.text).Make<CBuildingData>();
			var buildingPosition = CUtil.GetCenterScreen2WorldPoint();
			this.LoadVillageObject<CBuildingController> (true, buildingData, buildingPosition, completed);
		}

		public virtual CInventoryItemData CreateObjectToItem(CObjectData data) {
			var item = new CInventoryItemData ();
			item.itemName = data.objectName;
			item.itemAvatar = data.objectAvatar;
			item.itemSource = data.objectModel;
			return item;
		}

		public virtual void FollowObject(Transform obj, float timer = -1f) {
			this.m_CameraFollower.FollowUntil (obj, timer);
		}

		public virtual void SaveVillage(string saveSlot = "") {
			// BUILDING DATA
			if (this.m_VillageObjects.ContainsKey ("building")) {
				var listObjectBuildings = this.m_VillageObjects ["building"];
				this.m_VillageData.villageBuildings = new CBuildingData[listObjectBuildings.Count];
				for (int i = 0; i < listObjectBuildings.Count; i++) {
					if (listObjectBuildings [i].GetActive () == false)
						continue;
					var data = listObjectBuildings [i].GetData () as CBuildingData;
					data.objectPosition = listObjectBuildings [i].GetPosition ().ToV3String (); 
					this.m_VillageData.villageBuildings [i] = data;
				}
			} else {
				this.m_VillageData.villageBuildings = new CBuildingData[0];
			}
			// OBJECT DATA
			if (this.m_VillageObjects.ContainsKey ("environment")) {
				var listObjectEnvironments = this.m_VillageObjects ["environment"];
				this.m_VillageData.villageObjects = new CDamageableObjectData[listObjectEnvironments.Count];
				for (int i = 0; i < listObjectEnvironments.Count; i++) {
					if (listObjectEnvironments [i].GetActive () == false)
						continue;
					var data = listObjectEnvironments [i].GetData () as CDamageableObjectData;
					data.objectPosition = listObjectEnvironments [i].GetPosition ().ToV3String (); 
					this.m_VillageData.villageObjects[i] = data;
				}
			} else {
				this.m_VillageData.villageObjects = new CDamageableObjectData[0];
			}
			// CHARACTER DATA 
			if (this.m_VillageObjects.ContainsKey ("character")) {
				var listCharacters = this.m_VillageObjects ["character"];
				this.m_VillageData.villageCharacters = new CCharacterData[listCharacters.Count];
				for (int i = 0; i < listCharacters.Count; i++) {
					if (listCharacters [i].GetActive () == false)
						continue;
					var data = listCharacters [i].GetData () as CCharacterData;
					data.objectPosition = listCharacters [i].GetPosition ().ToV3String (); 
					this.m_VillageData.villageCharacters [i] = data;
				}
			} else {
				this.m_VillageData.villageCharacters = new CCharacterData[0];
			}
			var jsonSave = TinyJSON.JSON.Dump (this.m_VillageData);
			PlayerPrefs.SetString (CTaskUtil.VILLAGE_DATA_SAVE_01, jsonSave);
			PlayerPrefs.Save ();
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
