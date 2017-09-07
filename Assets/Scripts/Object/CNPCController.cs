using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleComponent;

namespace WarOfSlinger {
	public class CNPCController : CObjectController {

		#region Fields

		[Header("NPC Data")]
		[SerializeField]    protected TextAsset m_TextAsset;
		[SerializeField]    protected CObjectData m_ObjectData;

		// COMPONENTS
		protected CJobComponent m_JobComponent;

		#endregion

		#region Implementation Monobehaviour

		public override void Init() {
			base.Init();
			// DATA
			this.m_ObjectData = TinyJSON.JSON.Load(this.m_TextAsset.text).Make<CObjectData>();
			// REGISTER COMPONENT
			this.m_JobComponent = new CJobComponent(this);
			for (int i = 0; i < this.m_ObjectData.objectJobs.Length; i++) {
				var currentJob = this.m_ObjectData.objectJobs [i];
				this.m_JobComponent.RegisterJobs (this, currentJob, null, null, null);
			}
			this.RegisterComponent(this.m_JobComponent);
			// RANDOM NPC
			this.SetAnimation ("AnimParam", UnityEngine.Random.Range (1, 7));
		}

		protected override void Awake() {
			base.Awake();
			this.Init();
		}

		#endregion

		#region Main methods

		public override void ExcuteJobOwner(string jobName) {
			base.ExcuteJobOwner (jobName);
			this.m_JobComponent.ExcuteActiveJob (this, jobName);
		}

		public override void ClearJobOwner (string name)
		{
			base.ClearJobOwner (name);
			this.m_JobComponent.ClearJob (this, name);
		}

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
