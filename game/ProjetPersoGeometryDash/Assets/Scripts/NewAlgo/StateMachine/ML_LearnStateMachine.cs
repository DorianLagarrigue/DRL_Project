using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ML_LearnStateMachine
{
    bool isStop = false;
    ML_State currentState;


    public ML_LearnStateMachine()
    {
        currentState = new ML_StateInit();
    }

    public void Update()
    {
        if (isStop) return;

        currentState.Update();
        if (currentState.jobFinished)
            EndState();
    }

    public void EndState()
    {
        if (isStop) return;
        ML_State newState = currentState.OnEnd();
        currentState = null;
        currentState = newState;
        if (currentState == null)
            isStop = true;
    }

    public string GetCurrentStateName()
    {
        return currentState.jobName;
    }
}
