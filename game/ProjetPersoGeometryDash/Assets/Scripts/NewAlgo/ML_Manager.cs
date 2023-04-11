using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ML_Manager : MonoBehaviour
{
    [SerializeField] private int nbEpisode;
    [SerializeField] private int frequenceTest;
    [SerializeField] private ML_BigObserverManager observerManager;
    [SerializeField] private ML_OnlineManager onlineManager;
    [SerializeField] public ML_eGameMode gamemode;
    [SerializeField] private bool isAiControl;
    private ML_LearnStateMachine learnStateMachine;
    [SerializeField] public UnityEvent InitGameFunction;
    [SerializeField] public UnityEvent StartPlayingFunction;
    public float timeScale = 1.0f;
    public int currentEpisode = 0;



    private static ML_Manager instance;

    public static ML_Manager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType(typeof(ML_Manager)) as ML_Manager;

            return instance;
        }
        set
        {
            instance = value;
        }

    }

    public ML_OnlineManager OnlineManager { get => onlineManager; }
    public ML_BigObserverManager ObserverManager { get => observerManager; }

    public ML_BigObserverManager GetObserverManager()
    {
        return ObserverManager;
    }
    public int GetNbEpisode()
    {
        return nbEpisode;
    }
    public int GetFreqTest()
    {
        return frequenceTest;
    }

    private void Awake()
    {
        observerManager = new ML_BigObserverManager(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = timeScale;
        if (isAiControl)
        {
            onlineManager.observer = observerManager;
            learnStateMachine = new ML_LearnStateMachine();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isAiControl)
        {
            learnStateMachine.Update();
        }
    }

    public void StartPlay()
    {
        Debug.Log("StartPlay");
    }
    public void InitPlay()
    {
        Debug.Log("InitPlay");
    }
}
