using System;
using UnityEngine;

namespace WarOfSlinger {
	[Serializable]
	public class CResourceData {

		public string resouceName;
		public string resouceAvatar;
		public int currentAmount;
		public int maxAmount;

		public CResourceData ()
		{
			this.resouceName 	= string.Empty;
			this.resouceAvatar 	= string.Empty;
			this.currentAmount 	= 0;
			this.maxAmount 		= 999999;
		}
		
	}
}
