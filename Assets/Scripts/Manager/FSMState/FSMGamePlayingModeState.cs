using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class FSMGamePlayingModeState : FSMBaseState {

		protected CGameManager m_GameManager;

		public FSMGamePlayingModeState (IContext context): base(context)
		{
			this.m_GameManager = context as CGameManager;
		}

		public override void StartState ()
		{
			base.StartState ();
			var villageObjects = this.m_GameManager.villageObjects;
			foreach (var item in villageObjects) {
				var listObjs = item.Value;
				for (int i = 0; i < listObjs.Count; i++) {
					listObjs [i].gameObject.SetActive (true);
				}
			}
		}

		public override void UpdateState (float dt)
		{
			base.UpdateState (dt);
#if UNITY_EDITOR || UNITY_STANDALONE
			this.m_GameManager.OnMouseDetectStandalone();
#else
			this.m_GameManager.OnMouseDetectMobile();
#endif
		}

		public override void ExitState ()
		{
			base.ExitState ();
		}

	}
}
