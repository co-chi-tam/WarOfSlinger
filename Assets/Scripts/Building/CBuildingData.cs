using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
    [Serializable]
    public class CBuildingData : CDamageableObjectData {
		
		public int currentResident;
		public int maxResident;
		public float percentConstruction;
		public CObjectData[] NPCDatas;

        public CBuildingData() : base () {
			this.currentResident    = 0;
			this.maxResident        = 0;
			this.percentConstruction = 0f; // MAX 100f
        }

    }
}
