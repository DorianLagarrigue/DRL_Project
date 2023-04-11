using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class SocketAPI
{
    Socket socket;
    public Socket sockClient;
    bool isHost;
    private int port;
    private string adress;
    public Socket Sock { get => isHost?sockClient:socket;  }

    public void Init(string _adress, int _port)
    {
        sockClient = null;
        //create the socket with IP stream TCP
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        port = _port;
        adress = _adress;
    }

    public bool Send(byte[] b)
    {
        if (Sock == null) return false;
        Sock.Send(b);
        return true;
    }

    public byte[] Receive()
    {

        if (Sock != null && Sock.Available > 0)
        {
            Sock.ReceiveTimeout = 100;
            byte[] buffer = new byte[Sock.Available];
            int size = Sock.Receive(buffer);
            Array.Resize(ref buffer, size);
            return buffer;
        }

        return null;
    }

    public void Host()
    {
        isHost = true;
        socket.Bind(ComputeEndPoint(adress, port));
        socket.Listen(30); // toujours mettre ça, c'est obscur

        var tokenSrc = new CancellationTokenSource();
        CancellationToken ct = tokenSrc.Token;
        var t = Task.Run(() =>
        {
            Thread.Sleep(10000);
            if (!ct.IsCancellationRequested)
                socket.Close();
        });


        sockClient = socket.Accept(); //sockclient est un bébé socket créer par socket


        /*your blocking code*/
        tokenSrc.Cancel();
    }
    

    public void Join()
    {
        isHost = false;
        socket.Connect(adress, port);
    }

    public delegate void LogDelegate(string message);
    public LogDelegate log;

 

    public EndPoint ComputeEndPoint(string host, int port)
    {
        IPAddress ipAddress = IPAddress.None;
        if (host.Length == 0)
            host = Dns.GetHostName();

        bool found = false;
        try
        {
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            foreach (var ip in ipHost.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = ip;
                    found = true;
                    break;
                }
            }
        }
        catch (Exception) { }
        //fallback
        if (!found)
            ipAddress = IPAddress.Parse(host);

        log("Address resolved : " + ipAddress);
        return new IPEndPoint(ipAddress, port);
    }

    private void Log(string v)
    {
        throw new NotImplementedException();
    }
}
