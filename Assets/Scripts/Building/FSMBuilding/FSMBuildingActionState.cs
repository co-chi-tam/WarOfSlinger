using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class FSMBuildingActionState : FSMBaseState {

		protected CBuildingController m_Controller;

		public FSMBuildingActionState (IContext context): base(context)
		{
			this.m_Controller = context as CBuildingController;
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
