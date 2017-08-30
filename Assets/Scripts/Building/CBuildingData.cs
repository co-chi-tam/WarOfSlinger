using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
    [Serializable]
    public class CBuildingData : CDamageableObjectData {

        public CJobObjectData[] buildingJobs;

        public CBuildingData() : base () {
            // buildingJobs
        }

    }
}
