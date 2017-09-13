using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger  {
	public class CDinosaurController : CCharacterController {

		[SerializeField]	protected TextAsset m_TextAsset;

		public override void Init() {
			// DATA
			this.m_CharacterData = TinyJSON.JSON.Load(this.m_TextAsset.text).Make<CCharacterData>();
			base.Init();
		}

		protected override void Start ()
		{
			base.Start ();
			this.Init ();
		}
		
	}
}
