using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleComponent;

namespace WarOfSlinger {
    public class CJobComponent : CComponent {

        #region Fields

		protected IJobOwner m_Owner;
		protected List<CRemainJob> m_JobRemains;
		protected Dictionary<string, Action<IJobOwner, CRemainJob>> m_JobMap;
		protected Dictionary<string, object> m_JobCommandElement;

        #endregion

        #region Constructor

		public CJobComponent(IJobOwner owner) : base () {
			this.m_Owner 		= owner;
			this.m_JobRemains   = new List<CRemainJob>();
			this.m_JobMap       = new Dictionary<string, Action<IJobOwner, CRemainJob>>();
			this.m_JobMap.Add("GoldPerResident", 	this.GoldPerResident);
			this.m_JobMap.Add("WalkCommand",		this.WalkCommand);
			this.m_JobMap.Add("GatheringCommand",	this.GatheringCommand);
			this.m_JobMap.Add("CreateLaborCommand",	this.CreateLaborCommand);
			this.m_JobMap.Add("LoveCommand",		this.LoveCommand);
			this.m_JobMap.Add("MakeToolCommand",	this.MakeToolCommand);
			this.m_JobMap.Add("HatchEggCommand",	this.HatchEggCommand);

			this.m_JobCommandElement = new Dictionary <string, object> ();
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
		public virtual void RegisterJobs(IJobOwner owner, CJobObjectData value, Action onCompleted, Action<float> onProcessing) {
            // NEW JOB
			var newJob              = new CRemainJob();
            newJob.jobDisplayName	= value.jobDisplayName;
			newJob.jobAvatar 		= value.jobAvatar;
			newJob.jobExcute 		= value.jobExcute;
            newJob.jobDescription   = value.jobDescription;
            newJob.jobValues        = new string[value.jobValues.Length];
            Array.Copy(value.jobValues, newJob.jobValues, value.jobValues.Length);
            newJob.jobTimer         = value.jobTimer;
			newJob.jobCountdownTimer = value.jobTimer;
			newJob.jobType 			= value.jobType;
            newJob.jobOwner         = owner;
			newJob.OnJobCompleted 	= onCompleted;
			newJob.OnJobProcessing 	= onProcessing;
            // ADD JOB
            this.m_JobRemains.Add(newJob);
        }

        // UPDATE JOB TIMER
        public virtual void UpdateJobs(float dt) {
            for (int i = 0; i < this.m_JobRemains.Count; i++) {
                // CURRENT JOB
                var currentJob = this.m_JobRemains[i];
                // COUNT DOWN TIMER
                if (currentJob.jobCountdownTimer > 0f) {
                    currentJob.jobCountdownTimer -= dt;
					if (currentJob.OnJobProcessing != null) {
						currentJob.OnJobProcessing (currentJob.jobCountdownTimer / currentJob.jobTimer);
					}
                }
                else {
					if (currentJob.jobType == (int)CJobObjectData.EJobType.PassiveJob) {
						// EXCUTE JOB
						this.ExcuteActiveJob(currentJob);
					} 
                }
            }
        }

        // UNREGISTER JOB
        public virtual void UnregisterJobs(CJobObjectData value) {
            for (int i = 0; i < this.m_JobRemains.Count; i++) {
                var currentJob = this.m_JobRemains[i];
				if (currentJob.jobExcute == value.jobExcute) {
                    this.m_JobRemains.Remove(currentJob);
                    i--;
                }
            }
        }

        // EXCUTE JOB
		public virtual void ExcuteActiveJob(CRemainJob currentJob) {
			if (this.m_JobMap.ContainsKey(currentJob.jobExcute) == false)
                return;
			if (currentJob.jobCountdownTimer > 0f)
				return;
			// UPDATE JOB TIMER
			currentJob.jobCountdownTimer = currentJob.jobTimer;
            // CALLBACK JOB
			this.m_JobMap[currentJob.jobExcute](currentJob.jobOwner, currentJob);
        }

		// EXCUTE JOB
		public virtual void ExcuteActiveJob(IJobOwner owner, string jobName) {
			CRemainJob currentJob = null;
			for (int i = 0; i < this.m_JobRemains.Count; i++) {
				if (this.m_JobRemains[i].jobExcute == jobName 
					&& this.m_JobRemains[i].jobType == (int) CJobObjectData.EJobType.ActiveJob) {
					currentJob = this.m_JobRemains[i];
					break;
				}
			}
			if (currentJob == null)
				return;
			if (this.m_JobMap.ContainsKey(currentJob.jobExcute) == false)
				return;
			if (currentJob.jobCountdownTimer > 0f)
				return;
			// UPDATE JOB TIMER
			currentJob.jobCountdownTimer = currentJob.jobTimer;
			// CALLBACK JOB
			this.m_JobMap[currentJob.jobExcute](owner, currentJob);
		}

        // GOLD PER RESIDENT JOB
		protected virtual void GoldPerResident(IJobOwner owner, CRemainJob currentJob) {
			var buildingOwner 	= owner as IBuildingJobOwner;
			var values 			= currentJob.jobValues;
            var claimedGold 	= int.Parse (values[0]);
            var perResident 	= int.Parse (values[1]);
			if (currentJob.OnJobCompleted != null) {
				currentJob.OnJobCompleted ();
			}
			Debug.Log (claimedGold + " / " + perResident);
        }

		// WALK TO
		protected virtual void WalkCommand(IJobOwner owner, CRemainJob currentJob) {
			var values 			= currentJob.jobValues;
			var targetMethod	= values [0].ToString ();
			var controlMethod	= values [1].ToString ();
			var value01Method	= values [2].ToString ();
			var freeLabor = CJobManager.GetFreeLabor ();
			if (freeLabor != null) {
				var position = value01Method == "thisPosition" ? owner.GetClosestPoint (freeLabor.GetPosition ()) : owner.GetPosition ();
				freeLabor.SetTargetPosition (position);
				freeLabor.SetTargetController (null);
			}
		}

		// GATHERING TO
		protected virtual void GatheringCommand(IJobOwner owner, CRemainJob currentJob) {
			var values 			= currentJob.jobValues;
			var targetMethod	= values [0].ToString ();
			var controlMethod	= values [1].ToString ();
			var value01Method	= values [2].ToString ();
			var freeLabor = CJobManager.GetFreeLabor ();
			if (freeLabor != null) {
				freeLabor.SetTargetPosition (owner.GetClosestPoint(freeLabor.GetPosition()));
				freeLabor.SetTargetController (owner.GetController ());
			}
		}

		// CREATE LABOR
		protected virtual void CreateLaborCommand(IJobOwner owner, CRemainJob currentJob) {
			var values 			= currentJob.jobValues;
			var targetMethod	= values [0].ToString ();
			var controlMethod	= values [1].ToString ();
			var needLabor 		= values [2].ToString ();
			var laborType		= values [3].ToString ();
			var value01Method	= values [4].ToString ();
			var value02Method	= values [5].ToString ();
			var value03Method	= values [6].ToString ();
			var ownerPosition = owner.GetPosition ();
			var characters = Resources.LoadAll<CCharacterController> ("Character");
			// TEST
			var random 		= (int)Time.time % characters.Length;
			var newLabor 	= GameObject.Instantiate (characters[random]);
			newLabor.SetTargetPosition (ownerPosition);
			newLabor.SetPosition (ownerPosition);
		}

		// LOVE LABOR
		protected virtual void LoveCommand(IJobOwner owner, CRemainJob currentJob) {
			owner.SetAnimation ("IsHit");
		}

		// MAKE TOOL
		protected virtual void MakeToolCommand(IJobOwner owner, CRemainJob currentJob) {
			this.WalkCommand (owner, currentJob);
		}

		// HATCH EGG
		protected virtual void HatchEggCommand(IJobOwner owner, CRemainJob currentJob) {
			this.WalkCommand (owner, currentJob);
		}

        #endregion

    }
    
}
