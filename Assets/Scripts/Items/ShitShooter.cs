using UnityEngine;
using WarToilet.Interfaces;
using WarToilet.Utilities;

namespace WarToilet.Items
{
public class ShitShooter : MonoBehaviour, IWeapon
{
    private ObjectPool pool;
    private float nextShotTime;
    private const float shitStartPosition = 2;
    [SerializeField] private float shitForce = 30f;
    [SerializeField] private float shotSpeed = 5f;

    void Awake()
    {
        pool = GetComponent<ObjectPool>();
    }

    public void UpdateFromEntity(IController controller, GameObject target)
    {
        if(Time.time > nextShotTime)
        {
            ShootShot(target);
        }
    }

    private void ShootShot(GameObject target)
    {
        Transform shotTransform = pool.GetTransformAndSetPosition(transform.position + transform.forward * shitStartPosition);
        if (shotTransform != null)
        {
            shotTransform.GetComponent<Rigidbody>().AddForce((target.transform.position - transform.position).normalized * shitForce, ForceMode.Impulse);
        }
        nextShotTime = Time.time + shotSpeed;
    }
}
}
