using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class FSMCharacterIdleState : FSMBaseState {

		protected CCharacterController m_Controller;

		public FSMCharacterIdleState (IContext context): base(context)
		{
			this.m_Controller = context as CCharacterController;
		}

		public override void StartState ()
		{
			base.StartState ();
			this.m_Controller.SetAnimation ("AnimParam", (int) 0);
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
