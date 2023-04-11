using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ML_State
{
    public bool jobFinished;
    public string jobName;
    protected ML_OnlineManager onlineManager;
    protected ML_BigObserverManager bigObserverManager;
    protected ML_Manager manager;
    public ML_State()
    {
        manager = ML_Manager.Instance;
        onlineManager = ML_Manager.Instance.OnlineManager;
        bigObserverManager = ML_Manager.Instance.ObserverManager;
        OnStart();
    }
    public abstract void OnStart();
    public abstract void Update();
    public abstract ML_State OnEnd();
}
