using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System;


public class RobotConnector : MonoBehaviour
{

    public string robot_ip = "192.168.0.143";
    public int robot_port = 30003;
    TcpClient client_socket;
    NetworkStream stream;


    // Start is called before the first frame update
    void Start()
    {
        client_socket = new TcpClient(robot_ip, robot_port);
        stream = client_socket.GetStream();
        //get_actual_tcp_pose();

        //movel(0.27598, -0.00856, 0.08658, 1.901, -2.635, 2.397, 0.5, 0.1, 0, 0);

        //finish();

    }


    public void sendCommand(string cmd)
    {
        byte[] data = Encoding.ASCII.GetBytes(cmd);
        stream.Write(data, 0, data.Length);
    }

    public string sendAndRecv(string cmd)
    {
        Debug.Log("Command Sent: " + cmd);
        byte[] data = Encoding.ASCII.GetBytes(cmd);
        stream.Write(data, 0, data.Length);

        data = new byte[20480] ;
        string response = string.Empty;
        int bytes = stream.Read(data, 0, data.Length);
        Debug.Log("number of bytes read: " + bytes);
        Debug.Log("raw bytes info: " + data);
        if (bytes > 0) {
            response = Encoding.BigEndianUnicode.GetString(data, 0, bytes);
            Debug.Log("decoded byte info: " + response);
            Debug.Log("String Length: " + response.Length);
        }
        return response;
    }

    public void movel(double x, double y, double z, double rx, double ry, double rz, double acc, double vel, double t, double r)
    {
        string cmd = string.Format("movel(p[{0}, {1}, {2}, {3}, {4}, {5}], {6}, {7}, {8}, {9})\n", x, y, z, rx, ry, rz, acc, vel, t, r);
        sendCommand(cmd);
     
    }

    private double[] RecvDoubleValFromRobot(int startIndex, int takeLength)
    {
        byte[] data = new byte[1108];
        int bytes = stream.Read(data, 0, data.Length);

        byte[] slice = new byte[takeLength];

        for (int i = 0; i < takeLength; i++)
            slice[i] = data[startIndex + i];
  

        byte[] lEndian = new byte[takeLength];
        for (int i = 0; i < takeLength; i += 8)
        {
            for (int j = 7; j >= 0; j--)
                lEndian[i + (7 - j)] = slice[i + j];
        }
            
        double[] values = new double[takeLength / 8];

        for (int i = 0; i < values.Length; i++)
            values[i] = BitConverter.ToDouble(lEndian, i * 8);

        return values;
    }

    public double[] get_current_tcp()
    {
        double[] tcp_pose = RecvDoubleValFromRobot(444, 48);
        print(tcp_pose[0] + ",  " + tcp_pose[1] + ",  " + tcp_pose[2] + ",  " + tcp_pose[3] + ",  " + tcp_pose[4] + ",  " + tcp_pose[5]);
        return tcp_pose;

    }

    private void OnDestroy()
    {
        Debug.Log("Quiting Scene and Closing the Socket");
        finish();
        
    }
/*
    public void FLU()
    {

    }*/

    void finish()
    {
        stream.Close();
        client_socket.Close();
    }


}
