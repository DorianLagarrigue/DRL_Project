using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ML_StateWAction : ML_State
{
    private bool isMessageReceive;

    public ML_StateWAction() : base()
    {
        jobName = "Send enviro and Wait For Action";
    }

    public override ML_State OnEnd()
    {
        return new ML_StateResult();
    }

    public override void OnStart()
    {
        isMessageReceive = false;
    }

    public override void Update()
    {
        if (step < stepToUpdate)
        {
            step++;
            return;
        }

        byte[] message = onlineManager.CreateStateMessage();
        bool isSended = onlineManager.SendMessage(message);
        if (!isSended)
            throw new System.Exception("Le message de State n'a pas pu �tre envoy�");

        //wait action
        ML_Message msg;
        int[] action = new int[0];
        if (!isMessageReceive)
        {
            msg = onlineManager.Receive();
            isMessageReceive = msg.isReceive;
            if (msg.proto != ML_eProtocoleRec.eAction)
            {
                throw new System.Exception("Message re�u du mauvais protocole (Action est attendu), protocole re�u : " + msg.proto);
            }
            action = new int[msg.action.Length];
            msg.action.CopyTo(action, 0);
        }
        if (action.Length <= 0)
        {
            throw new System.Exception("Wtf l'action re�u est pas ouf");
        }

        //apply action
        bigObserverManager.DoActions(action);
        jobFinished = true;

    }
}
