﻿using System;
using UnityEngine;

namespace WarOfSlinger {
	[Serializable]	
	public class CVillageData {

		public string villageName;
		public int currentPopulation;
		public int maxPopulation;
		public CResourceData[] villageResources;
		public CBuildingData[] villageBuildings;
		public CCharacterData[] villageCharacters;
		public CObjectData[] villageNPCs;
		public CDamageableObjectData[] villageObjects;

		public CVillageData ()
		{
			this.villageName 	= string.Empty;
			this.currentPopulation = 0;
			this.maxPopulation 	= 99;
		}
		
	}
}
