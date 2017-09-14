using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class FSMCharacterFoundTargetState : FSMBaseState {

		protected CCharacterController m_Controller;
		protected string[] m_TargetTypes = new string[] { "building", "character" };

		public FSMCharacterFoundTargetState (IContext context): base(context)
		{
			this.m_Controller = context as CCharacterController;
		}

		public override void StartState ()
		{
			base.StartState ();
			this.m_Controller.ResetTimer ();
			this.m_Controller.SetAnimation ("AnimParam", (int) 0);

			var randomType 		= (int)Time.time % (this.m_TargetTypes.Length + this.m_TargetTypes.Length);
			if (randomType < this.m_TargetTypes.Length) {
				var targetList 		= CGameManager.Instance.FindObjectWith (this.m_TargetTypes[randomType]);

				var randomTarget 	= (int)Time.time % targetList.Length;
				var currentTarget 	= targetList[randomTarget];

				this.m_Controller.SetTargetPosition (currentTarget.GetClosestPoint (this.m_Controller.GetPosition ()));
				this.m_Controller.SetTargetController (currentTarget);
			}
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
