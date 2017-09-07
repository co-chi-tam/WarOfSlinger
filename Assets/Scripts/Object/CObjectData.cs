using System;
using UnityEngine;

namespace WarOfSlinger {
    [Serializable]
    public class CObjectData {

        public string objectId;
        public string objectName;
        public string objectModel;
        public string objectDescription;
        public string[] objectTypes;
		public int objectLevel;
		public CJobObjectData[] objectJobs;

        public CObjectData() {
            this.objectId       = Guid.NewGuid().ToString();
            this.objectName     = string.Empty;
            this.objectModel    = string.Empty;
            this.objectDescription = string.Empty;
            this.objectLevel    = 0;
        }

    }
}

