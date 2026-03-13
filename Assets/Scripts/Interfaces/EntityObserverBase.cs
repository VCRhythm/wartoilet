using UnityEngine;

namespace WarToilet.Interfaces
{
    public abstract class EntityObserverBase : MonoBehaviour, IEntityObserver
    {
        public virtual void UpdateHealth(int health) { }
        public virtual void Stun(Vector3 position, int health) { }
        public virtual void UnStun() { }
        public virtual void Die(Vector3 position) { }
        public virtual void Move(Vector3 moveVector) { }
    }
}
