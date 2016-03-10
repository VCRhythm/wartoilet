using UnityEngine;

public interface IEntityObserver {
    void UpdateHealth(int health);
    void Stun(Vector3 position, int health);
    void UnStun();
    void Die(Vector3 position);
    void Move(Vector3 moveVector);
}
