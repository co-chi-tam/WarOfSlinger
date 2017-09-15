using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class CGuardTowerBuildingController : CBuildingController, IGuarderJobOwner {

		#region Fields

		[Header("Detect enemy")]
		[SerializeField]	protected LayerMask m_EnemyLayerMask;

		protected RaycastHit2D[] m_EnemyList;
		protected List<CObjectController> m_EnemyCtrlList;

		#endregion

		#region Implementation Moonobehaviour

		public override void Init() {
			base.Init ();
			this.m_EnemyCtrlList = new List<CObjectController> ();
		}

		#endregion

		#region Implementation IGuarderJobOwner

		public virtual bool HaveEnemy ()
		{
			var originPosition = this.m_Transform.position;
			this.m_EnemyList = Physics2D.CircleCastAll (originPosition, this.m_DetectRadius, Vector2.zero, 0f, this.m_EnemyLayerMask);
			return this.m_EnemyList.Length > 0;
		}

		public virtual List<CObjectController> GetEnemies() {
			this.m_EnemyCtrlList.Clear ();
			for (int i = 0; i < this.m_EnemyList.Length; i++) {
				var parentRoot = this.m_EnemyList [i].transform.root;
				var objController = parentRoot.GetComponent<CObjectController> ();
				if (objController != null && objController.IsObjectWorking && objController.GetActive ()) {
					this.m_EnemyCtrlList.Add (objController);
				}
			}
			return this.m_EnemyCtrlList;
		}

		public virtual void SendRandomEnemyCommand(string command = "AttackCommand") {
			var listEnemies = this.GetEnemies ();
			if (listEnemies.Count == 0)
				return;
			var randomEnemy = (int)Time.time % listEnemies.Count;
			var guarderCtrl = listEnemies[randomEnemy];
			if (guarderCtrl != null && guarderCtrl.IsObjectWorking && guarderCtrl.GetActive ()) { 
				guarderCtrl.ExcuteJobOwner (command);
			}
		}

		#endregion
	}
}
