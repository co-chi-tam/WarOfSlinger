using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneTask;

namespace WarOfSlinger {
	public class CSimpleMapTask : CMapTask {

		public override void LoadMap ()
		{
			base.LoadMap ();
			this.m_Map ["IntroScene"]			= new CIntroTask ();
			this.m_Map ["MainScene"]			= new CMainTask ();
		}
		
	}
}
