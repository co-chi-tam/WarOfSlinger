using System;
using UnityEngine;

namespace WarOfSlinger {
    [Serializable]
    public class CJobObjectData {

        public string jobName;
        public string jobDescription;
        public string[] jobValues;
        public float jobTimer;

        public CJobObjectData() {
            this.jobName    = string.Empty;
            this.jobDescription = string.Empty;
            // jobValues
            this.jobTimer   = 0f;
        }

    }
}
