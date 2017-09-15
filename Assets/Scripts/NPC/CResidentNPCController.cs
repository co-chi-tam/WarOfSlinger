using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleComponent;

namespace WarOfSlinger {
	public class CResidentNPCController : CNPCController {

		#region Implementation Monobehaviour

		public override void Init() {
			base.Init();
			// RANDOM NPC
			this.OnRandomNPC ();
		}

		protected override void OnEnable ()
		{
			base.OnEnable ();
			// RANDOM NPC
			this.OnRandomNPC ();
		}

		#endregion

		#region Main methods

		private void OnRandomNPC() {
			this.SetAnimation ("AnimParam", UnityEngine.Random.Range (1, 7));
		}

		#endregion
		
	}
}
