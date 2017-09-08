using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class CEnvironmentObjectController : CDamageableObjectController, IJobOwner, IContext {

		#region Fields

		[Header("Object Data")]
		[SerializeField]    protected CDamageableObjectData m_ObjectData;

		[Header("FSM")]
		[SerializeField]	protected TextAsset m_FSMTextAsset;
		[SerializeField]	protected string m_FSMStateName;

		// FSMManager
		protected FSMManager m_FSMManager;

		#endregion

		#region Implementation Moonobehaviour

		public override void Init() {
			base.Init();
			// DATA
//			this.m_ObjectData = TinyJSON.JSON.Load(this.m_ObjectTextAsset.text).Make<CDamageableObjectData>();
			for (int i = 0; i < this.m_ObjectData.objectJobs.Length; i++) {
				var currentJob = this.m_ObjectData.objectJobs [i];
				this.m_JobComponent.RegisterJobs (this, currentJob, null, null, null);
			}
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
			base.Awake();
		}

		protected override void Start() {
			base.Start();
		}

		protected override void Update () {
			base.Update ();
			if (this.m_Inited == false)
				return;
			this.m_FSMManager.UpdateState (Time.deltaTime);
			this.m_FSMStateName = this.m_FSMManager.currentStateName;
		}

		#endregion

		#region Main methods

		#endregion

		#region Getter && Setter

		public override bool GetActive ()
		{
			return this.m_RedrawSprite.IsSpriteVisible && base.GetActive();
		}

		public override void SetActive (bool value)
		{
			base.SetActive (value);
			if (value) {
				this.m_RedrawSprite.SetupSprite ();
			}
		}

		public override void SetData(CObjectData value) {
			base.SetData(value);
			this.m_ObjectData = value as CDamageableObjectData;
		}

		public override CObjectData GetData() {
			base.GetData();
			return this.m_ObjectData;
		}

		public override void SetPosition (Vector3 value)
		{
			base.SetPosition (value);
			this.m_ObjectData.objectV3Position = value;
		}

		#endregion
		
	}
}
