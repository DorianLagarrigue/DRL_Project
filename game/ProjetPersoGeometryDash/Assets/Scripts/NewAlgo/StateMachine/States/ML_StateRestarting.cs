using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ML_StateRestarting : ML_State
{
    public ML_StateRestarting() : base()
    {
        jobName = "Restart";
    }

    public override ML_State OnEnd()
    {
        return new ML_StateWStart();
    }

    public override void OnStart()
    {
        jobFinished = false;
    }

    public override void Update()
    {
        //To change
        //if (GameManager.Instance.isRestartFinished)
        //{
            byte[] message = onlineManager.CreateReadyToStartMessage();
            bool isSended = onlineManager.SendMessage(message);
            if (!isSended)
                throw new System.Exception("Le message de Restart n'a pas pu être envoyé");

            jobFinished = true;
        //}
    }
}
