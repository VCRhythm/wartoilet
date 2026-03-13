using UnityEngine;
using WarToilet.Interfaces;

namespace WarToilet.Entities
{
public class EntityParticles : EntityObserverBase
{
    private ParticleSystem[] particles;

    void Awake()
    {
        GetComponent<Entity>().RegisterObserver(this);

        particles = GetComponentsInChildren<ParticleSystem>();
    }

    public override void Stun(Vector3 stunPosition, int health)
    {
        if (particles.Length > 0)
        {
            ChangeParticlesUpDirectionAndPlay(0, stunPosition);
        }
    }

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
}
