using System;
using UnityEngine;

namespace WarOfSlinger {
	[Serializable]
	public class CShopData {

		public string shopName;
		public CShopItemData[] shopItems;

		public CShopData ()
		{
			this.shopName = string.Empty;
		}
		
	}
}
