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
	public partial class CGameManager : CMonoSingleton<CGameManager>, IContext {

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

		[Header("Map points")]
		[SerializeField]	protected GameObject[] m_MapPoints;

		protected Dictionary<string, List<CObjectController>> m_VillageObjects;
		public Dictionary<string, List<CObjectController>> villageObjects {
			get { return this.m_VillageObjects; }
			protected set { this.m_VillageObjects = value; }
		}

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

		public float villageTimer {
			get { 
				if (this.m_VillageData == null)
					return Time.time;
				return this.m_VillageData.villageTimer;
			}
		}

		#endregion

		#region Implementation Monobehaviour

		protected override void Awake ()
		{
			base.Awake ();
#if UNITY_EDITOR
			PlayerPrefs.DeleteAll ();
#endif
			this.m_VillageObjects = new Dictionary<string, List<CObjectController>> ();
			// FSM
			this.RegisterFSM ();
		}

		protected virtual void Start() {
			this.canChangeMode = true;
			this.m_GameMode = EGameMode.LOADING;
			// LOAD DATA
			this.LoadVillageData (() => {
				// LOAD VILLAGE OBJECTS
				this.LoadAllVillageObjects(() => {
					// SWITCH PLAYING MODE
					this.m_GameMode = EGameMode.PLAYING;
					// SET UP RESPAWN OBJECT
					this.SetupRespawnObject(this.m_RespawnSlot);
				});
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

		#region Load data 

		public virtual void LoadVillageData(Action completed) {
			StartCoroutine (this.HandleLoadVillageData (completed));
		}

		protected virtual IEnumerator HandleLoadVillageData(Action completed) {
			// LOAD DATA
			var villageDataText = PlayerPrefs.GetString (CTaskUtil.VILLAGE_DATA_SAVE_01, this.m_VillageTextAsset.text);
			this.m_VillageData = TinyJSON.JSON.Load (villageDataText).Make<CVillageData>();
			yield return this.m_VillageData != null;
			if (completed != null) {
				completed ();
			}
		}

		protected virtual void LoadAllVillageObjects(Action completed) {
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
						if (completed != null) {
							completed ();
						}
					}, null);
				}, (index, objCtrl) => {
					// SET IS FREE LABOR
					CJobManager.ReturnFreeLabor (objCtrl as CCharacterController);
					this.m_VillageData.currentPopulation += 1;
				});
			}, (index, objCtrl) => {
				var buildingData = this.m_VillageData.villageBuildings[index];
				this.m_VillageData.maxPopulation += buildingData.maxResident;
			});
		}

		#endregion

		#region UI

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

		public virtual void CalculateVillageTimer(float dt) {
			this.m_VillageData.villageTimer += dt;
			// RESPAWN OBJECT
			if (this.m_VillageData.villageTimer >= this.m_RespawnObjNextTimer) {
				this.RespawnObject (this.m_RespawnSlot);
			}
		}

		public virtual void OpenBuildingControl() {
			CUIGameManager.Instance.OpenBuildingControl();
			this.SetMode (EGameMode.BUILDING);
		}

		public virtual void OpenShop(string shopName) {
			if (this.m_VillageData.villageShops.ContainsKey (shopName) == false) {
				return;
			}
			var shopData = this.m_VillageData.villageShops [shopName];
			CUIGameManager.Instance.OpenShop (shopData, (itemData) => {
				if (CVillageManager.IsEnoughResources(itemData.itemCost) && itemData.currentAmount < itemData.maxAmount) {
					CUIGameManager.Instance.OpenBuildingControl();
					CUIGameManager.Instance.CloseShop (shopName);
					this.SetMode (EGameMode.BUILDING);
					switch (shopName) {
					case "building-shop":
						this.CreateItemFromData (itemData, (building) => {
							building.SetEnabledPhysic (false);
							itemData.currentAmount++;
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

		#region Getter && Setter

		public void SetMode(EGameMode value) {
			if (this.canChangeMode == false)
				return;
			this.m_GameMode = value;
		}

		public GameObject[] GetMapPoints() {
			return this.m_MapPoints;
		}

		#endregion

	}
}
