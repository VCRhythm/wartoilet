using UnityEngine;
using WarToilet.Items;

namespace WarToilet.Interfaces
{
    public interface IController {

        Vector3 GetMoveVector();
        Swing GetSwing();
        float GetWeaponPosition();
        bool ChangeTarget();
    }
}
