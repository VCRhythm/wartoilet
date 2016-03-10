using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public string poolName;
    public int initialPoolSize = 10;
    public PooledObject[] pooledObjectPrefabs;

    [ReadOnly]
    public int poolSize = 0;
    public bool canGrow = false;

    public Stack<PooledObject>[] PooledObjects { get; private set; }
    public List<PooledObject> ActiveObjects { get; private set; }

    private Transform parent = null;
    private Dictionary<GameObject, PooledObject> poolObjectLookup = new Dictionary<GameObject, PooledObject>();
    private int TotalObjects
    {
        get
        {
            int sum = 0;
            for (int i = 0; i < PooledObjects.Length; i++)
                sum += PooledObjects[i].Count;
            
            return sum;
        }
    }

    private List<IPoolObserver> observers = new List<IPoolObserver>();

    void Awake()
    {
        GameObject pool = GameObject.Find(poolName);
        if(pool)
        {
            parent = pool.transform;
        }
        else
        {
            parent = new GameObject(poolName).transform;
        }
        

/*        if (initialPoolSize < pooledObjectPrefabs.Length)
        {
            initialPoolSize = pooledObjectPrefabs.Length;
        }
        */
        ActiveObjects = new List<PooledObject>();
        PooledObjects = new Stack<PooledObject>[pooledObjectPrefabs.Length];
        for (int i = 0; i < pooledObjectPrefabs.Length; i++)
        {
            PooledObjects[i] = new Stack<PooledObject>();
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            Grow(false);
        }
    }

    public void RegisterObserver(IPoolObserver observer)
    {
        observers.Add(observer);
    }

    public Transform GetTransformAndSetPosition(Vector3 position, int index = -1)
    {
        PooledObject obj = Pop(index);
        if (obj != null)
        {
            obj.SetPositionAndActivate(position);
            TellObservers((x) => { x.OnPoolPop(obj.transform); });

            return obj.transform;
        }
        return null;
    }

    public void InsertAllActiveObjects()
    {
        for(int i=ActiveObjects.Count-1; i>=0; i--)
        {
            Insert(ActiveObjects[i].gameObject);
        }
    }

    public void DestroyAllObjects()
    {
        InsertAllActiveObjects();

        for (int i = PooledObjects.Length - 1; i >= 0; i--)
        {
            PooledObject[] pooledObjects = PooledObjects[i].ToArray();
            for(int j=pooledObjects.Length -1; j>=0; j--)
            {
                Destroy(pooledObjects[j].gameObject);
            }
        }
    }

    public void Insert(GameObject gameObject)
    {
        gameObject.transform.DOKill();
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }

        PooledObject obj = poolObjectLookup[gameObject];
        ActiveObjects.Remove(obj);
        Push(obj);

        TellObservers((x) => { x.OnPoolInsert(obj.transform, obj.index); });
    }

    private void TellObservers(System.Action<IPoolObserver> action)
    {
        for(int i=0; i<observers.Count; i++)
        {
            action(observers[i]);
        }
    }

    private PooledObject Pop(int requestedIndex = -1)
    {
        int index = ProcessRequestedIndex(requestedIndex);

        if (PooledObjects[index].Count > 0)
        {
            PooledObject obj = PooledObjects[index].Pop();
            ActiveObjects.Add(obj);
            return obj;
        }

        if (canGrow)
        {
            PooledObject obj = Grow(true, requestedIndex);
            ActiveObjects.Add(obj);
            return obj;
        }

        return null;
    }

    private int ProcessRequestedIndex(int index)
    {
        if (index >= 0 && index < PooledObjects.Length)
        {
            return index;
        }
        else if (canGrow || HasAvailableObject())
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, PooledObjects.Length);
            } while (PooledObjects[randomIndex].Count == 0 && !canGrow);

            return randomIndex;
        }

        return 0;
    }

    private bool HasAvailableObject()
    {
        for(int i=0; i<PooledObjects.Length; i++)
        {
            if(PooledObjects[i].Count > 0)
            {
                return true;
            }
        }
        return false;
    }

    private void Push(PooledObject obj)
    {
        obj.gameObject.SetActive(false);
        PooledObjects[obj.index].Push(obj);
    }

    private PooledObject Grow(bool isPopped, int index = -1)
    {
        if (index < 0 || index >= pooledObjectPrefabs.Length)
        {
            index = TotalObjects % pooledObjectPrefabs.Length;
        }

        PooledObject obj = Instantiate(pooledObjectPrefabs[index]) as PooledObject;
        obj.index = index;

        if (parent != null) obj.transform.SetParent(parent);
        obj.Pool = this;

        poolObjectLookup.Add(obj.gameObject, obj);

        poolSize++;

        if (!isPopped)
        {
            Push(obj);
            return null;
        }

        return obj;
    }
}