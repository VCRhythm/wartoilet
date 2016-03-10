using UnityEngine;

public class EntityMove : MonoBehaviour {

    private Rigidbody rbody;
    
    void Awake()
    {
        rbody = GetComponentInParent<Rigidbody>();
    }

}
