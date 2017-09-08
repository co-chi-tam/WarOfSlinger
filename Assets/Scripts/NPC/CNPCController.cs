using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleComponent;

namespace WarOfSlinger {
	public class CNPCController : CObjectController {

		#region Fields

		[Header("NPC Data")]
		[SerializeField]    protected CObjectData m_ObjectData;

		#endregion

		#region Implementation Monobehaviour

		public override void Init() {
			base.Init();
			// DATA
//			this.m_ObjectData = TinyJSON.JSON.Load(this.m_TextAsset.text).Make<CObjectData>();
			for (int i = 0; i < this.m_ObjectData.objectJobs.Length; i++) {
				var currentJob = this.m_ObjectData.objectJobs [i];
				this.m_JobComponent.RegisterJobs (this, currentJob, null, null, null);
			}
			// RANDOM NPC
			this.SetAnimation ("AnimParam", UnityEngine.Random.Range (1, 7));
			this.objectSide = (int) Time.time % 2 == 0 ? 1 : -1;
		}

		protected override void Awake() {
			base.Awake();
		}

		#endregion

		#region Main methods

		#endregion

		#region Getter && Setter

		public override void SetData (CObjectData value)
		{
			base.SetData (value);
			this.m_ObjectData = value;
		}

		public override CObjectData GetData ()
		{
			base.GetData ();
			return this.m_ObjectData;
		}

		#endregion

	}
}
