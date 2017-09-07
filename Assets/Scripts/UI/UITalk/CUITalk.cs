using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ObjectPool;

namespace WarOfSlinger {
	public class CUITalk : MonoBehaviour {

		[SerializeField]	protected GameObject m_TalkRoot;
		[SerializeField]	protected CUITalkItem m_TalkItemPrefab;

		protected ObjectPool<CUITalkItem> m_TalkItemPool;

		protected virtual void Awake() {
			this.m_TalkItemPool = new ObjectPool<CUITalkItem> ();
			this.m_TalkItemPrefab.gameObject.SetActive (false);
		}

		public virtual void ShowTalkItem(Transform parent, string value) {
			CUITalkItem item = null;
			if (this.m_TalkItemPool.Get (ref item)) {
				item.SetUpItem (parent, value, this);
			} else {
				var newItem = Instantiate (this.m_TalkItemPrefab);
				newItem.transform.SetParent (this.m_TalkRoot.transform);
				this.m_TalkItemPool.Create (newItem);
				item = this.m_TalkItemPool.Get ();
				item.SetUpItem (parent, value, this);
			}
		}

		public virtual void ReturnItem (CUITalkItem item) {
			this.m_TalkItemPool.Set (item);
		}

	}
}
