using UnityEngine;
using System.Collections;
using System.Linq;
using WarToilet.Interfaces;
using WarToilet.Utilities;

namespace WarToilet.Objects
{
    public class Toilet : MonoBehaviour, IPoolObserver, ITriggerObserver {

        private const float MinSpawnDelay = 1f;
        private const float MaxSpawnDelay = 3f;

        [SerializeField] private int numToSpawn = 0;

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
            animator.SetBool(AnimatorParams.IsSpawning, false);

            foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
            {
                ps.Stop();
            }

            transform.parent.GetComponentInParent<LevelLoad>()?.LoadLevel();
            animator.SetBool(AnimatorParams.IsDoor, true);
        }

        private IEnumerator StartSpawning()
        {
            animator.SetBool(AnimatorParams.IsSpawning, true);
            foreach(ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
            {
                ps.Play();
            }

            while (spawnCount < numToSpawn)
            {
                animator.SetTrigger(AnimatorParams.Open);
                pool.GetTransformAndSetPosition(transform.position);

                spawnCount++;
                activeSpawns++;

                yield return new WaitForSeconds(Random.Range(MinSpawnDelay, MaxSpawnDelay));
            }
        }
    }
}
