using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WarOfSlinger {
	public class CJobManager {

		// FREE LABOR LIST
		private static LinkedList<IJobLabor> jobLaborFree = new LinkedList<IJobLabor> ();
		// WORKING LABOR LIST
		private static HashSet<IJobLabor> jobLaborInProcessing = new HashSet<IJobLabor> ();

		public static List<IJobLabor> laborFreeList() {
			return jobLaborFree.ToList ();
		}

		public static List<IJobLabor> laborInProcessingList() {
			return jobLaborInProcessing.ToList ();
		}

		// SET FREE LABOR
		public static void ReturnFreeLabor (IJobLabor value) {
			if (jobLaborInProcessing.Contains (value)) {
				jobLaborInProcessing.Remove (value);
			}
			if (jobLaborFree.Contains (value) == false) {
				jobLaborFree.AddFirst (value);
			}
		}

		public static void ReleaseLabor(IJobLabor value) {
			if (jobLaborInProcessing.Contains (value)) {
				jobLaborInProcessing.Remove (value);
			}
			if (jobLaborFree.Contains (value)) {
				jobLaborFree.Remove (value);
			}
		}

		// GET LAST LIST
		public static IJobLabor GetFreeLabor() {
			var first = jobLaborFree.First;
			if (first != null) {
				jobLaborFree.RemoveFirst ();
				if (jobLaborInProcessing.Contains (first.Value) == false) {
					jobLaborInProcessing.Add (first.Value);
				}
				return first.Value;
			}
			return null;
		}

		public static bool HaveFreeLabor() {
			return jobLaborFree.Count > 0;
		}

	}
}
