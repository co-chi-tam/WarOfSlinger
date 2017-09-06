using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class FSMCharacterActionState : FSMBaseState {

		protected CCharacterController m_Controller;
		protected float m_AnimationDelay = 0f;

		public FSMCharacterActionState (IContext context): base(context)
		{
			this.m_Controller = context as CCharacterController;
		}

		public override void StartState ()
		{
			base.StartState ();
			this.m_AnimationDelay = 1f;
		}

		public override void UpdateState (float dt)
		{
			base.UpdateState (dt);
			var target = this.m_Controller.targetObject;
			if (this.m_AnimationDelay > 0f) {
				this.m_AnimationDelay -= dt;
			} else {
				var colliderPoint = target.colliderPoint;
				target.OnDamageObject (colliderPoint, 30);
				this.m_AnimationDelay = 1f;
				this.m_Controller.SetAnimation ("IsAttack");
			}
			if (target != null) {
				this.m_Controller.targetPosition = target.GetClosestPoint (this.m_Controller.objectPosition);
			}
		}

		public override void ExitState ()
		{
			base.ExitState ();
		}

	}
}
