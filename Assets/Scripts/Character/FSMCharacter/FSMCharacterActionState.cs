using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class FSMCharacterActionState : FSMBaseState {

		protected CCharacterController m_Controller;

		public FSMCharacterActionState (IContext context): base(context)
		{
			this.m_Controller = context as CCharacterController;
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
