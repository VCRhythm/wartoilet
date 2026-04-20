using UnityEngine;

namespace WarToilet.Interfaces
{
    public interface IDangerous {
        bool IsDangerous { get; }
        Vector3 ImpactPoint { get; }
    }
}
