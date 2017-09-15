using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
	[Serializable]	
	public class CVillageData {

		public string villageName;
		public int currentPopulation;
		public int maxPopulation;
		public float villageTimer;
		public CRespawnObjectData[] villageRespawnObjects;
		public CResourceData[] villageResources;
		public CBuildingData[] villageBuildings;
		public CCharacterData[] villageCharacters;
		public CDamageableObjectData[] villageObjects;
		public Dictionary<string, CShopData> villageShops;
		public CInventoryItemData[] villageInventories;

		public CVillageData ()
		{
			this.villageName 		= string.Empty;
			this.currentPopulation 	= 0;
			this.maxPopulation 		= 99;
			this.villageTimer 		= 0f;
		}
		
	}
}
