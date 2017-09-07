using System;
using UnityEngine;
using UnityEngine.UI;

namespace WarOfSlinger {
	public class CUIShopBuildingItem : MonoBehaviour {
			
		[Header("Item info")]
		[SerializeField]	protected Button m_ShopItemButton;
		[SerializeField]	protected Image m_ShopItemImage;
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
		[SerializeField]	protected GameObject m_MeatCostGo;
		[SerializeField]	protected Text m_MeatCostText;
		[Header("Tool")]
		[SerializeField]	protected GameObject m_ToolCostGo;
		[SerializeField]	protected Text m_ToolCostText;


	
	}
}
