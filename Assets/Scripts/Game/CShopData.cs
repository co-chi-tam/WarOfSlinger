using System;
using UnityEngine;

namespace WarOfSlinger {
	[Serializable]
	public class CShopData {

		public string shopName;
		public string shopDisplayName;
		public CShopItemData[] shopItems;

		public CShopData ()
		{
			this.shopName 			= string.Empty;
			this.shopDisplayName 	= string.Empty;
		}
		
	}
}
