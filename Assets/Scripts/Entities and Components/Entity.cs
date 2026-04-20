using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WarToilet.Interfaces;
using WarToilet.Utilities;

namespace WarToilet.Entities
{
public class Entity : PooledObject {

    [SerializeField] private int maxHealth = 100;
    [SerializeField] private string[] targetTags;
    [SerializeField] private string[] touchTags;

    [ReadOnly] [SerializeField] private GameObject target;
    [ReadOnly] [SerializeField] private bool isInWater = false;

    public GameObject Target => target;
    public bool IsInWater => isInWater;

    protected IController controller;
    private IWeapon weapon;
    private List<IEntityObserver> observers = new List<IEntityObserver>();

    private int health = 0;
    protected bool isStunned = false;
    private const float stunTime = 1f;
    private const float maxSqrMoveVelocity = 16f;
    private Coroutine stunCoroutine;

    protected virtual void Awake()
    {
        controller = GetComponent<IController>();
        weapon = GetComponentInChildren<IWeapon>();

        transform.Register();
    }
	
    void OnEnable()
    {
        Heal();
    }

	protected virtual void Update ()
    {
        if (controller != null && !isStunned)
        {
            if (weapon != null)
            {
                weapon.UpdateFromEntity(controller, target);
            }
        }
	}

    void FixedUpdate()
    {
        FindTarget();
    }

    void OnTriggerEnter(Collider otherCollider)
    {
        var dangerous = otherCollider.GetDangerous();
        if(touchTags.Contains(otherCollider.tag) && dangerous != null && dangerous.IsDangerous)
        {
            TakeDamage(dangerous.ImpactPoint);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        var otherEntity = other.gameObject.GetEntity();
        if(touchTags.Contains(other.transform.tag) && otherEntity != null && !otherEntity.isStunned)
        {
            TakeDamage(other.transform.position);
        }
    }

    public void RegisterObserver(IEntityObserver observer)
    {
        observers.Add(observer);
    }

    public void InStream(bool isTrue)
    {
        if (isTrue)
        {
            Heal();
            isInWater = true;
        }
        else
        {
            isInWater = false;
        }
    }

    private void Heal()
    {
        health = maxHealth;

        TellObservers(x => x.UpdateHealth(health));
    }

    private void GetStunned(Vector3 stunPosition)
    {
        TellObservers(x => x.Stun(stunPosition, health));

        isStunned = true;

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);
        stunCoroutine = StartCoroutine(UnstunAfterDelay());
    }

    private IEnumerator UnstunAfterDelay()
    {
        yield return new WaitForSeconds(stunTime);
        Unstun();
    }

    protected virtual void Die(Vector3 damagePosition)
    {
        TellObservers(x => x.Die(damagePosition));

        enabled = false;
    }

    private void Unstun()
    {
        isStunned = false;

        TellObservers(x => x.UnStun());
    }

    protected void TellObservers(System.Action<IEntityObserver> action)
    {
        for(int i = 0; i < observers.Count; i++)
        {
            action(observers[i]);
        }
    }

    private void TakeDamage(Vector3 damagePosition)
    {
        health -= 1;

        if(health <= 0)
        {
            Die(damagePosition);
        }
        else
        {
            GetStunned(damagePosition);
        }
    }

    private void FindTarget()
    {
        if(!target || target.GetEntity() == null || !target.GetEntity().enabled || controller.ChangeTarget())
        {
            target = GetTarget();
        }
    }

    private GameObject GetTarget()
    {
        List<GameObject> targets = new List<GameObject>();
        for(int i = 0; i < targetTags.Length; i++)
        {
            targets.AddRange(GameObject.FindGameObjectsWithTag(targetTags[i]));
        }

        foreach( GameObject potentialTarget in targets.OrderBy(x => (x.transform.position - transform.position).sqrMagnitude))
        {
            if((potentialTarget != target || targets.Count == 1) && potentialTarget && potentialTarget.GetEntity().enabled)
            {
                return potentialTarget;
            }
        }

        return null;
    }

}
}
