using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarOfSlinger {
	public class CUIInventory : MonoBehaviour {

		[Header("Inventory")]
		[SerializeField]	protected Text m_InventoryNameText;
		[Header ("Inventory item")]
		[SerializeField]	protected GameObject m_InventoryRoot;
		[SerializeField]	protected CUIInventoryItem m_InventoryItemPrefab;

		protected List<CUIInventoryItem> m_SaveInventories = new List<CUIInventoryItem> ();

		public void LoadInventoryData(CInventoryItemData[] inventoryItems, Action<CInventoryItemData> onSelectedItem) {
			this.m_InventoryNameText.text = "Inventory";
			for (int i = 0; i < inventoryItems.Length; i++) {
				CUIInventoryItem item = null;
				if (i >= this.m_SaveInventories.Count) {
					item = Instantiate (this.m_InventoryItemPrefab);
					this.m_SaveInventories.Add (item);
				} else {
					item = this.m_SaveInventories [i];
				}
				item.transform.SetParent (this.m_InventoryRoot.transform);
				var itemData = inventoryItems [i];
				if (itemData == null || string.IsNullOrEmpty (itemData.itemName)) {
					item.gameObject.SetActive (false);
					continue;
				}
				item.SetUpItem (itemData, onSelectedItem);
				item.gameObject.SetActive (true);
			}
			this.m_InventoryItemPrefab.gameObject.SetActive (false);
		}
		
	}
}
