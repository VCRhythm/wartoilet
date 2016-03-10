using UnityEngine;
using System.Collections.Generic;

public class Trigger : MonoBehaviour {

    public enum Type
    {
        In,
        Out
    }
    public Type type;

    private List<ITriggerObserver> observers = new List<ITriggerObserver>();

    public void Register(ITriggerObserver observer)
    {
        observers.Add(observer);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(type == Type.In)
            {
                TellObservers((ITriggerObserver observer) => { observer.EnterInTrigger(other); });
                Destroy(gameObject);
            }
            else if(type == Type.Out)
            {
                TellObservers((ITriggerObserver observer) => { observer.EnterOutTrigger(other); });
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(type == Type.Out)
            {
                TellObservers((ITriggerObserver observer) => { observer.ExitOutTrigger(other); });
            }
        }
    }

    private void TellObservers(System.Action<ITriggerObserver> action)
    {
        for(int i = 0; i < observers.Count; i++)
        {
            action(observers[i]);
        }
    }
}