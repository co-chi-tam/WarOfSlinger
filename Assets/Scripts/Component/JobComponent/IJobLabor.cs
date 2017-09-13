using System;
using UnityEngine;

namespace WarOfSlinger {
	public interface IJobLabor : IJobOwner {

		bool IsActive();

		void ClearJobLabor();
		
	}
}
