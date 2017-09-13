using System;
using UnityEngine;
using UnityEngine.UI;

namespace WarOfSlinger {
	public class CUIShopItem : CUIInventoryItem {
			
		[Header("Wood")]
		[SerializeField]	protected GameObject m_WoodCostGo;
		[SerializeField]	protected Text m_WoodCostText;
		[Header("Rock")]
		[SerializeField]	protected GameObject m_RockCostGo;
		[SerializeField]	protected Text m_RockCostText;
		[Header("Leaf")]
		[SerializeField]	protected GameObject m_LeafCostGo;
		[SerializeField]	protected Text m_LeafCostText;
		[Header("Meat")]
		[SerializeField]	protected GameObject m_FoodCostGo;
		[SerializeField]	protected Text m_FoodCostText;
		[Header("Tool")]
		[SerializeField]	protected GameObject m_ToolCostGo;
		[SerializeField]	protected Text m_ToolCostText;

		public void SetUpItem(CShopItemData value, Action<CShopItemData> onSelectedItem) {
			this.m_ShopItemImage.sprite = CUtil.FindResourceSprite (value.itemAvatar);
			this.m_ShopItemNameText.text = value.itemName;
			var itemCost = value.itemCost;
			for (int i = 0; i < itemCost.Length; i++) {
				var resourceData = itemCost [i];
				var currentAmount = resourceData.currentAmount;
				switch (resourceData.resouceName) {
				case "wood":
					this.m_WoodCostGo.SetActive (currentAmount != 0);
					this.m_WoodCostText.text = currentAmount.ToString ();
					break;
				case "rock":
					this.m_RockCostGo.SetActive (currentAmount != 0);
					this.m_RockCostText.text = currentAmount.ToString ();
					break;
				case "leaf":
					this.m_LeafCostGo.SetActive (currentAmount != 0);
					this.m_LeafCostText.text = currentAmount.ToString ();
					break;
				case "food":
					this.m_FoodCostGo.SetActive (currentAmount != 0);
					this.m_FoodCostText.text = currentAmount.ToString ();
					break;
				case "tool":
					this.m_ToolCostGo.SetActive (currentAmount != 0);
					this.m_ToolCostText.text = currentAmount.ToString ();
					break;
				}
			}
			this.m_ShopItemButton.onClick.RemoveAllListeners ();
			this.m_ShopItemButton.onClick.AddListener (() => {
				if (onSelectedItem != null) {
					onSelectedItem(value);
				}	
			});
		}
	
	}
}
