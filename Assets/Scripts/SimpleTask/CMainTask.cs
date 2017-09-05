using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneTask;

namespace WarOfSlinger {
	public class CMainTask : CSimpleTask {

		#region Properties

		#endregion

		#region Constructor

		public CMainTask () : base ()
		{
			this.taskName = "MainScene";
			this.nextTask = "IntroScene";
		}

		#endregion

		#region Implementation Task

		public override void StartTask ()
		{
			base.StartTask ();
		}

		#endregion

	}
}
