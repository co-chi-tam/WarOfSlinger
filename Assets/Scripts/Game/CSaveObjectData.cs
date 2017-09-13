using System;
using UnityEngine;

namespace WarOfSlinger {
	[Serializable]
	public class CSaveObjectData {

		public string objectPosition;

		private Vector3 m_ObjectV3Position;
		public Vector3 objectV3Position {
			get { return objectPosition.ToV3 (); }
			set { 
				this.m_ObjectV3Position = value; 
				this.objectPosition = value.ToV3String ();
			}
		}

		public CSaveObjectData ()
		{
			this.objectPosition = string.Empty;
			this.objectV3Position = Vector3.zero;
		}
		
	}
}
