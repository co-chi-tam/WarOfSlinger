using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class CBuildingController : CDamageableObjectController, IBuildingJobOwner, IContext {

        #region Fields

		[Header("Detect other")]
		[SerializeField]	protected LayerMask m_DetectLayerMask;
		[SerializeField]	protected float m_DetectRadius = 1f;
		[SerializeField]	protected Collider2D[] m_ExceptCollider;
		[SerializeField]	protected bool m_TriggerWithOther = false;
		[SerializeField]	protected Color m_NormalColor = Color.white;
		[SerializeField]	protected Color m_AlertColor = Color.red;

        [Header("Building Data")]
		[SerializeField]    protected CBuildingData m_BuidingData;

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
//            this.m_BuidingData = TinyJSON.JSON.Load(this.m_BuildingTextAsset.text).Make<CBuildingData>();
			for (int i = 0; i < this.m_BuidingData.objectJobs.Length; i++) {
				var currentJob = this.m_BuidingData.objectJobs [i];
				this.m_JobComponent.RegisterJobs (this, currentJob, null, null, null);
			}
			// FSM
			this.m_FSMManager = new FSMManager ();
			this.m_FSMManager.LoadFSM (this.m_FSMTextAsset.text);
			// STATE
			this.m_FSMManager.RegisterState ("BuildingIdleState", 		new FSMBuildingIdleState(this));
			this.m_FSMManager.RegisterState ("BuildingActionState", 	new FSMBuildingActionState(this));
			this.m_FSMManager.RegisterState ("BuildingInactiveState", 	new FSMBuildingInactiveState(this));
			// CONDITION
			this.m_FSMManager.RegisterCondition("HaveAction", 			this.HaveAction);
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

		protected virtual void LateUpdate() {
			this.OnDetectColliderWithOther ();
			this.OnObjectChangeColor ();
		}

		protected virtual void OnDrawGizmos() {
			Gizmos.DrawWireSphere (this.transform.position, this.m_DetectRadius);
		}

        #endregion

		#region Main methods

		protected virtual void OnDetectColliderWithOther() {
			var originPosition = this.m_Transform.position;
			var rayHit2Ds = Physics2D.CircleCastAll (originPosition, this.m_DetectRadius, Vector2.zero, 0f, this.m_DetectLayerMask);
			for (int i = 0; i < rayHit2Ds.Length; i++) {
				var collider = rayHit2Ds [i].collider;
				this.m_TriggerWithOther = Array.IndexOf (this.m_ExceptCollider, collider) == -1;
				this.m_IsObjectWorking = !this.m_TriggerWithOther;
			}
		}

		protected virtual void OnObjectChangeColor() {
			this.SetColor (this.m_TriggerWithOther ? this.m_AlertColor : this.m_NormalColor);
		}

		#endregion

		#region FSM

		public virtual bool HaveAction ()
		{
			return false;
		}

		#endregion

        #region Getter && Setter

		public override bool GetActive ()
		{
			return this.m_RedrawSprite.IsSpriteVisible && base.GetActive();
		}

        public override void SetData(CObjectData value) {
            base.SetData(value);
            this.m_BuidingData = value as CBuildingData;
        }

        public override CObjectData GetData() {
            base.GetData();
            return this.m_BuidingData;
        }

		public override void SetPosition (Vector3 value)
		{
			base.SetPosition (value);
			this.m_BuidingData.objectV3Position = value;
		}

        public virtual void SetCurrentResident(int value) {
			
        }

        public virtual int GetCurrentResident() {
            return 0;
        }

        #endregion

    }
}
