using UnityEngine;

public interface IWeapon {
    void UpdateFromEntity(IController controller, GameObject target);
}
