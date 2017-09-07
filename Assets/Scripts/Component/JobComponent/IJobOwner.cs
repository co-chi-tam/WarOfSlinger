using System;
using UnityEngine;

namespace WarOfSlinger {
    public interface IJobOwner {

		void ExcuteJobOwner(string jobName);
		void ClearJobOwner(string jobName);

		bool GetActive ();
		void SetActive (bool value);

		Vector3 GetPosition ();
		void SetPosition (Vector3 value);

		Vector3 GetTargetPosition ();
		void SetTargetPosition(Vector3 value);

		CObjectController GetTargetController();
		void SetTargetController(CObjectController value);

		Vector3 GetClosestPoint (Vector3 point);
		Vector3 GetUIPointPosition();

		void SetAnimation (string name, object param = null);

		CObjectController GetController();

    }
}
