using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
    public interface IBuildingJobOwner : IJobOwner {

        // RESIDENCE
        int GetCurrentResident();
        void SetCurrentResident(int value);

    }
}
