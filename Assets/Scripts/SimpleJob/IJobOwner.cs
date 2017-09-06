using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
    public interface IJobOwner {

		bool GetActive ();
		void SetActive (bool value);

		Vector3 GetPosition ();
		void SetPosition (Vector3 value);

		Vector3 GetTargetPosition ();
		void SetTargetPosition(Vector3 value);

		CObjectController GetTargetController();
		void SetTargetController(CObjectController value);

		Vector3 GetClosestPoint (Vector3 point);

		void SetAnimation (string name, object param = null);

		void InvokeAction (string name);
		void InvokeAction (string name, params object[] prams);
		void AddAction (string name, System.Action callback);
		void AddAction (string name, System.Action<object[]> callbacks);

		CObjectController GetController();

    }
}
