using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ML_StateChoiceGameMode :ML_State
{
    public ML_StateChoiceGameMode() : base()
    {
        jobName = "GameModeChoice";
    }

    public override ML_State OnEnd()
    {
        return new ML_StateWStart();
    }

    public override void OnStart()
    {


    }

    public override void Update()
    {
        byte[] message = onlineManager.CreateGameModeMessage();
        bool isSended = onlineManager.SendMessage(message);
        if (isSended)
            jobFinished = true;
        else throw new System.Exception("Le message de gamemode n'a pas pu être envoyé");
    }
}
