using UnityEngine;

public interface IController {

    Vector3 GetMoveVector();
    Swing GetSwing();
    float GetWeaponPosition();
    bool ChangeTarget();
}
