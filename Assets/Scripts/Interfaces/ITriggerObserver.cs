using UnityEngine;

public interface ITriggerObserver {
    void EnterInTrigger(Collider other);
    void EnterOutTrigger(Collider other);
    void ExitOutTrigger(Collider other);
}
