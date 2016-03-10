using UnityEngine;

public class EntityPhysics : MonoBehaviour, IEntityObserver {

    public Vector3 velocity { get { return rbody.velocity; } }

    private const float maxSqrMoveVelocity = 16;

    private const float stunImpulseForce = 5f;
    private const float stunImpactEffectAngle = 60f;
    private const float stunImpactTime = 0.2f;
    private const float impactRecoveryTime = 0.5f;

    private const float deathImpactTime = 1f;
    private const float deathImpactEffectAngle = 90f;
    
    private Rigidbody rbody;
    private ImpactResponder[] impactResponders;

    void Awake()
    {
        GetComponent<Entity>().RegisterObserver(this);

        rbody = GetComponentInChildren<Rigidbody>();
        impactResponders = GetComponentsInChildren<ImpactResponder>();
    }

    public void Stun(Vector3 hitPosition, int health)
    {
        Vector3 stunHeading = transform.position - hitPosition;
        stunHeading.y = 0;

        ShowImpact(stunHeading, stunImpactEffectAngle, stunImpactTime, true);

        rbody.AddForce(stunHeading.normalized * stunImpulseForce, ForceMode.Impulse);
    }

    public void UnStun() { }

    public void UpdateHealth(int health) { }

    public void Die(Vector3 hitPosition)
    {
        ShowImpact(transform.position - hitPosition, deathImpactEffectAngle, deathImpactTime, false);
    }

    public void Move(Vector3 moveVector) { }
    /*{
        if (moveVector != Vector3.zero && rbody.velocity.sqrMagnitude < maxSqrMoveVelocity)
        {
            rbody.AddForce(moveVector * 300f * Time.deltaTime, ForceMode.Force);
            moveVector = Vector3.zero;
        }
    }*/

    private void ShowImpact(Vector3 impact, float effectAngle, float effectSpeed, bool canRecover)
    {
        impact = transform.InverseTransformDirection(impact).normalized;

        for (int i = 0; i < impactResponders.Length; i++)
        {
            impactResponders[i].Impact(impact, effectAngle, effectSpeed, canRecover, impactRecoveryTime);
        }
    }

    private void Hop(float force)
    {
        rbody.AddForce(force * transform.up, ForceMode.Impulse);
    }

    private void Accelerate(float force)
    {
        rbody.AddForce(force * transform.forward, ForceMode.Force);
    }

    private void Push(float force)
    {
        rbody.AddForce(force * transform.forward, ForceMode.Impulse);
    }
}
