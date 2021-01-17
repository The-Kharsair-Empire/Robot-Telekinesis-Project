using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class RobotConnector : MonoBehaviour
{

    public string robot_ip = "192.168.0.143";
    public int robot_port = 30002;
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
        byte[] data = System.Text.Encoding.ASCII.GetBytes(cmd);
        stream.Write(data, 0, data.Length);
    }

    public string sendAndRecv(string cmd)
    {
        byte[] data = System.Text.Encoding.ASCII.GetBytes(cmd);
        stream.Write(data, 0, data.Length);

        data = new byte[256];
        string response = string.Empty;
        int bytes = stream.Read(data, 0, data.Length);
        Debug.Log("number of bytes read: " + bytes);
        response = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
        Debug.Log("receive data response from robot: " + data);
        Debug.Log("receive string response from robot: " + response);
        
        return response;
    }

    public void movel(double x, double y, double z, double rx, double ry, double rz, double acc, double vel, double t, double r)
    {
        string cmd = string.Format("movel(p[{0}, {1}, {2}, {3}, {4}, {5}], {6}, {7}, {8}, {9})\n", x, y, z, rx, ry, rz, acc, vel, t, r);
        string res = sendAndRecv(cmd);
        Debug.Log("response from the robot: " + res);
    }

    public void get_actual_tcp_pose()
    {
        string cmd = "get_actual_tcp_pose()";
        string res = sendAndRecv(cmd);
        Debug.Log("response from the robot: " + res);
    }

    private void OnDestroy()
    {
        Debug.Log("Quiting Scene and Closing the Socket");
        finish();
        
    }

    void finish()
    {
        stream.Close();
        client_socket.Close();
    }


}
