using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger  {
	public class CCharacterController : CObjectController, ICharacterContext, IJobLabor, ICharacterJobOwner {

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
			for (int i = 0; i < this.m_CharacterData.objectJobs.Length; i++) {
				var currentJob = this.m_CharacterData.objectJobs [i];
				this.m_JobComponent.RegisterJobs (this, currentJob, null, null, null);
			}
			// FSM
			this.m_FSMManager = new FSMManager ();
			this.m_FSMManager.LoadFSM (this.m_FSMTextAsset.text);
			// STATE
			this.m_FSMManager.RegisterState ("CharacterIdleState", 			new FSMCharacterIdleState(this));
			this.m_FSMManager.RegisterState ("CharacterMoveState", 			new FSMCharacterMoveState(this));
			this.m_FSMManager.RegisterState ("CharacterActionState", 		new FSMCharacterActionState(this));
			this.m_FSMManager.RegisterState ("CharacterDeathState", 		new FSMCharacterDeathState(this));
			this.m_FSMManager.RegisterState ("CharacterFoundTargetState", 	new FSMCharacterFoundTargetState(this));
			// CONDITION
			this.m_FSMManager.RegisterCondition("DidMoveToTarget", 		this.DidMoveToTarget);
			this.m_FSMManager.RegisterCondition("HaveTargetObject", 	this.HaveTargetObject);
			this.m_FSMManager.RegisterCondition("IsActive", 			this.IsActive);
			this.m_FSMManager.RegisterCondition("After30Second", 		this.After30Second);
			this.m_FSMManager.RegisterCondition("After60Second", 		this.After60Second);
			this.m_FSMManager.RegisterCondition("After90Second", 		this.After90Second);
			// GAME OBJECT
			this.m_TargetPosition = this.m_Transform.position;
        }

        protected override void Awake() {
			base.Awake();
        }

		protected override void Start() {
			base.Start ();
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

		public override void OnDamageObject (Vector2 point, CObjectController target, int damage)
		{
			base.OnDamageObject (point, target, damage);
			this.SetAnimation ("IsHit");
			var totalDamage = this.GetCurrentHealth () - damage;
			this.SetCurrentHealth (totalDamage);
			Debug.Log (target.name + " ==> " + damage); 
		}

		#endregion

		#region FSM

		public virtual bool DidMoveToTarget ()
		{
			var direction = this.targetPosition - this.objectPosition;
			var distance = 0.01f;
			return direction.sqrMagnitude <= distance * distance;
		}

		public virtual bool HaveTargetObject() {
			return this.m_TargetObject != null 
				&& this.m_TargetObject.GetActive();
		}

		public override bool IsActive ()
		{
			return base.IsActive () && this.GetCurrentHealth() > 0;
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

		public override bool GetActive ()
		{
			return base.GetActive () && this.GetCurrentHealth() > 0;
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
			this.targetObject = value;
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

		public override string GetObjectType ()
		{
			return m_CharacterData.objectType;
		}

		public virtual int GetConsumeFood() {
			return this.m_CharacterData.consumeFood;
		}

		public override int GetCurrentHealth() {
			base.GetCurrentHealth ();
			return this.m_CharacterData.currentHealth;
		}

		public override void SetCurrentHealth(int value) {
			base.SetCurrentHealth (value);
			this.m_CharacterData.currentHealth = value > this.m_CharacterData.maxHealth ? this.m_CharacterData.maxHealth : value;
		}

		public override int GetMaxHealth() {
			base.GetMaxHealth ();
			return this.m_CharacterData.maxHealth;
		}

		public virtual int GetDamageBuilding() {
			return this.m_CharacterData.damageBuilding;
		}

		public virtual int GetDamageCharacter() {
			return this.m_CharacterData.damageCharacter;
		}

		public override float GetActionSpeed ()
		{
			return this.m_CharacterData.actionSpeed;
		}

		public virtual void SetSide(Vector3 targetPosition) {
			var currentPosition = this.objectPosition;
			var direction 		= targetPosition - currentPosition;
			this.objectSide 	= direction.x >= 0f ? 1f : -1f;
		}

		#endregion

    }
}
