using System;
using UnityEngine;
using UnityEngine.UI;
using ObjectPool;

namespace WarOfSlinger {
	public class CUIInfo : MonoBehaviour {

		[Header("Info")]
		[SerializeField]	protected GameObject m_InfoRoot;
		[SerializeField]	protected CUIInfoItem m_InfoItemPrefabs;

		protected virtual void Awake() {
			this.m_InfoItemPrefabs.gameObject.SetActive (false);
		}

		public void LoadInfoData(CDamageableObjectData data, CObjectController controller) {
			var currentHealthValue = (float)data.currentHealth / data.maxHealth;
			this.m_InfoItemPrefabs.SetUpItem (controller.transform, data.objectName, currentHealthValue);
			this.m_InfoItemPrefabs.gameObject.SetActive (true);
		}

	}
}
