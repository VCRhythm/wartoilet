using UnityEngine;

public abstract class PooledObject : MonoBehaviour
{
    [HideInInspector] public int index;
    [HideInInspector] public ObjectPool Pool;
    public System.Action deregisterAction = () => { };
    
	public bool IsActive { get { return gameObject.activeInHierarchy; } set { gameObject.SetActive(value); } }
	
	public void SetPositionAndActivate(Vector3 position)
	{
		transform.position = position;
		gameObject.SetActive(true);
	}

    public virtual void Destroy()
    {
        if (Pool != null) Pool.Insert(gameObject);
    }
}