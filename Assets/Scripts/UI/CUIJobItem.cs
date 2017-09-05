using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarOfSlinger {
	public class CUIJobItem : MonoBehaviour {

		#region Fields

		[Header ("UI Job Item")]
		[SerializeField]	protected Button m_JobItemButton;
		[SerializeField]	protected Text m_JobItemText;
		[SerializeField]	protected Image m_JobItemImage;

		protected RectTransform m_RectTransform;

		#endregion

		#region Main methods

		public virtual void SetupItem(Vector2 pos, CJobObjectData data, Action<CJobObjectData> submit) {
			this.m_RectTransform = this.transform as RectTransform;
			this.m_RectTransform.anchoredPosition = pos;
			this.gameObject.SetActive (true);
			this.m_JobItemText.text = data.jobDisplayName;
			this.m_JobItemButton.onClick.RemoveAllListeners ();
			this.m_JobItemButton.onClick.AddListener (() => {
				if (submit != null) {
					submit(data);
				}
			});
		}

		#endregion

	}
}
