using UnityEngine;
using WarToilet.Utilities;
using WarToilet.Entities;

namespace WarToilet.Objects
{
    public class Stream : MonoBehaviour {
        void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                other.gameObject.GetEntity().InStream(true);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                other.gameObject.GetEntity().InStream(false);
            }
        }
    }
}
