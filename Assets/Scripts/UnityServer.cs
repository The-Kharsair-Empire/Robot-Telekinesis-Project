using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UnityServer : MonoBehaviour
{
    public string HOST_IP = "192.168.0.192";
    public int HOST_PORT = 21;

    private TcpClient robotClient;
    private TcpListener tcpListener;
    private NetworkStream stream;
    public bool connected = false;

    // Start is called before the first frame update
    void Start()
    {
        IPAddress ip = IPAddress.Parse(HOST_IP);
        tcpListener = new TcpListener(ip, HOST_PORT);
        tcpListener.Start();
        startServer();
    }


    private void startServer()
    {
        
        robotClient = tcpListener.AcceptTcpClient();
        connected = robotClient.Client.Connected;
        stream = robotClient.GetStream();
        byte[] greetings = new byte[1024];
        int greetingLength = stream.Read(greetings, 0, greetings.Length);
        Debug.Log(Encoding.ASCII.GetString(greetings, 0, greetingLength));
      
    }

    public void SendCommand(string pose_6_tuple)
    {
        if (robotClient.Client.Connected)
        {
            Debug.Log("Sending Command");
        } else
        {
            connected = false;
            startServer(); // restart Server if connection is broken
        }
    }

    // Update is called once per frame
  /*  void Update()
    {
        

        while (robotClient.Client.Connected)
        {

        }
        Debug.Log("Updating");
        
    }*/
}
