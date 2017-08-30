using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleComponent;

namespace WarOfSlinger {
    public class CJobComponent : CComponent {

        #region Fields

        protected List<CJobRemain> m_JobRemains;
        protected Dictionary<string, Action<IJobOwner, string[]>> m_JobMap;

        public Action<CJobRemain> OnJobActive;

        #endregion

        #region Internal class

        public class CJobRemain : CJobObjectData {
            public float jobUpdateTimer;
            public IJobOwner jobOwner;

            public CJobRemain(): base() {
                this.jobUpdateTimer = 0f;
            }
        }

        #endregion

        #region Constructor

        public CJobComponent() : base () {
            this.m_JobRemains   = new List<CJobRemain>();
            this.m_JobMap       = new Dictionary<string, Action<IJobOwner, string[]>>();
            this.m_JobMap.Add("GoldPerResident", this.GoldPerResident);
        }

        #endregion

        #region Implementation Component

        // START
        public override void StartComponent() {
            base.StartComponent();
        }

        // UPDATE
        public override void UpdateComponent(float dt) {
            base.UpdateComponent(dt);
            this.UpdateJobs(dt);
        }

        #endregion

        #region Job Main methods

        // REGISTER NEW JOB
        public virtual void RegisterJobs(IJobOwner owner, CJobObjectData value) {
            // NEW JOB
            var newJob              = new CJobRemain();
            newJob.jobName          = value.jobName;
            newJob.jobDescription   = value.jobDescription;
            newJob.jobValues        = new string[value.jobValues.Length];
            Array.Copy(value.jobValues, newJob.jobValues, value.jobValues.Length);
            newJob.jobTimer         = value.jobTimer;
            newJob.jobUpdateTimer   = value.jobTimer;
            newJob.jobOwner         = owner;
            // ADD JOB
            this.m_JobRemains.Add(newJob);
        }

        // UPDATE JOB TIMER
        public virtual void UpdateJobs(float dt) {
            for (int i = 0; i < this.m_JobRemains.Count; i++) {
                // CURRENT JOB
                var currentJob = this.m_JobRemains[i];
                // COUNT DOWN TIMER
                if (currentJob.jobUpdateTimer > 0f) {
                    currentJob.jobUpdateTimer -= dt;
                }
                // EXCUTE JOB DATA
                else {
                    // EXCUTE JOB
                    this.ExcuteJob(currentJob);
                    // UPDATE JOB TIMER
                    currentJob.jobUpdateTimer = currentJob.jobTimer;
                }
            }
        }

        // UNREGISTER JOB
        public virtual void UnregisterJobs(CJobObjectData value) {
            for (int i = 0; i < this.m_JobRemains.Count; i++) {
                var currentJob = this.m_JobRemains[i];
                if (currentJob.jobName == value.jobName) {
                    this.m_JobRemains.Remove(currentJob);
                    i--;
                }
            }
        }

        // EXCUTE JOB
        public virtual void ExcuteJob(CJobRemain value) {
            if (this.m_JobMap.ContainsKey(value.jobName) == false)
                return;
            // CALLBACK JOB
            this.m_JobMap[value.jobName](value.jobOwner, value.jobValues);
            // CALLBACK EVENT
            if (this.OnJobActive != null) {
                this.OnJobActive(value);
            }
        }

        // GOLD PER RESIDENT JOB
        protected virtual void GoldPerResident(IJobOwner owner, string[] values) {
            var claimedGold = int.Parse (values[0]);
            var perResident = int.Parse (values[1]);
            var buildingOwner = owner as IBuildingJobOwner;
        }

        #endregion

    }
    
}
