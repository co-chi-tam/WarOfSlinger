using System;
using UnityEngine;

namespace WarOfSlinger {
	[Serializable]
	public class CShopItemData: CInventoryItemData {

		public int currentAmount;
		public int maxAmount;
		public CResourceData[] itemCost;

		public CShopItemData () : base()
		{
			this.currentAmount 	= 0;
			this.maxAmount 		= 0;
		}

	}
}
