using System;
using UnityEngine;

namespace WarOfSlinger {
    [Serializable]
    public class CResidenceData : CBuildingData {

        public int currentResident;
        public int maxResident;

        public CResidenceData() : base() {
            this.currentResident    = 0;
            this.maxResident        = 0;
        }
		
	}
}
