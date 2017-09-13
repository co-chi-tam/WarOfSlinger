using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class FSMCharacterDeathState : FSMBaseState {

		protected CCharacterController m_Controller;

		public FSMCharacterDeathState (IContext context): base(context)
		{
			this.m_Controller = context as CCharacterController;
		}

		public override void StartState ()
		{
			base.StartState ();
			this.m_Controller.SetActive (false);
			this.m_Controller.IsObjectWorking = false;
			this.m_Controller.SetAnimation ("AnimParam", (int) 10);
			CJobManager.ReleaseLabor (this.m_Controller);
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
