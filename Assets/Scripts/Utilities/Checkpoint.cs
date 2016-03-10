using UnityEngine;

public class Checkpoint : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            FindObjectOfType<PlayerSpawner>().transform.position = transform.position;
        }
        enabled = false;
    }
}