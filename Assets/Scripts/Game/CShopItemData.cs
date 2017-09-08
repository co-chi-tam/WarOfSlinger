using System;
using UnityEngine;

namespace WarOfSlinger {
	[Serializable]
	public class CShopItemData {

		public string itemName;
		public string itemAvatar;
		public string itemCommand;
		public CResourceData[] itemCost;

		public CShopItemData ()
		{
			this.itemName 		= string.Empty;
			this.itemAvatar 	= string.Empty;
			this.itemCommand 	= string.Empty;
		}

	}
}
