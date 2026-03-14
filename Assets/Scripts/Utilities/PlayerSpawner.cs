using UnityEngine;
using System.Collections;
using WarToilet.Interfaces;
using WarToilet.Entities;

namespace WarToilet.Utilities
{
    public class PlayerSpawner : MonoBehaviour, IPoolObserver {

        private const float RespawnDelaySeconds = 5f;

        private ObjectPool pool;

        public void OnPoolInsert(Transform trans, int transIndex)
        {
            StartCoroutine(Respawn(transIndex));
        }

        public void OnPoolPop(Transform trans)
        { }

        void Start ()
        {
            pool = GetComponent<ObjectPool>();
            pool.RegisterObserver(this);
            for (int i = 0; i < pool.InitialPoolSize; i++)
            {
                Transform player = pool.GetTransformAndSetPosition(transform.position + Vector3.forward * i);
                player.GetComponent<JoystickInput>().inputAddendum = i + 1;
            }
        }

        private IEnumerator Respawn(int index)
        {
            yield return new WaitForSeconds(RespawnDelaySeconds);

            GameObject player = pool.GetTransformAndSetPosition(transform.position, index).gameObject;
            player.GetEntity().enabled = true;

        }
    }
}
