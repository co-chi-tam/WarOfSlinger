using System;
using UnityEngine;
using UnityEngine.UI;

namespace WarOfSlinger {
	public class CUIInventoryItem : MonoBehaviour {

		[Header("Item info")]
		[SerializeField]	protected Button m_ShopItemButton;
		[SerializeField]	protected Image m_ShopItemImage;
		[SerializeField]	protected Text m_ShopItemNameText;

		public void SetUpItem(CInventoryItemData value, Action<CInventoryItemData> onSelectedItem) {
			this.m_ShopItemImage.sprite = CUtil.FindResourceSprite (value.itemAvatar);
			this.m_ShopItemNameText.text = value.itemName;
			this.m_ShopItemButton.onClick.RemoveAllListeners ();
			this.m_ShopItemButton.onClick.AddListener (() => {
				if (onSelectedItem != null) {
					onSelectedItem(value);
				}	
			});
		}

	}
}
