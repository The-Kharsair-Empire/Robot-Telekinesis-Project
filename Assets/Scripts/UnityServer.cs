using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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

    private void startServer() {
        Thread t = new Thread(new ThreadStart(startServerRoutine));
        t.Start();

    }


    private void startServerRoutine()
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
            byte[] data = Encoding.ASCII.GetBytes(pose_6_tuple);
            stream.Write(data, 0, data.Length);
       
        } else
        {
            connected = false;
            startServer(); // restart Server if connection is broken
        }
    }

    public double[] RecvJointPositions()
    {
        if (robotClient.Client.Connected)
        {
            byte[] res = new byte[1024];
            int bytes = stream.Read(res, 0, res.Length);
            string response = Encoding.ASCII.GetString(res, 0, bytes);
            Debug.Log(response);
            response = response.Trim(new char[]{'[', ']'});
            Debug.Log(response);
            string[] each_joint_pose_string = response.Split(',');
            double[] joint_pose_result = new double[6];
            int i = 0;
            foreach (var each in each_joint_pose_string)
            {
                joint_pose_result[i] = double.Parse(each);
                Debug.Log(joint_pose_result[i]);
                i++;
            }
            return joint_pose_result;
            
            
        } else
        {
            connected = false;
            startServer();
        }
        return null;
       
    }

    private void OnDestroy()
    {
        Debug.Log("Quiting Scene and Closing the Socket");
        finish();

    }

    private void finish()
    {
        stream.Close();
        robotClient.Close();
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
