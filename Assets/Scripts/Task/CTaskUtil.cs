using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneTask {
	public class CTaskUtil {

		public static Dictionary<string, object> REFERENCES = new Dictionary<string, object> () { 
//			{ USER_DATA,			new CUserData()	}
		};

		public static object Get(string name) {
			return REFERENCES [name];
		}

		public static T Get<T>(string name) {
			var value = REFERENCES [name];
			var convert = Convert.ChangeType (value, typeof(T));
			return (T)convert;
		}

		public static void Set(string name, object value) {
			REFERENCES [name] = value;
		}

	}

}
