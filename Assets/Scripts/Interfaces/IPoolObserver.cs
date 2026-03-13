using UnityEngine;

namespace WarToilet.Interfaces
{
    public interface IPoolObserver {
        void OnPoolInsert(Transform objectTransform, int objectIndex);
        void OnPoolPop(Transform objectTransform);
    }
}
