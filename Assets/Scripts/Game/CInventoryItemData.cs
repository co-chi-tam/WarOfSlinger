using System;
using UnityEngine;

namespace WarOfSlinger {
	[Serializable]
	public class CInventoryItemData {

		public string itemName;
		public string itemAvatar;
		public string itemSource;

		public CInventoryItemData ()
		{
			this.itemName 		= string.Empty;
			this.itemAvatar 	= string.Empty;
			this.itemSource 	= string.Empty;
		}

	}
}
