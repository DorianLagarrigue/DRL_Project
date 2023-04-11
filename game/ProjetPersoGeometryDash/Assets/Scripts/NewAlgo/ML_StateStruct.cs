using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ML_StateStruct
{
    public ML_StateStruct(float[] _state, float _reward, bool _isDone, bool _isAiControl)
    {
        state = new float[_state.Length];
        _state.CopyTo(state, 0);
        reward = _reward;
        isDone = _isDone;
        isAiControl = _isAiControl;        
    }

    public void DisplayConsole()
    {
        if(isAiControl)
        {
            //Debug.Log("State: ");
            string displayArray = "[";
            foreach(var s in state)
            {
                displayArray += s + ",";
            }
            displayArray += "]";
            Debug.Log(displayArray);
            Debug.Log("Reward: " + reward);
            Debug.Log("isDone" + isDone);
            Debug.Log("isAiControl: " + isAiControl);
        }

    }

    public float[] state;
    public float reward;
    public bool isDone;
    public bool isAiControl;
}
