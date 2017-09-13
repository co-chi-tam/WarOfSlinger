using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
    [Serializable]
    public class CCharacterData : CDamageableObjectData {

		public int consumeFood;
        public int damageCharacter;
        public int damageBuilding;
        public float actionSpeed;

        public CCharacterData(): base() {
			this.consumeFood 		= 0;
            this.damageCharacter    = 0;
            this.damageBuilding 	= 0;
            this.actionSpeed        = 0f;
        }
	    
	}
}
