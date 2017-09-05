using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class FSMObjectInactiveState : FSMBaseState {

		public FSMObjectInactiveState (IContext context): base(context)
		{

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
