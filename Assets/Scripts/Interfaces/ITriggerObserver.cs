using UnityEngine;

namespace WarToilet.Interfaces
{
    public interface ITriggerObserver {
        void EnterInTrigger(Collider other);
        void EnterOutTrigger(Collider other);
        void ExitOutTrigger(Collider other);
    }
}
