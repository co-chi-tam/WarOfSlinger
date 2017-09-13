using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
	public class CVillageManager {

		public static CVillageData GetData() {
			return CGameManager.Instance.GetData ();
		}

		public static bool IsEnoughResources(Dictionary<string, int> values) {
			var villageData = CGameManager.Instance.GetData ();
			var resourceData = villageData.villageResources;
			var isEnough = true;
			foreach (var item in values) {
				for (int i = 0; i < resourceData.Length; i++) {
					if (resourceData [i].resouceName == item.Key) {
						isEnough &= resourceData [i].currentAmount >= item.Value;
						break;
					}
				}
			}
			return isEnough;
		}

		public static bool IsEnoughResources(CResourceData[] values) {
			var villageData = CGameManager.Instance.GetData ();
			var resourceData = villageData.villageResources;
			var isEnough = true;
			for (int i = 0; i < values.Length; i++) {
				var resourceValue = values [i];
				for (int x = 0; x < resourceData.Length; x++) {
					if (resourceData [x].resouceName == resourceValue.resouceName) {
						isEnough &= resourceData [x].currentAmount >= resourceValue.currentAmount;
						break;
					}
				}
			}
			return isEnough;
		}

		public static bool IsSubstractResources(CResourceData[] values) {
			if (IsEnoughResources (values)) {
				var villageData = CGameManager.Instance.GetData ();
				var resourceData = villageData.villageResources;
				for (int i = 0; i < values.Length; i++) {
					var resourceValue = values [i];
					for (int x = 0; x < resourceData.Length; x++) {
						if (resourceData [x].resouceName == resourceValue.resouceName) {
							var totalValue = resourceData [x].currentAmount - resourceValue.currentAmount;
							resourceData [x].currentAmount = totalValue < 0 ? 0 : totalValue;
							break;
						}
					}
				}
				return true;
			}
			return false;
		}

		public static bool IsAddResources(CResourceData[] values) {
			var villageData = CGameManager.Instance.GetData ();
			var resourceData = villageData.villageResources;
			for (int i = 0; i < values.Length; i++) {
				var resourceValue = values [i];
				for (int x = 0; x < resourceData.Length; x++) {
					if (resourceData [x].resouceName == resourceValue.resouceName) {
						var totalValue = resourceData [x].currentAmount + resourceValue.currentAmount;
						resourceData [x].currentAmount = totalValue > resourceData [x].maxAmount ? resourceData [x].maxAmount : totalValue;
						break;
					}
				}
			}
			return true;
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
					resourceData [i].currentAmount = value >= resourceData [i].maxAmount 
														? resourceData [i].maxAmount 
														: value < 0 
														? 0 
														: value;
					break;
				}
			}
		}

	}
}
