using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public interface ICharacterContext : IContext {

		bool DidMoveToTarget();
		bool HaveTargetObject();
		bool IsActive();

	}
}
