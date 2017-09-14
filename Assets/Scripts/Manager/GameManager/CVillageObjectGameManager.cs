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
	public partial class CGameManager {

		#region Load village object

		public virtual CObjectController[] FindObjectWith(string type) {
			if (this.m_VillageObjects.ContainsKey (type)) {
				var result = this.m_VillageObjects [type];
				return result.ToArray ();
			}
			return null;
		}

		public virtual void RegisterVillageObject(string villageObjectType, CObjectController objectCtrl) {
			// SETUP VILLAGE OBJECT
			if (this.m_VillageObjects.ContainsKey (villageObjectType) == false) {
				this.m_VillageObjects [villageObjectType] = new List<CObjectController> ();
			}
			if (this.m_VillageObjects [villageObjectType].Contains (objectCtrl) == false) {
				this.m_VillageObjects [villageObjectType].Add (objectCtrl);
			}
		}

		public virtual void LoadVillageObjects<T> (bool needRegistry, CObjectData[] villageObjects, Action completed, Action<int, CObjectController> processing) where T : CObjectController {
			StartCoroutine (this.HandleLoadVillageObjects<T> (needRegistry, villageObjects, completed, processing));
		}

		public virtual void LoadVillageObject<T> (bool needRegistry, CObjectData villageObject, Vector3 position, Action<CObjectController> completed) where T : CObjectController {
			StartCoroutine (this.HandleLoadVillageObject<T> (needRegistry, villageObject, position, completed));
		}

		public virtual void LoadObject<T> (string modelPath, Action<CObjectController> completed) where T : CObjectController {
			StartCoroutine (this.HandleLoadObject <T> (modelPath, completed));
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
					this.RegisterVillageObject (objectData.objectType, objectCtrl);
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
				this.RegisterVillageObject (objectData.objectType, objectCtrl);
			}
			if (completed != null) {
				completed (objectCtrl);
			}
		}

		protected virtual IEnumerator HandleLoadObject<T> (string modelPath, Action<CObjectController> completed) where T : CObjectController {
			var objectCtrl = Instantiate (Resources.Load<T>(modelPath));
			yield return objectCtrl != null;
			if (completed != null) {
				completed (objectCtrl);
			}
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

		#endregion
		
	}
}
