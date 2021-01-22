using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCPTracker : MonoBehaviour
{
    public UnityServer unityServer;

    public RobotConnector robotConnector;

    public float interval = 0.005f;

    public GameObject boundryCollider;

    public GameObject VRtracker;

    public float trackerPos_x = 0f;

    public float trackerPos_y = 0f;

    public float trackerPos_z = 0f;

    public GameObject testObject;

    private float counter = 0;

    private void Start()
    {
        
    }


    void Update()
    {
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


        counter += Time.deltaTime;
        if (counter > interval && unityServer.connected)
        {

            unityServer.SendCommand(packCommand());
            //movel(false);
            double[] robot_joint_state = unityServer.RecvJointPositions();
            counter = 0;
        }
        else if (!unityServer.connected) Debug.Log("No Robot Connection");

        //diaplayTrackerPosInfo();

        // test();
    }

    private string packCommand()
    {
        double x = transform.position.z;
        double y = -transform.position.x;
        double z = transform.position.y;
        string pose_6_tuple = "(" + x + "," + y + "," + z + "," + transform.rotation.x + "," + transform.rotation.y + "," + transform.rotation.z + ")\n";
        return pose_6_tuple;
    }


    private void movel(bool useTracker)
    {
      
        double x = transform.position.z;
        double y = -transform.position.x;
        double z = transform.position.y;

        robotConnector.movel(x, y, z, transform.rotation.x, transform.rotation.y, transform.rotation.z, 0.5, 0.1, 0, 0);
        //TODO: this rotation is 4-tuple quanterion (x, y, z, w), it is incorrect, use Euler angle and convert it to right-hand rotation coordinate used in UR robot

        //counter = 0;
        
    }

    private void resetTrackerOffset()
    {

    }

    private void diaplayTrackerPosInfo()
    {
        trackerPos_x = VRtracker.transform.position.x;
        trackerPos_y = VRtracker.transform.position.y;
        trackerPos_z = VRtracker.transform.position.z;
    }
    private void test()
    {
        double[] tcp_pose = robotConnector.get_current_tcp();
        setTestObjectPose(tcp_pose);
    }

    private void setTestObjectPose(double[] tcp_pose)
    {

       testObject.transform.position = new Vector3((float)tcp_pose[0], (float)tcp_pose[1], (float)tcp_pose[2]);

    }
}
