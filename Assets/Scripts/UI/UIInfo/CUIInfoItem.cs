using System;
using UnityEngine;
using UnityEngine.UI;

namespace WarOfSlinger {
	public class CUIInfoItem : MonoBehaviour {

		[Header("Info item")]
		[SerializeField]	protected Text m_InfoNameText;
		[SerializeField]	protected Image m_InfoHealthBarImage;

		protected Transform m_FollowTransform;
		protected Transform m_Transform;

		protected virtual void Awake() {
			this.m_Transform = this.transform;
		}

		protected virtual void LateUpdate() {
			if (this.m_FollowTransform == null)
				return;
			var screenPosition = Camera.main.WorldToScreenPoint (this.m_FollowTransform.position);
			this.m_Transform.position = screenPosition;
		}

		public void SetUpItem (Transform followTransform, string name, float healthValue) {
			this.m_InfoNameText.text = name;
			this.m_InfoHealthBarImage.fillAmount = healthValue;
			this.m_FollowTransform = followTransform;
		}
		
	}
}
