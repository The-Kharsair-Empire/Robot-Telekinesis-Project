using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
public class TCPTracker : MonoBehaviour
{
    public UnityServer unityServer;

    public RobotConnector robotConnector;

    public float interval = 0.05f;

    public GameObject boundryCollider;

    public GameObject VRtracker;

    public float trackerPos_x = 0f;

    public float trackerPos_y = 0f;

    public float trackerPos_z = 0f;


    private float counter = 0;

    public Vector3 VRtracker_TCP_offset;
    public Vector3 desired_pos;
    public Vector3 tracker_pos;

    public Vector3 previous_tracker_pos;
    public Vector3 previous_tcp_pos;

    private bool get_offset_flag = true;

    


    void Update()
    {

        if (get_offset_flag)
        {
            VRtracker_TCP_offset = transform.position - VRtracker.transform.position;
            previous_tracker_pos = VRtracker.transform.position;
            previous_tcp_pos = transform.position;
            get_offset_flag = false;
        }
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.position = new Vector3(transform.position.x +0.01f, transform.position.y, transform.position.z);
            move();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position = new Vector3(transform.position.x -0.01f, transform.position.y, transform.position.z);
            move();
        }*/

        tracker_pos = VRtracker.transform.position;
        
        Vector3 actual_movement = tracker_pos - previous_tracker_pos;
        if (actual_movement.magnitude > 0.1)
        {
            Vector3 actual_tcp_movement =  (VRtracker_TCP_offset + tracker_pos) - previous_tcp_pos;
            Vector3 wanted_tcp_movement = actual_tcp_movement / 10;
            desired_pos = previous_tcp_pos + wanted_tcp_movement;
            transform.position = desired_pos;
            new Thread(new ParameterizedThreadStart(CommRoutine)).Start(desired_pos);

            previous_tracker_pos = tracker_pos;
            previous_tcp_pos = desired_pos;
        }


        //transform.position = desired_pos;

        diaplayTrackerPosInfo();

    }

    private void CommRoutine(object desired_goal)
    {
        if (unityServer.connected)
        {

            unityServer.SendCommand(packCommand(((Vector3) desired_goal)));
            //movel(false);
            double[] robot_joint_state = unityServer.Recv6Tuple();
            if (robot_joint_state != null)
            {
                //handle received joint state (double[6]) data
            }

            double[] robot_tcp_pose = unityServer.Recv6Tuple();
            if (robot_tcp_pose != null)
            {
                // handle recived tcp data
            }
            counter = 0;
        }
        else if (!unityServer.connected)
        {
            Debug.Log("No Robot Connection");

        }
    }

    private string packCommand(Vector3 desired_pos)
    {
        
        double x = desired_pos.z;
        double y = -desired_pos.x;
        double z = desired_pos.y;
 
        string pose_6_tuple = "(" + x + "," + y + "," + z + "," + 0 + "," + 0 + "," + 0 + ")\n";
        return pose_6_tuple;
    }




    private void resetTrackerOffset()
    {

    }

    private void diaplayTrackerPosInfo()
    {
        trackerPos_x = tracker_pos.x;
        trackerPos_y = tracker_pos.y;
        trackerPos_z = tracker_pos.z;
    }



}
