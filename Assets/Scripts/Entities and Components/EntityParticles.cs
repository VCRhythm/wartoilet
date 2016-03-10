using UnityEngine;

public class EntityParticles : MonoBehaviour, IEntityObserver
{
    private ParticleSystem[] particles;

    void Awake()
    {
        GetComponent<Entity>().RegisterObserver(this);

        particles = GetComponentsInChildren<ParticleSystem>();
    }

    public void Die(Vector3 position)
    {
    }

    public void Stun(Vector3 stunPosition, int health)
    {
        if (particles.Length > 0)
        {
            ChangeParticlesUpDirectionAndPlay(0, stunPosition);
        }
    }

    public void UnStun() {}

    public void UpdateHealth(int health) { }

    public void Move(Vector3 moveVector) { }

    private void PlayParticles(int index)
    {
        particles[index].Play();
    }

    private void ChangeParticlesUpDirectionAndPlay(int index, Vector3 upPosition)
    {
        upPosition.y = particles[index].transform.position.y;
        particles[index].transform.LookAt(2 * particles[index].transform.position - upPosition);
        PlayParticles(index);
    }

}
