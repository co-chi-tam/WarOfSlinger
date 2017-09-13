using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class FSMObjectInactiveState : FSMBaseState {

		protected CObjectController m_Controller;

		public FSMObjectInactiveState (IContext context): base(context)
		{
			this.m_Controller = context as CObjectController;
		}

		public override void StartState ()
		{
			base.StartState ();
			this.m_Controller.SetActive (false);
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
