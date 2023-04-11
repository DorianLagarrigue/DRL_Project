using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ML_StateWStart : ML_State
{
    public bool isMessageReceive;
    public ML_StateWStart() : base()
    {
        jobName = "Wait for Start";
    }

    public override ML_State OnEnd()
    {
        manager.currentEpisode++;
        return new ML_StateWAction();
    }

    public override void OnStart()
    {
        isMessageReceive = false;
    }

    public override void Update()
    {
        ML_Message msg;
        if (!isMessageReceive)
        {
            msg = onlineManager.Receive();
            isMessageReceive = msg.isReceive;
            if (msg.proto != ML_eProtocoleRec.eStart)
            {
                throw new System.Exception("Message reçu du mauvais protocole (Start est attendu), protocole reçu : " + msg.proto);
            }
        }
        manager.StartPlayingFunction.Invoke();
        jobFinished = true;
    }
}
