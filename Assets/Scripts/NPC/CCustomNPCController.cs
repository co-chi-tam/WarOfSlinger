using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger  {
	public class CCustomNPCController : CCharacterController {

		#region Fields

		[Header("Custom Data")]
		[SerializeField]	protected TextAsset m_TextAsset;

		#endregion

		#region Implementation Monobehaviour

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

		#endregion

		#region Main methods

		public override void OnDamageObject (Vector2 point, CObjectController target, int damage)
		{
			base.OnDamageObject (point, target, damage);
			if (this.m_TargetObject == null || this.m_TargetObject.GetActive() == false) {
				this.m_TargetObject = target;
			}
		}

		public override void ResetOject ()
		{
			base.ResetOject ();
			this.m_CharacterData.currentHealth = this.m_CharacterData.maxHealth;
		}

		#endregion
		
	}
}
