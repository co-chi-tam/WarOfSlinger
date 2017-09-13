using System;
using UnityEngine;

namespace WarOfSlinger {
	[Serializable]
	public class CShopItemData: CInventoryItemData {

		public CResourceData[] itemCost;

		public CShopItemData () : base()
		{
			
		}

	}
}
