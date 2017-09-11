using System;
using UnityEngine;

namespace WarOfSlinger {
    [Serializable]
	public class CObjectData: CSaveObjectData {

        public string objectId;
        public string objectName;
        public string objectModel;
        public string objectDescription;
        public string objectType;
		public int objectLevel;
		public CJobObjectData[] objectJobs;

		public CObjectData(): base() {
            this.objectId       = Guid.NewGuid().ToString();
            this.objectName     = string.Empty;
            this.objectModel    = string.Empty;
            this.objectDescription = string.Empty;
			this.objectType 	= string.Empty;
            this.objectLevel    = 0;
        }

    }
}

