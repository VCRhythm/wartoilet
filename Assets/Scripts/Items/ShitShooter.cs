using UnityEngine;

public class ShitShooter : MonoBehaviour, IWeapon
{
    private ObjectPool pool;
    private float nextShotTime;
    private const float shitStartPosition = 2;
    public float shitForce = 30f;
    public float shotSpeed = 5f;

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
        pool.GetTransformAndSetPosition(transform.position + transform.forward * shitStartPosition).GetComponent<Rigidbody>().AddForce((target.transform.position - transform.position).normalized * shitForce, ForceMode.Impulse);
        nextShotTime = Time.time + shotSpeed;
    }
}
