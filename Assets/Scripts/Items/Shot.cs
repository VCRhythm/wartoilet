using UnityEngine;

public class Shot : PooledObject, IDangerous {

    private ParticleSystem hitSplatter;

    public Vector3 ImpactPoint { get { return transform.position; } }
    public bool IsDangerous { get {return true; } }

    void Awake()
    {
        transform.Register();

        hitSplatter = GetComponentsInChildren<ParticleSystem>()[1];
    }

    void OnCollisionEnter(Collision other)
    {
        hitSplatter.Play();
        Invoke("Destroy", 1f);
    }
}
