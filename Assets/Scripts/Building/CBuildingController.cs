using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class CBuildingController : CDamageableObjectController, IBuildingJobOwner, IContext {

        #region Fields

		[Header("NPC")]
		[SerializeField]	protected GameObject[] m_NPCPoints;

		[Header("Detect other")]
		[SerializeField]	protected LayerMask m_DetectLayerMask;
		[SerializeField]	protected float m_DetectRadius = 1f;
		[SerializeField]	protected GameObject[] m_ExceptObjects;
		[SerializeField]	protected bool m_TriggerWithOther = false;
		[SerializeField]	protected Color m_NormalColor = Color.white;
		[SerializeField]	protected Color m_AlertColor = Color.red;

        [Header("Building Data")]
		[SerializeField]    protected CBuildingData m_BuidingData;

		[Header("FSM")]
		[SerializeField]	protected string m_FSMStateName;

		// FSMManager
		protected FSMManager m_FSMManager;
		// NPCs
		protected CNPCController[] m_NPCCtrls;

        #endregion

        #region Implementation Moonobehaviour

        public override void Init() {
            base.Init();
			if (this.m_BuidingData != null) {
				for (int i = 0; i < this.m_BuidingData.objectJobs.Length; i++) {
					var currentJob = this.m_BuidingData.objectJobs [i];
					this.m_JobComponent.RegisterJobs (this, currentJob, null, null, null);
				}
			}
			if (string.IsNullOrEmpty (this.m_BuidingData.objectFSMPath) == false) {
				// TEXT ASSET
				var fsmTextAsset = Resources.Load<TextAsset> (this.m_BuidingData.objectFSMPath);
				// FSM
				this.m_FSMManager = new FSMManager ();
				this.m_FSMManager.LoadFSM (fsmTextAsset.text);
				// STATE
				this.m_FSMManager.RegisterState ("BuildingIdleState", 		new FSMBuildingIdleState (this));
				this.m_FSMManager.RegisterState ("BuildingActionState", 	new FSMBuildingActionState (this));
				this.m_FSMManager.RegisterState ("BuildingInactiveState", 	new FSMBuildingInactiveState (this));
				// CONDITION
				this.m_FSMManager.RegisterCondition ("IsConstructionCompleted", this.IsConstructionCompleted);
				this.m_FSMManager.RegisterCondition ("HaveAction", 			this.HaveAction);
				this.m_FSMManager.RegisterCondition ("IsActive", 			this.IsActive);
			}
			// NPC
			this.m_NPCCtrls = new CNPCController[this.m_BuidingData.NPCDatas.Length];
			CGameManager.Instance.LoadVillageObjects <CNPCController> (false, this.m_BuidingData.NPCDatas, () => {
				// TODO
			}, (index, objCtrl) => {
				var parentPosition = this.m_NPCPoints[index % this.m_NPCPoints.Length].transform.position;
				this.m_NPCCtrls[index] = objCtrl as CNPCController;
				this.m_NPCCtrls[index].SetPosition (parentPosition);
				this.m_NPCCtrls[index].gameObject.SetActive (false);
			});
        }	

        protected override void Awake() {
            base.Awake();
        }

        protected override void Start() {
            base.Start();
        }

		protected override void Update () {
			base.Update ();
			if (this.m_Inited == false || this.m_FSMManager == null)
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
			this.m_TriggerWithOther = false;
			for (int i = 0; i < rayHit2Ds.Length; i++) {
				var collider = rayHit2Ds [i].collider;
				this.m_TriggerWithOther |= Array.IndexOf (this.m_ExceptObjects, collider.gameObject) == -1;
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

		public virtual bool IsConstructionCompleted () {
			if (this.m_BuidingData == null)
				return false;
			return this.m_BuidingData.percentConstruction >= 100f;
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

		public override CJobObjectData[] GetJobDatas ()
		{
			return this.m_BuidingData.objectJobs;
		}

		public override void SetPosition (Vector3 value)
		{
			base.SetPosition (value);
			this.m_BuidingData.objectV3Position = value;
		}

        public virtual void SetCurrentResident(int value) {
			this.m_BuidingData.currentResident = value >= this.m_BuidingData.maxResident ? this.m_BuidingData.maxResident : value;
        }

        public virtual int GetCurrentResident() {
			return this.m_BuidingData.currentResident;
        }

		public virtual CNPCController[] GetNPCControllers() {
			return this.m_NPCCtrls;
		}

		public virtual GameObject[] GetNPCPoints() {
			return this.m_NPCPoints;
		}

		public override string GetObjectType ()
		{
			return m_BuidingData.objectType;
		}

        #endregion

    }
}
