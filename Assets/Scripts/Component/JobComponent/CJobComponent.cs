using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
			// PASSIVE JOB
			this.m_JobMap.Add("GoldPerResident", 	this.GoldPerResident);
			this.m_JobMap.Add("CallRandomNPC", 		this.CallRandomNPC);
			this.m_JobMap.Add("ConsumeFood", 		this.ConsumeFood);
			// ACTIVE JOB
			this.m_JobMap.Add("WalkCommand",		this.WalkCommand);
			this.m_JobMap.Add("CoverCommand",		this.CoverCommand);
			this.m_JobMap.Add("GatheringCommand",	this.GatheringCommand);
			this.m_JobMap.Add("AttackCommand",		this.AttackCommand);
			this.m_JobMap.Add("DemolitionCommand",	this.DemolitionCommand);
			this.m_JobMap.Add("CreateLaborCommand",	this.CreateLaborCommand);
			this.m_JobMap.Add("LoveCommand",		this.LoveCommand);
			this.m_JobMap.Add("MakeToolCommand",	this.MakeToolCommand);
			this.m_JobMap.Add("ImproveToolCommand",	this.MakeToolCommand);
			this.m_JobMap.Add("HatchEggCommand",	this.HatchEggCommand);
			this.m_JobMap.Add("OpenShopCommand",	this.OpenShopCommand);
			this.m_JobMap.Add("OpenInventoryCommand",	this.OpenInventoryCommand);

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
			newJob.jobToolRequire	= value.jobToolRequire;
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
			if (buildingOwner != null) {
				var values = currentJob.jobValues;
				var claimedGold = int.Parse (values [0]);
				var perResident = int.Parse (values [1]);
				if (currentJob.OnJobCompleted != null) {
					currentJob.OnJobCompleted (owner, currentJob);
				}
				Debug.Log (claimedGold + " / " + perResident);
			}
        }

		// CALL RANDOM NPC
		protected virtual void CallRandomNPC(IJobOwner owner, CRemainJob currentJob) {
			var buildingOwner 	= owner as IBuildingJobOwner;
			if (buildingOwner != null) {
				var buildingCtrl = buildingOwner.GetController () as CBuildingController;
				var npcCtrls = buildingCtrl.GetNPCControllers ();
				var npcPoints = buildingCtrl.GetNPCPoints ();

				var random = UnityEngine.Random.Range (0, 999999);
				var randomNPC = random % (npcCtrls.Length + npcCtrls.Length);
				var randomPos = random % npcPoints.Length;

				for (int i = 0; i < npcCtrls.Length; i++) {
					var npc = npcCtrls [i];
					npc.gameObject.SetActive (false);
					npc.gameObject.SetActive (i == randomNPC);
					npc.SetPosition (npcPoints[randomPos].transform.position);
				}
			}
		}

		protected virtual void ConsumeFood (IJobOwner owner, CRemainJob currentJob) {
			var characterOwner 	= owner as ICharacterJobOwner;
			var resourceName	= "food";
			var resourceData 	= CVillageManager.GetResource (resourceName);
			if (resourceData != null) {
				var foodConsume = characterOwner.GetConsumeFood ();
				if (resourceData.currentAmount >= foodConsume) {
					var totalAmount = resourceData.currentAmount - foodConsume;
					CVillageManager.SetResource (resourceName, totalAmount);
					CGameManager.Instance.UpdateVillageData ();
				} else {
					owner.GetController ().Talk ("I am hungry !!");
					var values = currentJob.jobValues;
					var isHunger = Array.IndexOf (values, "hungry") != -1;
					if (isHunger) {
						var healthDecrease = 5;
						var currentHealth = characterOwner.GetCurrentHealth ();
						var totalHealth = currentHealth - healthDecrease;
						characterOwner.SetCurrentHealth (totalHealth);
					}
				}
			}
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
			var resourceName 	= "tool";
			var isNeedTool 		= currentJob.jobToolRequire > 0;
			if (isNeedTool) {
				var resourceData = CVillageManager.GetResource (resourceName);
				if (resourceData != null && resourceData.currentAmount >= currentJob.jobToolRequire) {
					var freeLabor = CJobManager.GetFreeLabor ();
					if (freeLabor != null) {
						freeLabor.SetTargetPosition (owner.GetClosestPoint (freeLabor.GetPosition ()));
						freeLabor.SetTargetController (owner.GetController ());
						currentJob.RegisterJobLabor (freeLabor);
						currentJob.OnJobCompleted -= HandleGatheringObject;
						currentJob.OnJobCompleted += HandleGatheringObject;
						var totalAmount = resourceData.currentAmount - currentJob.jobToolRequire;
						CVillageManager.SetResource (resourceName, totalAmount);
					} else {
						owner.GetController ().Talk ("NOT FREE LABOR.");
					}
				}
			} else {
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
		}

		// DEMOLITION 
		protected virtual void AttackCommand(IJobOwner owner, CRemainJob currentJob) {
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

		protected virtual void CoverCommand(IJobOwner owner, CRemainJob currentJob) {
			var freeLabor = CJobManager.GetFreeLabor ();
			if (freeLabor != null) {
				freeLabor.SetTargetPosition (owner.GetTargetPosition ());
				freeLabor.SetTargetController (owner.GetTargetController ());
				currentJob.jobOwner = owner.GetTargetController ();
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
				}
				currentJob.OnJobCompleted -= HandleCreateLabor;
				currentJob.OnJobCompleted += HandleCreateLabor;
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
			currentJob.OnJobCompleted -= HandleMakeTool;
			currentJob.OnJobCompleted += HandleMakeTool;
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

		// OPEN INVENTORY
		protected virtual void OpenInventoryCommand (IJobOwner owner, CRemainJob currentJob) {
			this.WalkCommand (owner, currentJob);
			currentJob.OnJobCompleted -= HandleOpenInventory;
			currentJob.OnJobCompleted += HandleOpenInventory;
		}

        #endregion

		#region Job methods

		protected virtual void HandleMakeTool (IJobOwner owner, CJobObjectData data) {
			var toolValue = data.jobValues;
			var resourceValue = toolValue.GroupBy ((x) => x)
										.ToDictionary(k => k.Key, v => v.Count());
			if (CVillageManager.IsEnoughResources (resourceValue)) {
				foreach (var resourceCost in resourceValue) {
					var resourceName 	= resourceCost.Key;
					var resourceData 	= CVillageManager.GetResource (resourceName);
					if (resourceData != null && resourceName != "tool") {
						var totalAmount = resourceData.currentAmount - resourceCost.Value;
						CVillageManager.SetResource (resourceName, totalAmount);
					}
				}
				var toolData 	= CVillageManager.GetResource ("tool");
				if (toolData != null && resourceValue.ContainsKey ("tool")) {
					var toolInscrease = resourceValue ["tool"];
					var totalAmount = toolData.currentAmount + toolInscrease;
					CVillageManager.SetResource ("tool", totalAmount);
				}
			} else {
				owner.GetController ().Talk ("NEED MORE RESOURCE.");
			}
			CGameManager.Instance.UpdateVillageData ();
		}

		protected virtual void HandleGatheringObject (IJobOwner owner, CJobObjectData data) {
			var gatherValues = data.jobValues;
			for (int i = 0; i < gatherValues.Length; i++) {
				var resourceName 	= gatherValues [i];
				var resourceData 	= CVillageManager.GetResource (resourceName);
				if (resourceData != null) {
					var totalAmount = resourceData.currentAmount + 1;
					CVillageManager.SetResource (resourceName, totalAmount);
				}
			}
			CGameManager.Instance.UpdateVillageData ();
		}

		protected virtual void HandleCreateLabor(IJobOwner owner, CJobObjectData data) {
			var ownerPosition 	= owner.GetPosition ();
			CGameManager.Instance.CreateResident (ownerPosition);
		}

		protected virtual void HandleOpenShop(IJobOwner owner, CJobObjectData data) {
			var jobValues = data.jobValues;
			for (int i = 0; i < jobValues.Length; i++) {
				CGameManager.Instance.OpenShop (jobValues[i]);
			}
		}

		protected virtual void HandleOpenInventory(IJobOwner owner, CJobObjectData data) {
			CGameManager.Instance.OpenInventory ();
		}

		#endregion

		#region Getter && Setter

		public int CountJobLabor () {
			var countLabor = 0;
			for (int i = 0; i < this.m_JobRemains.Count; i++) {
				if (this.m_JobRemains[i].jobType == (int) CJobObjectData.EJobType.ActiveJob) {
					countLabor += this.m_JobRemains[i].CountLabor();
				}
			}
			return countLabor;
		}

		#endregion

    }
    
}
