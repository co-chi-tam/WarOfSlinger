﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
    [Serializable]
    public class CBuildingData : CDamageableObjectData {
		
		public int currentResident;
		public int maxResident;
		public CObjectData[] NPCDatas;

        public CBuildingData() : base () {
			this.currentResident    = 0;
			this.maxResident        = 0;
        }

    }
}
