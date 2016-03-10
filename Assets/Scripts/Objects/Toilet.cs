using UnityEngine;
using System.Collections;
using System.Linq;

public class Toilet : MonoBehaviour, IPoolObserver, ITriggerObserver {

    public int numToSpawn = 0;

    private int activeSpawns = 0;
    private int spawnCount = 0;
    private ObjectPool pool;
    private Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        pool = GetComponent<ObjectPool>();

        pool.RegisterObserver(this);

        Trigger[] triggers = transform.parent.parent.GetComponentsInChildren<Trigger>();
        triggers.First(x => x.type == Trigger.Type.In).Register(this);
    }

    public void EnterInTrigger(Collider collider)
    {
        if (spawnCount < numToSpawn)
        {
            StartCoroutine(StartSpawning());
        }
        else if (activeSpawns == 0)
        {
            MakeDoor();
        }
    }

    public void EnterOutTrigger(Collider collider) { }

    public void ExitOutTrigger(Collider collider) { }

    public void OnPoolPop(Transform trans) {}

    public void OnPoolInsert(Transform trans, int transIndex)
    {
        activeSpawns--;

        if(activeSpawns <= 0 && numToSpawn == spawnCount)
        {
            MakeDoor();
        }
    }

    private void MakeDoor()
    {
        animator.SetBool("IsSpawning", false);

        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop();
        }

        transform.parent.GetComponentInParent<LevelLoad>().LoadLevel();
        animator.SetBool("IsDoor", true);
    }

    private IEnumerator StartSpawning()
    {
        animator.SetBool("IsSpawning", true);
        foreach(ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Play();
        }

        while (spawnCount < numToSpawn)
        {
            animator.SetTrigger("Open");
            pool.GetTransformAndSetPosition(transform.position);

            spawnCount++;
            activeSpawns++;

            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }
}
