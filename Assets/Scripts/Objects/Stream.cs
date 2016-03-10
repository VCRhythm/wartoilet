using UnityEngine;

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
