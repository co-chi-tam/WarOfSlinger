using System;
using UnityEngine;
using UnityEngine.UI;

namespace WarOfSlinger {
	public class CUIShopBuilding : MonoBehaviour {

		[Header("Building data")]
		[SerializeField]	protected CShopData m_ShopData;
		[Header("Shop")]
		[SerializeField]	protected Text m_ShopNameText;
		[Header ("Building item")]
		[SerializeField]	protected GameObject m_ShopBuildingRoot;
		[SerializeField]	protected CUIShopItem m_ShopBuildingPrefab;

		protected CUIShopItem[] m_ShopItemList;
		protected bool m_DataLoaded = false;

		public void LoadShopData(CShopData shopData, Action<CShopItemData> onSelectedItem) {
			if (this.m_DataLoaded == true)
				return;
			this.m_ShopData = shopData; 
			this.m_ShopNameText.text = this.m_ShopData.shopDisplayName;
			var shopItems = this.m_ShopData.shopItems;
			for (int i = 0; i < shopItems.Length; i++) {
				var item = Instantiate (this.m_ShopBuildingPrefab);
				var itemData = shopItems [i];
				item.SetUpItem (itemData, onSelectedItem);
				item.transform.SetParent (this.m_ShopBuildingRoot.transform);
				item.gameObject.SetActive (true);
			}
			this.m_ShopBuildingPrefab.gameObject.SetActive (false);
			this.m_DataLoaded = true;
		}

		public void SaveShop() {
			var jsonSaved = TinyJSON.JSON.Dump (this.m_ShopData);
			Debug.Log (jsonSaved);
		}
		
	}
}
