using System;
using UnityEngine;

namespace WarOfSlinger {
    [Serializable]
    public class CJobObjectData {

        public string jobDisplayName;
		public string jobAvatar;
		public string jobExcute;
        public string jobDescription;
        public string[] jobValues;
		public float jobTimer;
		public float jobCountdownTimer;
		public int jobType;
		public IJobOwner jobOwner;

		public enum EJobType: int {
			PassiveJob = 0,
			ActiveJob = 1
		}

        public CJobObjectData() {
            this.jobDisplayName    	= string.Empty;
			this.jobAvatar 			= string.Empty;
			this.jobExcute 			= string.Empty;
            this.jobDescription 	= string.Empty;
            // jobValues
			this.jobTimer   		= 0f;
			this.jobCountdownTimer	= 0f;
			this.jobType			= 0;
        }

    }
}
