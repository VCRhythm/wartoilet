using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour, IPoolObserver {

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
        for (int i = 0; i < pool.initialPoolSize; i++)
        {
            Transform player = pool.GetTransformAndSetPosition(transform.position + Vector3.forward * i);
            player.GetComponent<JoystickInput>().inputAddendum = i + 1;
        }
    }

    private IEnumerator Respawn(int index)
    {
        yield return new WaitForSeconds(5f);

        GameObject player = pool.GetTransformAndSetPosition(transform.position, index).gameObject;
        player.GetEntity().enabled = true;

    }
}

