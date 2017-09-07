using System;
using UnityEngine;

namespace WarOfSlinger {
	[Serializable]
	public class CShopItemData {

		public string itemName;
		public CResourceData[] itemCost;

		public CShopItemData ()
		{
			this.itemName = string.Empty;
		}

	}
}
