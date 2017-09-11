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
			this.SetAnimation ("AnimParam", UnityEngine.Random.Range (1, 7));
			this.objectSide = (int) Time.time % 2 == 0 ? 1 : -1;
		}

		#endregion
		
	}
}
