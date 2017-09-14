using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class FSMGamePlayingModeState : FSMBaseState {

		protected CGameManager m_GameManager;
		protected string[] m_ApplyObjectType;

		public FSMGamePlayingModeState (IContext context): base(context)
		{
			this.m_GameManager = context as CGameManager;
		}

		public override void StartState ()
		{
			base.StartState ();
			this.m_GameManager.OnEventTouchedGameObject = this.OnOpenUIJobPanel;
		}

		public override void UpdateState (float dt)
		{
			base.UpdateState (dt);
#if UNITY_EDITOR || UNITY_STANDALONE
			this.m_GameManager.OnMouseDetectStandalone();
#else
			this.m_GameManager.OnMouseDetectMobile();
#endif
			this.m_GameManager.CalculatePopular ();
		}

		public override void ExitState ()
		{
			base.ExitState ();
		}

		protected virtual void OnOpenUIJobPanel(GameObject detectedGo) {
			this.m_GameManager.OnOpenUIJobPanel (detectedGo);
//			this.m_GameManager.OnOpenUIInfoPanel (detectedGo);
		}

	}
}
