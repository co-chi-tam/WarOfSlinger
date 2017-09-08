using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class FSMGameBuildingModeState : FSMBaseState {

		protected CGameManager m_GameManager;

		public FSMGameBuildingModeState (IContext context): base(context)
		{
			this.m_GameManager = context as CGameManager;
		}

		public override void StartState ()
		{
			base.StartState ();
			var villageObjects = this.m_GameManager.villageObjects;
			foreach (var item in villageObjects) {
				if (item.Key != "building") {
					var listObjs = item.Value;
					for (int i = 0; i < listObjs.Count; i++) {
						listObjs [i].gameObject.SetActive (false);
					}
				}
			}
		}

		public override void UpdateState (float dt)
		{
			base.UpdateState (dt);
		}

		public override void ExitState ()
		{
			base.ExitState ();
		}

	}
}
