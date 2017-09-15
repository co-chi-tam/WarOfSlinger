using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
	public interface IGuarderJobOwner : IJobOwner {

		bool HaveEnemy ();

		List<CObjectController> GetEnemies();

		void SendRandomEnemyCommand(string command = "AttackCommand");
		
	}
}
