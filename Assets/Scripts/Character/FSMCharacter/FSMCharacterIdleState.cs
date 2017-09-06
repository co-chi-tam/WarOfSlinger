using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class FSMCharacterIdleState : FSMBaseState {

		protected CCharacterController m_Controller;
		protected float m_JobReleaseDelay = 1f;

		public FSMCharacterIdleState (IContext context): base(context)
		{
			this.m_Controller = context as CCharacterController;
		}

		public override void StartState ()
		{
			base.StartState ();
			this.m_Controller.SetAnimation ("AnimParam", (int) 0);
			this.m_JobReleaseDelay = 1f;
		}

		public override void UpdateState (float dt)
		{
			base.UpdateState (dt);
			if (this.m_JobReleaseDelay > 0f) {
				this.m_JobReleaseDelay -= dt;
			} else {
				// FREE LABOR
				if (this.m_Controller.DidMoveToTarget () == true && this.m_Controller.HaveTargetObject () == false) {
					CJobManager.ReturnFreeLabor (this.m_Controller);
				}
				this.m_JobReleaseDelay = 1f;
			}
		}

		public override void ExitState ()
		{
			base.ExitState ();
		}
		
	}
}
