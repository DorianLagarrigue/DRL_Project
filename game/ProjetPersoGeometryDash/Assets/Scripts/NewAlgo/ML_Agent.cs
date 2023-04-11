using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ML_Agent : MonoBehaviour
{
    [SerializeField] protected ML_BigObserverManager ML_ObserverManager;
    [SerializeField] protected ML_Observer observer;
    private bool isAlreadyDone;

    [SerializeField] private List<uint> modelLayers = new List<uint>();
    
    [SerializeField] private uint memorySize;
    [SerializeField] private uint batchSize;
    [SerializeField] private float learningRate;
    [SerializeField] private float discountFactor;

    public float DiscountFactor { get => discountFactor; }
    public float LearningRate { get => learningRate; }
    public uint MemorySize { get => memorySize; }
    public uint BatchSize { get => batchSize; }

    public abstract float[] GetState();
    public abstract float GetReward();

    public abstract bool IsDone();
    public abstract bool IsAIControl();//to rename


    public List<uint> GetLayers()
    {
        return modelLayers;
    }
}
