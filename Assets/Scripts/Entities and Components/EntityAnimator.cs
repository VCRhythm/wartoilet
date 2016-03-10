using UnityEngine;

public class EntityAnimator : MonoBehaviour, IEntityObserver {

    public bool overrideAnimatorForDeath = false;

    private Animator animator;
    private Entity entity;
    private Transform entityTransform;
    private Vector3 acceleration { get { return entityTransform.TransformDirection(localAcceleration); } }
    private Vector3 localAcceleration { get { return new Vector3(animator.GetFloat("Direction"), 0, animator.GetFloat("Speed")) * accelerationFactor; } }

    private Vector3 velocity;

    public float frictionFactor = .9f;
    public float accelerationFactor = .01f;
    private const float maxVelocitySqrMagnitude = 200f;
    private const float turnSpeed = 2f;
    private const float animateFactor = 0.25f;

    void Awake()
    {
        animator = GetComponent<Animator>();

        entity = GetComponentInParent<Entity>();
        entityTransform = entity.transform;
        entity.RegisterObserver(this);
    }

    void OnEnable()
    {
        if(animator)
        {
            animator.enabled = true;
        }
    }

    public void Move(Vector3 moveVector)
    {
        Vector3 inverseMoveVector = transform.InverseTransformDirection(moveVector);

        SetAnimatorValues(inverseMoveVector, (Mathf.Abs(inverseMoveVector.z) + Mathf.Abs(inverseMoveVector.x)) * animateFactor);

        velocity += acceleration * Time.deltaTime;
        velocity -= velocity * (frictionFactor * Time.deltaTime);

        entityTransform.position += velocity;
    }

    public void UpdateHealth(int health) { }

    public void Die(Vector3 position)
    {
        if (animator)
        {
            if (overrideAnimatorForDeath)
            {
                animator.enabled = false;
            }
            else
            {
                animator.SetTrigger("Die");
            }
        }
    }

    public void Stun(Vector3 position, int health)
    {
        SetAnimatorValues(((transform.position - position) * 2).normalized, 1.5f);
    }

    public void UnStun()
    {
        animator.SetFloat("Speed", 0);
    }

    private void SetAnimatorValues(Vector3 moveVector, float animationSpeed)
    {
        animator.SetFloat("Speed", moveVector.z);
        animator.SetFloat("Direction", moveVector.x);
        animator.SetFloat("AnimationSpeed", animationSpeed);
        Debug.Log(animator.GetFloat("Speed"));
    }

    private void TurnTowardsTarget()
    {
        if (Mathf.Abs(acceleration.x) + Mathf.Abs(acceleration.z) < 0.1f) return;

        if (entity.target && entity.target.transform.position != entityTransform.position)
        {
            entityTransform.rotation = Quaternion.Slerp(entityTransform.rotation, Quaternion.LookRotation(new Vector3(entity.target.transform.position.x - entityTransform.position.x, 0, entity.target.transform.position.z - entityTransform.position.z), Vector3.up), turnSpeed * Time.deltaTime);
        }
        else
        {
            entityTransform.rotation = Quaternion.Slerp(entityTransform.rotation, Quaternion.LookRotation(acceleration, Vector3.up), turnSpeed * Time.deltaTime);
        }
    }
}
