using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent2 : ML_Agent
{
    public bool isDone;

    public override float GetReward()
    {
        return 0.2f;
    }

    public override float[] GetState()
    {
        float[] array = new float[5];
        foreach (float f in array)
        {
            array[0] = f+1;
        }
        return array;
    }

    public override bool IsAIControl()
    {
        return true;
    }

    public override bool IsDone()
    {
        return isDone;
    }

    // Start is called before the first frame update
    void Start()
    {
        ML_ObserverManager = ML_Manager.Instance.ObserverManager;
        observer = ML_ObserverManager.RegisterAgent(this);
        observer.AddAction(Action0);
        observer.AddAction(Action1);
    }

    public int Action0()
    {
        Debug.Log("Action1");
        return 0;
    }

    public int Action1()
    {
        Debug.Log("Action1");
        return 0;
    }
}
