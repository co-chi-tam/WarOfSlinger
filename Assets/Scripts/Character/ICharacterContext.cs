using System;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public interface ICharacterContext : IContext {

		bool DidMoveToTarget();
		bool HaveTargetObject();
		bool IsActive();

		void InvokeAction (string name);
		void InvokeAction (string name, params object[] prams);
		void AddAction (string name, Action callback);
		void AddAction (string name, Action<object[]> callbacks);

	}
}
