using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarOfSlinger {
	public class CUIJobItem : MonoBehaviour {

		#region Fields

		[Header ("UI Job Item")]
		[SerializeField]	protected Button m_SubmitJobButton;
		[SerializeField]	protected Button m_CloseJobButton;
		[SerializeField]	protected Text m_JobNameText;
		[SerializeField]	protected GameObject m_JobLaborRequireGo;
		[SerializeField]	protected Text m_JobLaborRequireText;
		[SerializeField]	protected Image m_JobItemImage;

		protected RectTransform m_RectTransform;

		#endregion

		#region Main methods

		public virtual void SetupItem(Vector2 pos, CJobObjectData data, Action<CJobObjectData> submit, Action<CJobObjectData> close) {
			this.m_RectTransform = this.transform as RectTransform;
			this.m_RectTransform.anchoredPosition = pos;
			this.gameObject.SetActive (true);
			this.m_JobNameText.text = data.jobDisplayName;
			this.m_JobLaborRequireGo.SetActive (data.jobLaborRequire > 1);
			this.m_JobLaborRequireText.text = data.jobLaborRequire.ToString ();
			this.m_SubmitJobButton.onClick.RemoveAllListeners ();
			this.m_SubmitJobButton.onClick.AddListener (() => {
				if (submit != null) {
					submit(data);
				}
			});
			this.m_CloseJobButton.onClick.RemoveAllListeners ();
			this.m_CloseJobButton.onClick.AddListener (() => {
				if (close != null) {
					close(data);
				}
			});
		}

		#endregion

	}
}
