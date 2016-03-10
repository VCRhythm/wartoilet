using UnityEngine;

public interface IDangerous {
    bool IsDangerous { get; }
    Vector3 ImpactPoint { get; }
}
