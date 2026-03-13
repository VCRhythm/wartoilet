using UnityEngine;
using WarToilet.Interfaces;

namespace WarToilet.Entities
{
public class EntitySounds : EntityObserverBase {

    public AudioClip[] footSteps;
    public AudioClip waterFootStep;
    public AudioClip stunAudio;
    public AudioClip deathAudio;


    private int footStepIndex;

    private AudioSource audioSource;
    private Entity entity;

    void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entity.RegisterObserver(this);

        audioSource = GetComponentInParent<AudioSource>();
    }

    public void PlayFootstep()
    {
        audioSource.clip = entity.isInWater ? waterFootStep : footSteps[footStepIndex++ % footSteps.Length];
        audioSource.Play();
    }

    public override void Die(Vector3 position)
    {
        audioSource.clip = deathAudio;
        audioSource.Play();
    }

    public override void Stun(Vector3 position, int health)
    {
        audioSource.clip = stunAudio;
        audioSource.Play();
    }

}
}
