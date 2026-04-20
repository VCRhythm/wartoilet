using UnityEngine;

namespace WarToilet.Utilities
{
    public class Checkpoint : MonoBehaviour {

        void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag(GameTags.Player))
            {
                FindObjectOfType<PlayerSpawner>().transform.position = transform.position;
            }
            enabled = false;
        }
    }
}
