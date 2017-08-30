using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
    [Serializable]
    public class CCharacterData : CDamageableObjectData {

        public int damageCharacter;
        public int damageConstruction;
        public float actionSpeed;

        public CCharacterData(): base() {
            this.damageCharacter    = 0;
            this.damageConstruction = 0;
            this.actionSpeed        = 0f;
        }
	    
	}
}
