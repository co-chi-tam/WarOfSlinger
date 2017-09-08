using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger  {
	public class CCharacterController : CObjectController, ICharacterContext, IJobLabor {

        #region Fields

        [Header("Character Data")]
        [SerializeField]    protected CCharacterData m_CharacterData;

		[Header("FSM")]
		[SerializeField]	protected TextAsset m_FSMTextAsset;
		[SerializeField]	protected string m_FSMStateName;
		[SerializeField]	protected Vector3 m_TargetPosition;
		[SerializeField]	protected CObjectController m_TargetObject;

		protected FSMManager m_FSMManager;

		#endregion

		#region Properties

		public override Vector3 targetPosition {
			get { return this.m_TargetPosition; }
			set { 
				value.y = 0;
				value.z = 0;
				this.m_TargetPosition = value; 
			}
		}

		public virtual CObjectController targetObject {
			get { return this.m_TargetObject; }
			set { this.m_TargetObject = value; }
		}

		#endregion

        #region Implementation Moonobehaviour

        public override void Init() {
            base.Init();
            // DATA
//            this.m_CharacterData = TinyJSON.JSON.Load(this.m_TextAsset.text).Make<CCharacterData>();
			for (int i = 0; i < this.m_CharacterData.objectJobs.Length; i++) {
				var currentJob = this.m_CharacterData.objectJobs [i];
				this.m_JobComponent.RegisterJobs (this, currentJob, null, null, null);
			}
			// FSM
			this.m_FSMManager = new FSMManager ();
			this.m_FSMManager.LoadFSM (this.m_FSMTextAsset.text);
			// STATE
			this.m_FSMManager.RegisterState ("CharacterIdleState", 		new FSMCharacterIdleState(this));
			this.m_FSMManager.RegisterState ("CharacterMoveState", 		new FSMCharacterMoveState(this));
			this.m_FSMManager.RegisterState ("CharacterActionState", 	new FSMCharacterActionState(this));
			this.m_FSMManager.RegisterState ("CharacterDeathState", 	new FSMCharacterDeathState(this));
			// CONDITION
			this.m_FSMManager.RegisterCondition("DidMoveToTarget", 		this.DidMoveToTarget);
			this.m_FSMManager.RegisterCondition("HaveTargetObject", 	this.HaveTargetObject);
			this.m_FSMManager.RegisterCondition("IsActive", 			this.IsActive);
			// GAME OBJECT
			this.m_TargetPosition = this.m_Transform.position;
			// SET IS FREE LABOR
			CJobManager.ReturnFreeLabor (this);
        }

        protected override void Awake() {
			base.Awake();
        }

		protected virtual void Start() {
			// REGISTER UI
			this.Talk ("Hello world !!!");
		}

		protected override void Update ()
		{
			base.Update ();
			if (this.m_Inited == false)
				return;
			this.m_FSMManager.UpdateState (Time.deltaTime);
			this.m_FSMStateName = this.m_FSMManager.currentStateName;
		}

         #endregion

		#region Main methods

		public override void ClearJobLabor() {
			base.ClearJobLabor ();
			this.targetPosition = this.transform.position;
			this.targetObject = null;
		}

		#endregion

		#region FSM

		public virtual bool DidMoveToTarget ()
		{
			var direction = this.targetPosition - this.objectPosition;
			return direction.sqrMagnitude <= 0.001f;
		}

		public virtual bool HaveTargetObject() {
			return this.m_TargetObject != null && this.m_TargetObject.GetCollider().enabled == true;
		}

		#endregion

		#region Getter && Setter

		public override void SetData (CObjectData value)
		{
			base.SetData (value);
			this.m_CharacterData = value as CCharacterData;
		}

		public override CObjectData GetData ()
		{
			base.GetData ();
			return this.m_CharacterData as CCharacterData;
		}

		public override void SetTargetPosition (Vector3 value)
		{
			base.SetTargetPosition (value);
			value.y = 0f;
			value.z = 0f;
			this.m_TargetPosition = value;
		}

		public override Vector3 GetTargetPosition ()
		{
			return this.m_TargetPosition;
		}

		public override void SetTargetController (CObjectController value)
		{
			base.SetTargetController (value);
			this.m_TargetObject = value;
		}

		public override CObjectController GetTargetController ()
		{
			return this.m_TargetObject;
		}

		public override void SetPosition (Vector3 value)
		{
			base.SetPosition (value);
			this.m_CharacterData.objectV3Position = value;
		}

		#endregion

    }
}
