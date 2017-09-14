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

		#region Fields

		[SerializeField]	protected float m_RespawnObjNextTimer = 0f;

		#endregion

		#region Respawn object

		public virtual void SetupRespawnObject() {
			var villageTimer 		= (int)this.m_VillageData.villageTimer;
			var respawnObject 		= this.m_VillageData.villageRespawnObject;
			var randomTimer 		= villageTimer % respawnObject.respawnTimers.Length;
			var randomSpawnTimer 	= respawnObject.respawnTimers[randomTimer];
			this.m_RespawnObjNextTimer = villageTimer + randomSpawnTimer;
		}

		public virtual void RespawnObject() {
			var villageTimer 		= (int)this.m_VillageData.villageTimer;
			var respawnObjectData	= this.m_VillageData.villageRespawnObject;
			// SOURCE PATH
			var randomSource 		= villageTimer % respawnObjectData.respawnSources.Length;
			var respawnSourcePath	= respawnObjectData.respawnSources[randomSource];
			// POINT PATH
			var randomPoint			= villageTimer % respawnObjectData.respawnPoints.Length;
			var respawnPointPath	= respawnObjectData.respawnPoints [randomPoint];
			// SPAWN OBJECT
			var savedObjects		= this.FindObjectWith (respawnSourcePath);
			var positionObject		= this.FindObjectWith (respawnPointPath);
			var rePosition 			= Vector3.zero;
			if (positionObject != null) {
				rePosition = positionObject [villageTimer % positionObject.Length].GetPosition();
			} else {
				rePosition = this.m_MapPoints [villageTimer % this.m_MapPoints.Length].transform.position;
			}
			if (savedObjects != null && savedObjects.Length > 0) {
				var reObjectCtrl = savedObjects [villageTimer % savedObjects.Length];
				// RESET OBJECT
				reObjectCtrl.SetPosition (rePosition);
				reObjectCtrl.ResetOject ();
				reObjectCtrl.SetActive (true);
				reObjectCtrl.gameObject.SetActive (true);
			} else {
				LoadObject<CObjectController> (respawnSourcePath, (objController) => {
					// RESET OBJECT
					objController.SetPosition (rePosition);
					objController.ResetOject ();
					objController.SetActive (true);
					objController.gameObject.SetActive (true);
					this.RegisterVillageObject (respawnSourcePath, objController);
				});
			}
			// RESET TIMER
			this.SetupRespawnObject ();
		}

		#endregion
		
	}
}
