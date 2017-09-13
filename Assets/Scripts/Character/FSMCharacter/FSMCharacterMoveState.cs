using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class FSMCharacterMoveState : FSMBaseState {

		protected CCharacterController m_Controller;

		protected CCharacterData m_CharacterData;

		public FSMCharacterMoveState (IContext context): base(context)
		{
			this.m_Controller = context as CCharacterController;
		}

		public override void StartState ()
		{
			base.StartState ();
			// ANIMATOR
			this.m_Controller.SetAnimation ("AnimParam", (int) 1);
			// DATA
			this.m_CharacterData = this.m_Controller.GetData () as CCharacterData;
		}

		public override void UpdateState (float dt)
		{
			base.UpdateState (dt);
			// CURRENT POSITION
			var currentPosition = this.m_Controller.objectPosition;
			// TARGET POSITION
			var targetPosition = this.m_Controller.targetPosition;
			// DIRECTION
			var direction = targetPosition - currentPosition;
			var movePostion = direction.normalized;
			// DATA
			var moveSpeed = 1f / this.m_CharacterData.actionSpeed;
			// CALCULATE POSITION
			currentPosition += movePostion * moveSpeed * dt;
			this.m_Controller.objectPosition = currentPosition;
			this.m_Controller.objectSide = direction.x >= 0f ? 1f : -1f;
		}

		public override void ExitState ()
		{
			base.ExitState ();
		}

	}
}
