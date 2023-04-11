using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ML_StateResult : ML_State
{
    bool isMessageReceive;
    bool isNewLoop;
    bool isStop;
    public ML_StateResult() : base()
    {
        jobName = "Action and result";
    }

    public override ML_State OnEnd()
    {
        if (isNewLoop)
            return new ML_StateRestarting();
        if (isStop)
            return null;

        return new ML_StateWAction();
    }

    public override void OnStart()
    {
        isMessageReceive = false;
        isNewLoop = false;
        isStop = false;
        jobFinished = false;
    }

    public override void Update()
    {
        byte[] message = onlineManager.CreateNextStateMesssage();
        bool isSended = onlineManager.SendMessage(message);
        if (!isSended)
            throw new System.Exception("Le message de State n'a pas pu être envoyé");

        //Wait Calcul
        ML_Message msg;
        if (!isMessageReceive)
        {
            msg = onlineManager.Receive();
            isMessageReceive = msg.isReceive;
            if (msg.proto != ML_eProtocoleRec.eNextStep && msg.proto != ML_eProtocoleRec.eNewLoop && msg.proto != ML_eProtocoleRec.eStop)
            {
                throw new System.Exception("Message reçu du mauvais protocole (NextStep ou NewLoop ou stop attendu), protocole reçu : " + msg.proto);
            }

            if (msg.proto == ML_eProtocoleRec.eNewLoop)
            {
                isNewLoop = true;
            }
            if (msg.proto == ML_eProtocoleRec.eStop)
                isStop = true;

            jobFinished = true;

        }
    }
}
