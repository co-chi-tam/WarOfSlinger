using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
    public class CBuildingController : CObjectController, IBuildingJobOwner {

        #region Fields

        [Header("Building Data")]
        [SerializeField]    protected TextAsset m_BuildingTextAsset;
        [SerializeField]    protected CBuildingData m_BuidingData;

        [Header("Object Control")]
        [SerializeField]    protected RedrawableCollider m_RedrawCollider;
        [SerializeField]    protected RedrawableSprite m_RedrawSprite;

        [Header("Build timer")]
        [SerializeField]    protected float m_DelayTime = 0.1f;
        [SerializeField]    protected float m_CountDownActiveTime = 1f;

        protected bool m_BuildingActive = false;
        protected float m_Delay = 0.1f;
        protected float m_CountDownActive = 1f;

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
            // REGISTER EVENT
            this.m_RedrawCollider.OnEventColliderObject -= OnBuildingCollider;
            this.m_RedrawCollider.OnEventColliderObject += OnBuildingCollider;
        }

        protected override void Start() {
            base.Start();
        }

        protected override void Update() {
            base.Update();
            // DELAY TIMER
            if (this.m_Delay > 0f) {
                this.m_Delay -= Time.deltaTime;
            }
            // COUNTDOWN TIMER
            if (this.m_CountDownActive > 0f) {
                this.m_CountDownActive -= Time.deltaTime;
            }
            // UPDATE BUILDING ACTIVE
            this.m_BuildingActive = this.m_Delay < 0f && this.m_CountDownActive < 0f;
        }

        #endregion

        #region Main methods

        protected virtual void OnBuildingCollider(Vector2 point, GameObject obj) {
            // BUILDING ACTIVED
            if (this.m_BuildingActive == false)
                return;
            // TEST
            this.m_RedrawSprite.Draw(point.x, point.y, 30);
            this.m_CountDownActive = this.m_CountDownActiveTime;

        }

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
