using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class CEnvironmentObjectController : CDamageableObjectController, IJobOwner, IContext {

		#region Fields

		[Header("Object Data")]
		[SerializeField]    protected TextAsset m_ObjectTextAsset;
		[SerializeField]    protected CDamageableObjectData m_ObjectData;

		[Header("FSM")]
		[SerializeField]	protected TextAsset m_FSMTextAsset;
		[SerializeField]	protected string m_FSMStateName;

		// COMPONENTS
		protected CJobComponent m_JobComponent;
		// FSMManager
		protected FSMManager m_FSMManager;

		#endregion

		#region Implementation Moonobehaviour

		public override void Init() {
			base.Init();
			// DATA
			this.m_ObjectData = TinyJSON.JSON.Load(this.m_ObjectTextAsset.text).Make<CDamageableObjectData>();
			// REGISTER COMPONENT
			this.m_JobComponent = new CJobComponent(this);
			for (int i = 0; i < this.m_ObjectData.objectJobs.Length; i++) {
				var currentJob = this.m_ObjectData.objectJobs [i];
				this.m_JobComponent.RegisterJobs (this, currentJob, null, null);
			}
			this.RegisterComponent(this.m_JobComponent);
			// FSM
			this.m_FSMManager = new FSMManager ();
			this.m_FSMManager.LoadFSM (this.m_FSMTextAsset.text);
			// STATE
			this.m_FSMManager.RegisterState ("ObjectIdleState", 		new FSMObjectIdleState(this));
			this.m_FSMManager.RegisterState ("ObjectInactionState", 	new FSMObjectInactiveState(this));
			// CONDITION
			this.m_FSMManager.RegisterCondition("IsActive", 			this.IsActive);
		}

		protected override void Awake() {
			// TEST
			this.Init();
			base.Awake();
		}

		protected override void Start() {
			base.Start();
		}

		protected override void Update () {
			base.Update ();
			this.m_FSMManager.UpdateState (Time.deltaTime);
			this.m_FSMStateName = this.m_FSMManager.currentStateName;
		}

		#endregion

		#region Main methods

		public override void ExcuteJob(string jobName) {
			base.ExcuteJob (jobName);
			this.m_JobComponent.ExcuteActiveJob (this, jobName);
		}

		#endregion

		#region FSM

		public virtual bool IsActive ()
		{
			return this.IsObjectActive;
		}

		#endregion

		#region Getter && Setter

		public override void SetData(CObjectData value) {
			base.SetData(value);
			this.m_ObjectData = value as CDamageableObjectData;
		}

		public override CObjectData GetData() {
			base.GetData();
			return this.m_ObjectData;
		}

		#endregion
		
	}
}
