using System;
using UnityEngine;
using UnityEngine.UI;

namespace WarOfSlinger {
	public class CUIShopBuilding : MonoBehaviour {

		[Header ("Building item")]
		[SerializeField]	protected GameObject m_ShopBuildingRoot;
		[SerializeField]	protected CUIShopBuildingItem m_ShopBuildingPrefab;
		
	}
}
