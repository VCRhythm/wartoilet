using UnityEngine;

public class EntitySounds : MonoBehaviour, IEntityObserver {

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

    public void Die(Vector3 position)
    {
        audioSource.clip = deathAudio;
        audioSource.Play();
    }

    public void UpdateHealth(int health) { }

    public void Move(Vector3 moveVector) { }

    public void Stun(Vector3 position, int health)
    {
        audioSource.clip = stunAudio;
        audioSource.Play();
    }

    public void UnStun() { }
}
