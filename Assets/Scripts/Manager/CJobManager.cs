using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
	public class CJobManager {

		// FREE LABOR LIST
		private static LinkedList<IJobOwner> jobLaborFreeList = new LinkedList<IJobOwner> ();
		// WORKING LABOR LIST
		private static HashSet<IJobOwner> jobLaborInProcessing = new HashSet<IJobOwner> ();

		// SET FREE LABOR
		public static void ReturnFreeLabor (IJobOwner value) {
			if (jobLaborInProcessing.Contains (value)) {
				jobLaborInProcessing.Remove (value);
			}
			if (jobLaborFreeList.Contains (value) == false) {
				jobLaborFreeList.AddFirst (value);
			}
		}

		// GET LAST LIST
		public static IJobOwner GetFreeLabor() {
			var last = jobLaborFreeList.Last;
			if (last != null) {
				jobLaborFreeList.RemoveLast ();
				if (jobLaborInProcessing.Contains (last.Value) == false) {
					jobLaborInProcessing.Add (last.Value);
				}
				return last.Value;
			}
			return null;
		}

	}
}
