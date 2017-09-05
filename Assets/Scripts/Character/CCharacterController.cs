using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger  {
	public class CCharacterController : CObjectController, ICharacterContext {

        #region Fields

        [Header("Character Data")]
        [SerializeField]    protected TextAsset m_TextAsset;
        [SerializeField]    protected CCharacterData m_CharacterData;

		[Header("FSM")]
		[SerializeField]	protected TextAsset m_FSMTextAsset;
		[SerializeField]	protected string m_FSMStateName;
		[SerializeField]	protected Vector3 m_TargetPosition;
		[SerializeField]	protected CObjectController m_TargetObject;

		protected FSMManager m_FSMManager;

        #endregion

        #region Implementation Moonobehaviour

        public override void Init() {
            base.Init();
            // DATA
            this.m_CharacterData = TinyJSON.JSON.Load(this.m_TextAsset.text).Make<CCharacterData>();
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
			this.m_FSMManager.RegisterCondition("HaveAction", 			this.HaveAction);
			this.m_FSMManager.RegisterCondition("IsActive", 			this.IsActive);
			// GAME OBJECT
			this.m_TargetPosition = this.m_Transform.position;
        }

        protected override void Awake() {
			base.Awake();
			this.Init();
        }

		protected override void Update ()
		{
			base.Update ();
			this.m_FSMManager.UpdateState (Time.deltaTime);
			this.m_FSMStateName = this.m_FSMManager.currentStateName;
		}

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

		#region FSM

		public virtual bool DidMoveToTarget ()
		{
			var direction = this.targetPosition - this.objectPosition;
			return direction.sqrMagnitude <= 0.001f;
		}

		public virtual bool HaveAction ()
		{
			return false;
		}

		public virtual bool IsActive ()
		{
			return this.IsObjectActive;
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

		#endregion

    }
}
