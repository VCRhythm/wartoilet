using UnityEngine;
using System.Collections;
using WarToilet.Interfaces;
using WarToilet.Utilities;

namespace WarToilet.Items
{
public class Shot : PooledObject, IDangerous {

    private const float DestroyDelay = 1f;

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
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(DestroyDelay);
        Destroy();
    }
}
}
