using System;
using UnityEngine;

namespace WarOfSlinger {
	public class CVillageData {

		public string villageName;
		public int currentVillain;
		public int maxVillain;
		public CResourceData[] villageResources;

		public CVillageData ()
		{
			this.villageName 	= string.Empty;
			this.currentVillain = 0;
			this.maxVillain 	= 99;
		}
		
	}
}
