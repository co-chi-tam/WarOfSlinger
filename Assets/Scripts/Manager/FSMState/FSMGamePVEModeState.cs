using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class FSMGamePVEModeState : FSMBaseState {

		protected CGameManager m_GameManager;

		public FSMGamePVEModeState (IContext context): base(context)
		{
			this.m_GameManager = context as CGameManager;
		}

		public override void StartState ()
		{
			base.StartState ();
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
