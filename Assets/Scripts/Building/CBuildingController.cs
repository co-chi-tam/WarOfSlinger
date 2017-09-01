using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
	public class CBuildingController : CDamageableObjectController, IBuildingJobOwner {

        #region Fields

        [Header("Building Data")]
        [SerializeField]    protected TextAsset m_BuildingTextAsset;
        [SerializeField]    protected CBuildingData m_BuidingData;

        // COMPONENTS
        protected CJobComponent m_JobComponent;

        #endregion

        #region Implementation Moonobehaviour

        public override void Init() {
            base.Init();
            // DATA
            this.m_BuidingData = TinyJSON.JSON.Load(this.m_BuildingTextAsset.text).Make<CBuildingData>();
            // JOB COMPONENT
            this.m_JobComponent = new CJobComponent();
            var buildingJobs = this.m_BuidingData.buildingJobs;
            for (int i = 0; i < buildingJobs.Length; i++) {
                this.m_JobComponent.RegisterJobs(this, buildingJobs[i]);
            } 
            // REGISTER COMPONENT
            this.RegisterComponent(this.m_JobComponent);
        }

        protected override void Awake() {
            // TEST
            this.Init();
            base.Awake();
        }

        protected override void Start() {
            base.Start();
        }

        #endregion

        #region Main methods

        #endregion

        #region Getter && Setter

        public override void SetData(CObjectData value) {
            base.SetData(value);
            this.m_BuidingData = value as CBuildingData;
        }

        public override CObjectData GetData() {
            base.GetData();
            return this.m_BuidingData;
        }

        public virtual void SetCurrentResident(int value) {

        }

        public virtual int GetCurrentResident() {
            return 0;
        }

        #endregion

    }
}
