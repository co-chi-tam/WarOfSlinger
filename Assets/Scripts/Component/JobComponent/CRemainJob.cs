using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
	public class CRemainJob: CJobObjectData {

		public Action OnJobCompleted;
		public Action<float> OnJobProcessing;

		public CRemainJob ()
		{

		}
		
	}
}
