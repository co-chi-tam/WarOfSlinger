using System;
using UnityEngine;

namespace WarOfSlinger {
    [Serializable]
    public class CJobObjectData {

        public string jobDisplayName;
		public string jobAvatar;
		public string jobExcute;
        public string jobDescription;
		public int jobLaborRequire;
		public int jobToolRequire;
        public string[] jobValues;
		public float jobTimer;
		public float jobCountdownTimer;
		public int jobType;

		public enum EJobType: int {
			PassiveJob = 0,
			ActiveJob = 1
		}

        public CJobObjectData() {
            this.jobDisplayName    	= string.Empty;
			this.jobAvatar 			= string.Empty;
			this.jobExcute 			= string.Empty;
            this.jobDescription 	= string.Empty;
			this.jobLaborRequire	= 1;
			this.jobToolRequire		= 0;
            // jobValues
			this.jobTimer   		= 0f;
			this.jobCountdownTimer	= 0f;
			this.jobType			= 0;
        }

    }
}
