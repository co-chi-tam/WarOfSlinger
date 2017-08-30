using System;
using UnityEngine;

namespace WarOfSlinger {
    [Serializable]
    public class CDamageableObjectData : CObjectData {

        public int currentHealth;
        public int maxHealth;

        public CDamageableObjectData(): base () {
            this.currentHealth  = 0;
            this.maxHealth      = 0;
        }
		
	}
}
