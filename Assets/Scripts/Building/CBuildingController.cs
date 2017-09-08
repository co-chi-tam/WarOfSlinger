using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class CBuildingController : CDamageableObjectController, IBuildingJobOwner, IContext {

        #region Fields

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

        #endregion

		#region Main methods

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
