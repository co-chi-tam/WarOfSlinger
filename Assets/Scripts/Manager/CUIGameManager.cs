using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleSingleton;

namespace WarOfSlinger {
	public class CUIGameManager : CMonoSingleton<CUIGameManager> {

		[Header ("Shop")]
		[SerializeField]	protected CUIShopBuilding m_ShopBuilding;

		public void OpenShop(string shopName) {
			this.m_ShopBuilding.gameObject.SetActive (true);
		}

	}
}
