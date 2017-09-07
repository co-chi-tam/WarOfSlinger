using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarOfSlinger {
	public class CUITalkItem : MonoBehaviour {

		[SerializeField]	protected Text m_TalkItemText;

		protected Transform m_FollowTransform;
		protected Transform m_Transform;
		protected float m_ShowTime = 0f;
		protected CUITalk m_ObjectPool;

		protected virtual void Awake() {
			this.m_Transform = this.transform;
		}

		protected virtual void LateUpdate() {
			if (this.m_FollowTransform == null)
				return;
			if (this.m_ShowTime > 0f) {
				this.m_ShowTime -= Time.deltaTime;
				var screenPosition = Camera.main.WorldToScreenPoint (this.m_FollowTransform.position);
				this.m_Transform.position = screenPosition;
			} else {
				this.m_FollowTransform = null;
				this.gameObject.SetActive (false);
				this.m_ObjectPool.ReturnItem (this);
			}
		}

		public virtual void SetUpItem(Transform followTransform, string value, CUITalk objPool) {
			this.m_FollowTransform 	= followTransform;
			this.m_TalkItemText.text = value;
			this.m_ObjectPool 		= objPool;
			this.m_ShowTime 		= 3f;
			this.gameObject.SetActive (true);
		}

	}
}
