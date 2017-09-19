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
			for (int i = 0; i < this.m_ObjectData.objectJobs.Length; i++) {
				var currentJob = this.m_ObjectData.objectJobs [i];
				this.m_JobComponent.RegisterJobs (this, currentJob, null, null, null);
			}
			// SIDE
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

		public override CJobObjectData[] GetJobDatas ()
		{
			return this.m_ObjectData.objectJobs;
		}

		public override string GetObjectType ()
		{
			return m_ObjectData.objectType;
		}

		#endregion

	}
}
