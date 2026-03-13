using UnityEngine;

namespace WarToilet.Interfaces
{
    public interface IWeapon {
        void UpdateFromEntity(IController controller, GameObject target);
    }
}
