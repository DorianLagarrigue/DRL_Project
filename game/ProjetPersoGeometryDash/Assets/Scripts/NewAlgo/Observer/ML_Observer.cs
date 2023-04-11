using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ML_Observer
{
    ML_Agent agent;
    List<Func<int>> actionsFcts = new List<Func<int>>();

    public ML_Observer(ML_Agent newAgent)
    {
        agent = newAgent;
    }

    public int GetStateShape()
    {
        return agent.GetState().Length;
    }

    public ML_StateStruct GetStructState()
    {
        ML_StateStruct state = new ML_StateStruct(agent.GetState(), agent.GetReward(), agent.IsDone(), agent.IsAIControl());
        
        //state.DisplayConsole();
        return state;
    }

    public void DoAction(int actionId)
    {
        if (agent.IsDone() || !agent.IsAIControl()) return;


        if(actionId < 0 || actionId >= actionsFcts.Count)
            throw new System.Exception("Action request is "+actionId+" but max action id is:"+(actionsFcts.Count-1).ToString());
        actionsFcts[actionId]();
    }
    
    public ML_Agent GetAgent()
    {
        return agent;
    }
    
    public void AddAction(Func<int> action)
    {
        actionsFcts.Add(action);
    }
    public void RemoveAction(Func<int> action)
    {
        actionsFcts.Remove(action);
    }

    public int GetActionShape()
    {
        return actionsFcts.Count;
    }
    
    public List<uint> GetLayers()
    {
        return agent.GetLayers();
    }

    public float GetBatchSize()
    {
        return (float)agent.BatchSize;
    }
    public float GetMemorySize()
    {
        return (float)agent.MemorySize;
    }
    public float GetDiscountFactor()
    {
        return agent.DiscountFactor;
    }
    public float GetLearningRate()
    {
        return agent.LearningRate;
    }


}
