using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
	public class CRemainJob: CJobObjectData {

		public Action<IJobOwner, CJobObjectData> OnJobCompleted;
		public Action<float> OnJobProcessing;
		public Action<string> OnJobFailed;

		public IJobOwner jobOwner;
		public List<IJobLabor> jobLaborList;

		private Dictionary<string, Func<IJobLabor, bool>> m_JobCompleteCondition;

		public CRemainJob (): base () {
			this.jobLaborList = new List<IJobLabor> ();
			this.m_JobCompleteCondition = new Dictionary<string, Func<IJobLabor, bool>> ();
			this.m_JobCompleteCondition.Add("LoveCommand",			this.AutoCompleteCommand);
			this.m_JobCompleteCondition.Add("TalkCommand",			this.AutoCompleteCommand);
			this.m_JobCompleteCondition.Add("OpenShopCommand",		this.WalkCompleteCommand);
			this.m_JobCompleteCondition.Add("OpenInventoryCommand",	this.WalkCompleteCommand);
			this.m_JobCompleteCondition.Add("WalkCommand",			this.WalkCompleteCommand);
			this.m_JobCompleteCondition.Add("GatheringCommand",		this.InactiveCompleteCommand);
			this.m_JobCompleteCondition.Add("CoverCommand",			this.InactiveCompleteCommand);
			this.m_JobCompleteCondition.Add("AttackCommand",		this.InactiveCompleteCommand);
			this.m_JobCompleteCondition.Add("DemolitionCommand",	this.InactiveCompleteCommand);
			this.m_JobCompleteCondition.Add("CreateLaborCommand",	this.CreateLaborCompleteCommand);
			this.m_JobCompleteCondition.Add("MakeToolCommand",		this.MakeToolCompleteCommand);
			this.m_JobCompleteCondition.Add("ImproveToolCommand",	this.MakeToolCompleteCommand);
			this.m_JobCompleteCondition.Add("HatchEggCommand",		this.HatchEggCompleteCommand);
		}

		public virtual void UpdateRemainJob(float dt) {
			if (this.jobLaborList.Count >= this.jobLaborRequire) {
				// CHECK JOB COMPLETED
				var isConditionCorrect = true;
				for (int i = 0; i < this.jobLaborList.Count; i++) {
					var labor = this.jobLaborList [i];
					isConditionCorrect &= (this.m_JobCompleteCondition [this.jobExcute] (labor) && labor.IsActive());
				}
				// RELEASE JOB LABOR
				if (isConditionCorrect) {
					if (this.OnJobCompleted != null) {
						this.OnJobCompleted (this.jobOwner, this);
					}
					this.ClearJobLabor ();
				} 
			}
		}

		public virtual bool IsFullLabor() {
			return this.jobLaborList.Count >= this.jobLaborRequire;
		}

		public virtual int CountLabor () {
			return this.jobLaborList.Count;
		}

		// REGISTER JOB LABOR
		public virtual void RegisterJobLabor(IJobLabor labor) {
			if (this.jobLaborList.Contains (labor) == false) {
				this.jobLaborList.Add (labor);
			}
		}

		// UNREGISTER JOB LABOR
		public virtual void UnRegisterJobLabor(IJobLabor labor) {
			if (this.jobLaborList.Contains (labor)) {
				this.jobLaborList.Remove (labor);
				CJobManager.ReturnFreeLabor (labor);
			}
		}

		// CLEAR JOB LABOR
		public virtual void ClearJobLabor() {
			for (int i = 0; i < this.jobLaborList.Count; i++) {
				var labor = this.jobLaborList [i];
				labor.ClearJobLabor ();
				this.UnRegisterJobLabor (labor);
				i--;
			}
		}

		protected virtual bool AutoCompleteCommand(IJobLabor labor) {
			return true;
		}

		// WALL COMPLETED
		protected virtual bool WalkCompleteCommand(IJobLabor labor) {
			var laborPosition = labor.GetPosition ();
			var targetPosition = labor.GetTargetPosition ();
			var direction = targetPosition - laborPosition;
			return direction.sqrMagnitude <= 0.001f;
		}

		protected virtual bool InactiveCompleteCommand(IJobLabor labor) {
			if (this.jobOwner == null)
				return true;
			return this.jobOwner.GetActive() == false;
		}

		protected virtual bool CreateLaborCompleteCommand(IJobLabor labor) {
			return this.WalkCompleteCommand (labor);
		}

		protected virtual bool MakeToolCompleteCommand(IJobLabor labor) {
			return this.WalkCompleteCommand (labor);
		}

		protected virtual bool HatchEggCompleteCommand(IJobLabor labor) {
			return this.WalkCompleteCommand (labor);
		}

	}
}
