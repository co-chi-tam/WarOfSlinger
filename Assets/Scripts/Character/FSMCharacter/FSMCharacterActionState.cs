using System;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class FSMCharacterActionState : FSMBaseState {

		protected CCharacterController m_Controller;
		protected float m_AnimationDelay = 0f;
		protected string[] m_BuildingType = new string[] { "building", "environment" };
		protected string[] m_CharacterType = new string[] { "character", "dinosaur" };

		public FSMCharacterActionState (IContext context): base(context)
		{
			this.m_Controller = context as CCharacterController;
		}

		public override void StartState ()
		{
			base.StartState ();
			this.m_AnimationDelay = this.m_Controller.GetActionSpeed();
		}

		public override void UpdateState (float dt)
		{
			base.UpdateState (dt);
			var target = this.m_Controller.targetObject;
			if (this.m_AnimationDelay > 0f) {
				this.m_AnimationDelay -= dt;
			} else {
				var colliderPoint = target.colliderPoint;
				var totalDamage = 1;
				// DAMAGE BUILDING
				if (Array.IndexOf (this.m_BuildingType, target.GetObjectType ()) != -1) {
					totalDamage = this.m_Controller.GetDamageBuilding ();
				} else if (Array.IndexOf (this.m_CharacterType, target.GetObjectType ()) != -1) {
					// DAMAGE CHARACTER
					totalDamage = this.m_Controller.GetDamageCharacter ();
				}
				// DAMAGE
				target.OnDamageObject (colliderPoint, this.m_Controller, totalDamage);
				this.m_AnimationDelay = this.m_Controller.GetActionSpeed();
				this.m_Controller.SetAnimation ("IsAttack");
			}
			if (target != null) {
				this.m_Controller.targetPosition = target.GetClosestPoint (this.m_Controller.GetPosition());
			}
		}

		public override void ExitState ()
		{
			base.ExitState ();
		}

	}
}
