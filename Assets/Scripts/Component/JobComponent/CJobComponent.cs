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
			this.m_JobMap.Add("DemolitionCommand",	this.DemolitionCommand);
			this.m_JobMap.Add("CreateLaborCommand",	this.CreateLaborCommand);
			this.m_JobMap.Add("LoveCommand",		this.LoveCommand);
			this.m_JobMap.Add("MakeToolCommand",	this.MakeToolCommand);
			this.m_JobMap.Add("HatchEggCommand",	this.HatchEggCommand);
			this.m_JobMap.Add("OpenShopCommand",	this.OpenShopCommand);

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
		public virtual void RegisterJobs(IJobOwner owner, CJobObjectData value, 
											Action<IJobOwner, CJobObjectData> onCompleted, 
											Action<float> onProcessing, 
											Action<string> onFailed) {
            // NEW JOB
			var newJob              = new CRemainJob();
            newJob.jobDisplayName	= value.jobDisplayName;
			newJob.jobAvatar 		= value.jobAvatar;
			newJob.jobExcute 		= value.jobExcute;
            newJob.jobDescription   = value.jobDescription;
			newJob.jobLaborRequire	= value.jobLaborRequire;
            newJob.jobValues        = new string[value.jobValues.Length];
            Array.Copy(value.jobValues, newJob.jobValues, value.jobValues.Length);
            newJob.jobTimer         = value.jobTimer;
			newJob.jobCountdownTimer = value.jobTimer;
			newJob.jobType 			= value.jobType;
            newJob.jobOwner         = owner;
			newJob.OnJobCompleted 	= onCompleted;
			newJob.OnJobProcessing 	= onProcessing;
			newJob.OnJobFailed 		= onFailed;
            // ADD JOB
            this.m_JobRemains.Add(newJob);
        }

        // UPDATE JOB TIMER
        public virtual void UpdateJobs(float dt) {
            for (int i = 0; i < this.m_JobRemains.Count; i++) {
                // CURRENT JOB
                var currentJob = this.m_JobRemains[i];
				currentJob.UpdateRemainJob (dt);
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
						this.ExcutePassiveJob(currentJob);
					} 
                }
            }
        }

        // UNREGISTER JOB
        public virtual void UnregisterJob(CJobObjectData value) {
            for (int i = 0; i < this.m_JobRemains.Count; i++) {
                var currentJob = this.m_JobRemains[i];
				if (currentJob.jobExcute == value.jobExcute) {
                    this.m_JobRemains.Remove(currentJob);
                    i--;
                }
            }
        }

        // EXCUTE JOB
		public virtual void ExcutePassiveJob(CRemainJob currentJob) {
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

		// CLEAR JOB
		public virtual void ClearJob(IJobOwner owner, string jobName) {
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
			// UPDATE JOB TIMER
			currentJob.jobCountdownTimer = currentJob.jobTimer;
			// CLEAR JOB
			currentJob.ClearJobLabor ();
		}

		#endregion

		#region Passive job

        // GOLD PER RESIDENT JOB
		protected virtual void GoldPerResident(IJobOwner owner, CRemainJob currentJob) {
			var buildingOwner 	= owner as IBuildingJobOwner;
			var values 			= currentJob.jobValues;
            var claimedGold 	= int.Parse (values[0]);
            var perResident 	= int.Parse (values[1]);
			if (currentJob.OnJobCompleted != null) {
				currentJob.OnJobCompleted (owner, currentJob);
			}
			Debug.Log (claimedGold + " / " + perResident);
        }

		#endregion

		#region Active job

		// WALK TO
		protected virtual void WalkCommand(IJobOwner owner, CRemainJob currentJob) {
			var freeLabor = CJobManager.GetFreeLabor ();
			if (freeLabor != null) {
				freeLabor.SetTargetPosition (owner.GetClosestPoint (freeLabor.GetPosition ()));
				currentJob.RegisterJobLabor (freeLabor);
			}
		}

		// GATHERING TO
		protected virtual void GatheringCommand(IJobOwner owner, CRemainJob currentJob) {
			var freeLabor = CJobManager.GetFreeLabor ();
			if (freeLabor != null) {
				freeLabor.SetTargetPosition (owner.GetClosestPoint (freeLabor.GetPosition ()));
				freeLabor.SetTargetController (owner.GetController ());
				currentJob.RegisterJobLabor (freeLabor);
				currentJob.OnJobCompleted -= HandleGatheringObject;
				currentJob.OnJobCompleted += HandleGatheringObject;
			} else {
				owner.GetController ().Talk ("NOT FREE LABOR.");
			}
		}

		// DEMOLITION 
		protected virtual void DemolitionCommand(IJobOwner owner, CRemainJob currentJob) {
			var freeLabor = CJobManager.GetFreeLabor ();
			if (freeLabor != null) {
				freeLabor.SetTargetPosition (owner.GetClosestPoint (freeLabor.GetPosition ()));
				freeLabor.SetTargetController (owner.GetController ());
				currentJob.RegisterJobLabor (freeLabor);
			} else {
				owner.GetController ().Talk ("NOT FREE LABOR.");
			}
		}

		// CREATE LABOR
		protected virtual void CreateLaborCommand(IJobOwner owner, CRemainJob currentJob) {
			var currentPopulation = CVillageManager.GetCurrentPopulation ();
			var maxPopulation = CVillageManager.GetMaxPopulation ();
			if (currentPopulation + 1 > maxPopulation) {
				if (currentJob.OnJobFailed != null) {
					currentJob.OnJobFailed ("MAX POPULATION");
				}
				owner.GetController ().Talk ("MAX POPULATION.");
			} else {
				this.WalkCommand (owner, currentJob);
				if (currentJob.IsFullLabor () == false) {
					owner.GetController ().Talk ("NEED MORE LABOR.");
					currentJob.OnJobCompleted -= HandleCreateLabor;
					currentJob.OnJobCompleted += HandleCreateLabor;
				}
			}
		}

		// LOVE LABOR
		protected virtual void LoveCommand(IJobOwner owner, CRemainJob currentJob) {
			var values = currentJob.jobValues;
			owner.SetAnimation ("IsHit");
			owner.GetController ().Talk (values[UnityEngine.Random.Range (0, values.Length)]);
		}

		// MAKE TOOL
		protected virtual void MakeToolCommand(IJobOwner owner, CRemainJob currentJob) {
			this.WalkCommand (owner, currentJob);
		}

		// HATCH EGG
		protected virtual void HatchEggCommand(IJobOwner owner, CRemainJob currentJob) {
			this.WalkCommand (owner, currentJob);
		}

		// OPEN SHOP
		protected virtual void OpenShopCommand (IJobOwner owner, CRemainJob currentJob) {
			this.WalkCommand (owner, currentJob);
			currentJob.OnJobCompleted -= HandleOpenShop;
			currentJob.OnJobCompleted += HandleOpenShop;
		}

        #endregion

		#region Job methods

		protected virtual void HandleGatheringObject (IJobOwner owner, CJobObjectData data) {
			var gatherValues = data.jobValues;
			for (int i = 0; i < gatherValues.Length; i++) {
				var resourceName 	= gatherValues [i];
				var resourceData 	= CVillageManager.GetResource (resourceName);
				var totalAmount 	= resourceData.currentAmount + 1;
				CVillageManager.SetResource (resourceName, totalAmount);
			}
			CGameManager.Instance.UpdateVillageData ();
		}

		protected virtual void HandleCreateLabor(IJobOwner owner, CJobObjectData data) {
			var ownerPosition 	= owner.GetPosition ();
			var characters 		= Resources.LoadAll<CCharacterController> ("Character/Prefabs");
			var characterDatas 	= Resources.LoadAll<TextAsset>("Character/Data");
			// TEST RANDOM
			var random 		= (int)Time.time % characters.Length;
			var newLabor 	= GameObject.Instantiate (characters[random]);
			var newData 	= TinyJSON.JSON.Load (characterDatas[0].text).Make<CCharacterData>();
			newLabor.SetTargetPosition (ownerPosition);
			newLabor.SetPosition (ownerPosition);
			newLabor.SetData (newData);
			newLabor.Init ();
			var currentPopulation = CVillageManager.GetCurrentPopulation ();
			CVillageManager.SetCurrentPopulation (currentPopulation + 1);
		}

		protected virtual void HandleOpenShop(IJobOwner owner, CJobObjectData data) {
			var jobValues = data.jobValues;
			for (int i = 0; i < jobValues.Length; i++) {
				CGameManager.Instance.OpenShop (jobValues[i]);
			}
		}

		#endregion

    }
    
}
