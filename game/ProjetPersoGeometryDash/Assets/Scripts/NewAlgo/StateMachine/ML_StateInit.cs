using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ML_StateInit : ML_State
{
    public ML_StateInit() : base()
    {
        jobName = "Initialization";
    }

    public override ML_State OnEnd()
    {
        return new ML_StateChoiceGameMode();
    }

    public override void OnStart()
    {

    }

    public override void Update()
    {
        onlineManager.InitConnection();
        onlineManager.Receive();
        manager.InitGameFunction.Invoke();
        jobFinished = true;
        return;
    }
}
