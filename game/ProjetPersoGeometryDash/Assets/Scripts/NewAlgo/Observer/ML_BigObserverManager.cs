using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ML_ModelParameters
{   
    public float actionShape;
    public float enviroShape;

    public float learningRate;
    public float discountFactor;

    public float memorySize;
    public float batchSize;

    public int[] layers;

}

public class ML_BigObserverManager
{

    private List<ML_Observer> observers = new List<ML_Observer>();
    private ML_Manager MLmanager;
    public ML_BigObserverManager(ML_Manager manager)
    {
        MLmanager = manager;
    }

    public ML_Observer RegisterAgent(ML_Agent agent)
    {
        if (agent == null)
        {
            throw new System.Exception("Agent is null");
        }
        ML_Observer ob = new ML_Observer(agent);
        observers.Add(ob);
        return ob;
    }

    public ML_StateStruct[] GetAllStates()
    {
        List<ML_StateStruct> allStates = new List<ML_StateStruct>();
        foreach (ML_Observer ob in observers)
        {
            allStates.Add(ob.GetStructState());
        }

        return allStates.ToArray();
    }

    public int GetAgentCount()
    {
        return observers.Count;
    }


    public ML_ModelParameters[] GetModelsParameters()
    {
        List<ML_ModelParameters> models = new List<ML_ModelParameters>();
        foreach (ML_Observer ob in observers)
        {
            ML_ModelParameters model = new ML_ModelParameters();
            model.learningRate = ob.GetLearningRate();
            model.discountFactor = ob.GetDiscountFactor();
            model.batchSize = ob.GetBatchSize();
            model.memorySize = ob.GetMemorySize();
            model.actionShape = ob.GetActionShape();
            model.enviroShape = ob.GetStateShape();

            List<uint> layer = ob.GetLayers();
            List<int> layers = new List<int>();
            foreach (uint l in layer)
                layers.Add((int)l);
            layers.Add(-1);

            model.layers = layers.ToArray();

            models.Add(model);
        }

        return models.ToArray();
    }

    public void DoActions(int[] actions)
    {
        int index = 0;
        foreach(ML_Observer ob in observers)
        {
            ob.DoAction(actions[index]);
            ++index;
        }
    }
}
