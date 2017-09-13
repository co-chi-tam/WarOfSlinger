using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
	public interface ICharacterJobOwner : IJobOwner {

		string GetObjectType();

		int GetConsumeFood();

		void SetCurrentHealth (int value);
		int GetCurrentHealth ();
		int GetMaxHealth ();

		int GetDamageBuilding();
		int GetDamageCharacter();
		
	}
}
