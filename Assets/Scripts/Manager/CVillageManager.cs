using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
	public class CVillageManager {

		public static CVillageData GetData() {
			return CGameManager.Instance.GetData ();
		}

		public static void SetCurrentPopulation(int value) {
			var villageData = CGameManager.Instance.GetData ();
			villageData.currentPopulation = value >= villageData.maxPopulation ? villageData.maxPopulation : value;
		}

		public static int GetCurrentPopulation() {
			var villageData = CGameManager.Instance.GetData ();
			return villageData.currentPopulation;
		}

		public static void SetMaxPopulation(int value) {
			var villageData = CGameManager.Instance.GetData ();
			villageData.maxPopulation = value >= 99 ? 99 : value;
		}

		public static int GetMaxPopulation() {
			var villageData = CGameManager.Instance.GetData ();
			return villageData.maxPopulation;
		}

		public static CResourceData GetResource(string name) {
			var villageData = CGameManager.Instance.GetData ();
			var resourceData = villageData.villageResources;
			for (int i = 0; i < resourceData.Length; i++) {
				if (resourceData [i].resouceName == name) {
					return resourceData [i];
				}
			}
			return null;
		}

		public static void SetResource(string name, int value) {
			var villageData = CGameManager.Instance.GetData ();
			var resourceData = villageData.villageResources;
			for (int i = 0; i < resourceData.Length; i++) {
				if (resourceData [i].resouceName == name) {
					resourceData [i].currentAmount = value >= resourceData [i].maxAmount ? resourceData [i].maxAmount : value;
				}
			}
		}
	
	}
}
