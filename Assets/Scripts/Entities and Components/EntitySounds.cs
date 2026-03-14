using UnityEngine;
using WarToilet.Interfaces;

namespace WarToilet.Entities
{
public class EntitySounds : EntityObserverBase {

    [SerializeField] private AudioClip[] footSteps;
    [SerializeField] private AudioClip waterFootStep;
    [SerializeField] private AudioClip stunAudio;
    [SerializeField] private AudioClip deathAudio;


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
        audioSource.clip = entity.IsInWater ? waterFootStep : footSteps[footStepIndex++ % footSteps.Length];
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
