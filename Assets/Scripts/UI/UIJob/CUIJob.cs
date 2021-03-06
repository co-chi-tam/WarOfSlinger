﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarOfSlinger {
	public class CUIJob : MonoBehaviour {

		#region Fields

		[Header ("Job root")]
		[SerializeField]	protected GameObject m_Root;

		[Header("UI Job Item")]
		[SerializeField]	protected float m_Radius = 75f;
		[SerializeField]	protected CUIJobItem[] m_JobItems;

		protected Transform m_OwnerTransform;
		protected Transform m_Transform;

		#endregion

		#region Implementation Monobehaviour

		protected virtual void Awake() {
			this.m_Transform = transform;
			this.gameObject.SetActive (false);
		}

		protected virtual void LateUpdate() {
			if (m_OwnerTransform == null)
				return;
			var screenPosition = Camera.main.WorldToScreenPoint (this.m_OwnerTransform.position);
			this.m_Transform.position = screenPosition;
		}

		#endregion

		#region Main methods

		public virtual void ShowJobs (Transform owner, Action<CJobObjectData> jobSelected, Action<CJobObjectData> jobClosed, params CJobObjectData[] jobs) {
			var numberSegments = 6;
			var sequence = (Mathf.PI * 2f) / numberSegments;
			var theta = 0f;
			for (int i = 0; i < numberSegments; i++) {
				var x = Mathf.Sin (theta) * this.m_Radius;
				var y = Mathf.Cos (theta) * this.m_Radius;
				var tmpJob = this.m_JobItems [i];
				tmpJob.gameObject.SetActive (false);
				if (i > jobs.Length - 1) {
					continue;
				}
				var data = jobs [i];
				if (data.jobType == (int)CJobObjectData.EJobType.PassiveJob)
					continue;
				tmpJob.gameObject.SetActive (true);
				tmpJob.SetupItem (new Vector2 (x, y), data, (jobData) => {
					// SELECTED
					if (jobSelected != null) {
						jobSelected (jobData);
					}
					this.m_Root.SetActive (false);
				}, (jobData) => {
					// CLOSED
					if (jobClosed != null) {
						jobClosed (jobData);
					}
					this.m_Root.SetActive (false);
				});
				theta += sequence;
			}
			this.m_Root.SetActive (true);
			this.m_OwnerTransform = owner;
		}

		#endregion

	}
}
