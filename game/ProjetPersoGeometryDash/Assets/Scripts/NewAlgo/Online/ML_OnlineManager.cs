using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public enum ML_eGameMode
{
    eLearn = 0,
    eLearnFromLoad = 1,
    ePractice = 2,
}
public enum ML_eProtocoleSend
{
    eGameMode = 0, //float gamemode
    eState = 1, // float acceleration, float offset, float[80] enviro
    eNextState = 2, //float reward, float acceleration, float offset, float[80] enviro
    eReadyToStart = 3,
}
public enum ML_eProtocoleRec
{

    eStart = 0, //Call start the level
    eAction = 1, // int (action choisi) choisis quelle action effectué
    eNextStep = 2, //la partie n'est pas terminé, indique que l'on attend l'état (on recommence la boucle interne)
    eNewLoop = 3, // la partie est terminé (héro mort ou win), indique que l'on recommence une partie
    eStop = 4 //indique la fin de l'expérience

}

public struct ML_Message
{
    public ML_Message(ML_eProtocoleRec _proto, int[] _action)
    {
        proto = _proto;
        if(_action != null)
        {
            action = new int[_action.Length];
            _action.CopyTo(action, 0);
        }
        else
        {
            action = new int[0];
        }
        isReceive = false;
    }
    public bool isReceive;
    public ML_eProtocoleRec proto;
    public int[] action;
}


public class ML_OnlineManager : MonoBehaviour
{

    [SerializeField] ML_Manager MLManager;



    public bool isConnecting = false;
    public bool isListening;
    SocketAPI socketAPI;
    public ML_BigObserverManager observer;
    public string connectionIP = "localhost";
    public int connectionPort = 666;
    public bool isConnected;


    public void InitConnection()
    {
        //Debug.Log("Connection bloqué pour éviter des bêtises");
        //return;

        Debug.Log(isConnecting);

        if (isConnecting) return;
        socketAPI = new SocketAPI();
        socketAPI.log = Debug.Log;
        socketAPI.Init(connectionIP, connectionPort);
        socketAPI.Host();
    }



    public ML_Message Receive()
    {

        bool isReceived = false;
        ML_Message msg = new ML_Message();

        while (!isReceived)
        {
            byte[] byte_array = socketAPI.Receive();
            if (byte_array == null) continue;

            MemoryStream ms = new MemoryStream(byte_array);
            BinaryReader br = new BinaryReader(ms);
            int flag = br.ReadChar();
            isReceived = true;

            msg = new ML_Message((ML_eProtocoleRec)flag, null);
            if (flag == (int)ML_eProtocoleRec.eAction)
            {
                List<int> actions = new List<int>();

                int leng = observer.GetAgentCount();

                for(int i = 0; i < leng; i++)
                    actions.Add(br.ReadChar());

                msg.action = actions.ToArray();
                
            }
            msg.isReceive = true;
            ms.Close();
        }
        return msg;
    }

    public bool SendMessage(byte[] message)
    {
        if (message.Length > 0)
            return socketAPI.Send(message);
        return false;
    }
    public byte[] CreateGameModeMessage()
    {
        float flag = (float)ML_eProtocoleSend.eGameMode;
        MemoryStream ms = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(ms);
        
        bw.Write(flag);
        bw.Write((float)MLManager.gamemode);
        bw.Write((float)observer.GetAgentCount());
        
        bw.Write((float)MLManager.GetNbEpisode());
        bw.Write((float)MLManager.GetFreqTest());


        ML_ModelParameters[] models = observer.GetModelsParameters();

        foreach(ML_ModelParameters model in models)
        {
            bw.Write((float)model.actionShape);
            bw.Write((float)model.enviroShape);

            bw.Write(model.learningRate);
            bw.Write(model.discountFactor);

            bw.Write(model.memorySize);
            bw.Write(model.batchSize);

            foreach(int l in model.layers)
            {
                bw.Write((float)l);
            }
        }


        byte[] buffer = ms.ToArray();
        bw.Close();
        return buffer;

    }
    public byte[] CreateStateMessage()//enviro, isAiControl, isAlreadyDone
    {

        float flag = (float)ML_eProtocoleSend.eState;
        ML_StateStruct[] allStates = observer.GetAllStates();

        MemoryStream ms = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(ms);


        bw.Write(flag);

        foreach (ML_StateStruct state in allStates)
        {
            foreach (float n in state.state)
            {
                bw.Write(n);
            }
            if (state.isAiControl)
                bw.Write(1f);
            else
                bw.Write(0f);
        }


        
        byte[] buffer = ms.ToArray();
        bw.Close();
        return buffer;
    }
    public byte[] CreateNextStateMesssage()//reward, enviro, isAicontrol, isDone, isAlreadyDone
    {
        float flag = (float)ML_eProtocoleSend.eNextState;

        ML_StateStruct[] allStates = observer.GetAllStates();


        MemoryStream ms = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write(flag);




        foreach (ML_StateStruct state in allStates)
        {
            bw.Write(state.reward);


            foreach (float n in state.state)
            {
                bw.Write(n);
            }
            if (state.isAiControl)
                bw.Write(1f);
            else
                bw.Write(0f);

            if (state.isDone)
                bw.Write(1f);
            else
                bw.Write(0f);
        }


        byte[] buffer = ms.ToArray();
        bw.Close();
        return buffer;
    }
    public byte[] CreateReadyToStartMessage()
    {
        float flag = (float)ML_eProtocoleSend.eReadyToStart;


        MemoryStream ms = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(ms);
        bw.Write(flag);
        byte[] buffer = ms.ToArray();
        bw.Close();
        return buffer;
    }



}
