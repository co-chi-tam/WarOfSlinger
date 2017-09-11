using System;
using UnityEngine;
using UnityEngine.UI;
using SimpleSingleton;

namespace WarOfSlinger {
	public class CUIGameManager : CMonoSingleton<CUIGameManager> {

		#region Fields

		[Header("Village resouce")]
		[Header("Wood")]
		[SerializeField]	protected Text m_WoodResourceText;
		[Header("Rock")]
		[SerializeField]	protected Text m_RockResourceText;
		[Header("Leaf")]
		[SerializeField]	protected Text m_LeafResourceText;
		[Header("Meat")]
		[SerializeField]	protected Text m_FoodResourceText;
		[Header("Tool")]
		[SerializeField]	protected Text m_ToolResourceText;

		[Header ("Shop")]
		[SerializeField]	protected CUIShopBuilding m_ShopBuilding;

		[Header("Building control")]
		[SerializeField]	protected GameObject m_BuildingControlPanel;

		#endregion

		#region Main methods

		public void OpenShop(string shopName, Action<CShopItemData> onSelectedItem) {
			switch (shopName) {
			case "building-shop":
				this.m_ShopBuilding.LoadShopData (onSelectedItem);
				this.m_ShopBuilding.gameObject.SetActive (true);
				break;
			}
		}

		public void CloseShop(string shopName) {
			switch (shopName) {
			case "building-shop":
				this.m_ShopBuilding.gameObject.SetActive (false);
				break;
			}
		}

		public void OpenBuildingControl() {
			this.m_BuildingControlPanel.SetActive (true);
		}

		public void CloseBuildingControl() {
			if (CGameManager.Instance.canChangeMode == false)
				return;
			this.m_BuildingControlPanel.SetActive (false);
			CGameManager.Instance.SetMode (CGameManager.EGameMode.PLAYING);
		}

		public virtual void SetUpResource() {
			var villageData = CVillageManager.GetData ();
			this.SetUpResource (villageData);
		}

		public virtual void SetUpResource(CVillageData villageData) {
			var villageResouce = villageData.villageResources;
			for (int i = 0; i < villageResouce.Length; i++) {
				var resourceData = villageResouce [i];
				var currentAmount = resourceData.currentAmount.ToString ();
				switch (resourceData.resouceName) {
				case "wood":
					this.m_WoodResourceText.text = currentAmount;
					break;
				case "rock":
					this.m_RockResourceText.text = currentAmount;
					break;
				case "leaf":
					this.m_LeafResourceText.text = currentAmount;
					break;
				case "food":
					this.m_FoodResourceText.text = currentAmount;
					break;
				case "tool":
					this.m_ToolResourceText.text = currentAmount;
					break;
				}
			}
		}

		#endregion

	}
}
