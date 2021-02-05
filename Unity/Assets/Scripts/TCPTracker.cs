using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Threading.Tasks;

public class TCPTracker : MonoBehaviour
{
    public UnityServer unityServer;

    public RobotConnector robotConnector;

    public float interval = 0.05f;

    public GameObject boundryCollider;

    public GameObject VRtracker;

    public GameObject UR3;
    public GameObject[] joint_links;

    public float trackerPos_x = 0f;

    public float trackerPos_y = 0f;

    public float trackerPos_z = 0f;


    private float counter = 0;

    public Vector3 VRtracker_TCP_offset;
    public Vector3 desired_pos;
    public Quaternion desired_orientation_q;
    public Vector3 desired_orientation_e;
    public Vector3 tracker_pos;

    public Vector3 previous_tracker_pos;
    public Vector3 previous_tcp_pos;

    private bool get_offset_flag = true;

    private Dictionary<int, char> link_index_to_axis_mapper;
  
    void Start()
    {
        link_index_to_axis_mapper = new Dictionary<int, char>() {
            {0, 'y' },
            {1, 'x' },
            {2, 'y' },
            {3, 'x' },
            {4, 'y' },
            {5, 'z' }
        };
        //y, x, y, x, y, z
    }

    void Update()
    {

        if (get_offset_flag)
        {
            VRtracker_TCP_offset = transform.position - VRtracker.transform.position;
            previous_tracker_pos = VRtracker.transform.position;
           // previous_tcp_pos = transform.position;
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
        desired_orientation_q = VRtracker.transform.rotation;
        desired_orientation_e = VRtracker.transform.rotation.eulerAngles;
        if (actual_movement.magnitude > 0.05)
        {
            /*Vector3 actual_tcp_movement =  (VRtracker_TCP_offset + tracker_pos) - previous_tcp_pos;
            Vector3 wanted_tcp_movement = actual_tcp_movement / 10;
            desired_pos = previous_tcp_pos + wanted_tcp_movement;*/
            desired_pos = tracker_pos + VRtracker_TCP_offset;
            transform.position = desired_pos;
            StartCoroutine(CommRoutine());
            //new Thread(new ThreadStart(CommRoutine)).Start();

            previous_tracker_pos = tracker_pos;
           // previous_tcp_pos = desired_pos;
        }


        //transform.position = desired_pos;

        diaplayTrackerPosInfo();

    }

    private IEnumerator CommRoutine()
    {
        if (unityServer.connected)
        {

            unityServer.SendCommand(packCommand(desired_pos, desired_orientation_q));
            //movel(false);
            yield return null;
            double[] robot_joint_state = unityServer.Recv6Tuple();
            if (robot_joint_state != null)
            {
                //handle received joint state (double[6]) data

                var joint_state_in_deg = new float[robot_joint_state.Length];

                Parallel.For(0, robot_joint_state.Length, i => joint_state_in_deg[i] = rad2deg(robot_joint_state[i]));

                for (int i = 0; i < joint_state_in_deg.Length; i++)
                {
                    move(joint_links[i], i, joint_state_in_deg[i]);
                    yield return null;
                }
                //StartCoroutine(MoveRobotJointTo(joint_state_in_deg));
            }

           /* double[] robot_tcp_pose = unityServer.Recv6Tuple();
            if (robot_tcp_pose != null)
            {
                // handle recived tcp data
            }*/
 
        }
        else if (!unityServer.connected)
        {
            Debug.Log("No Robot Connection");

        }
        
    }

    private IEnumerator MoveRobotJointTo(float[] joint_state)
    {
        
        yield return new WaitForSeconds(1);
    }

    private void move(GameObject joint_link, int of_index, float to)
    {
        char move_which_axis;
        if (link_index_to_axis_mapper.TryGetValue(of_index, out move_which_axis))
        {
            Debug.Log("rotating joint of index: " + of_index + " along: " + move_which_axis + " axis, to degree: " + to);
            if (move_which_axis == 'x')
            {
                Debug.Log(move_which_axis);
                joint_link.transform.rotation = Quaternion.Euler(to, joint_link.transform.rotation.eulerAngles.y, joint_link.transform.rotation.eulerAngles.z);
            } else if (move_which_axis == 'y')
            {
                Debug.Log(move_which_axis);
                joint_link.transform.rotation = Quaternion.Euler(joint_link.transform.rotation.eulerAngles.x, to, joint_link.transform.rotation.eulerAngles.z);
            } else if (move_which_axis == 'z')
            {
                Debug.Log(move_which_axis);
                joint_link.transform.rotation = Quaternion.Euler(joint_link.transform.rotation.eulerAngles.x, joint_link.transform.rotation.eulerAngles.y, to);
            }
        }
        else
        {
            Debug.LogError("cannot find the joint to move !!!");
        }
    } 

    private float rad2deg(double rad)
    {
        Debug.Log("was: " + rad);
        Debug.Log("is " + (float)(180 * rad / Math.PI));
        return (float) ( 180 * rad / Math.PI);
        
    }

    private string packCommand(Vector3 desired_pos, Quaternion desired_orientation)
    {
        
        double x = desired_pos.z;
        double y = -desired_pos.x;
        double z = desired_pos.y;

        Vector3 axisAngle = Quaternion2axisAngle(desired_orientation);


        string pose_6_tuple = "(" + x + "," + y + "," + z + "," 
            + axisAngle.x + "," + axisAngle.y + "," + axisAngle.z + ")\n";
        return pose_6_tuple;
    }

    private string packCommand(Vector3 desired_pos, Vector3 desired_orientation)
    {

        double x = desired_pos.z;
        double y = -desired_pos.x;
        double z = desired_pos.y;

        Vector3 axisAngle = Eular2axisAngle(desired_orientation.x
            , desired_orientation.y
            , desired_orientation.z);

        string pose_6_tuple = "(" + x + "," + y + "," + z + "," 
            + axisAngle.x + "," + axisAngle.y + "," + axisAngle.z + ")\n";
        return pose_6_tuple;
    }

    private Vector3 Eular2axisAngle(double theta, double phi, double psi)
    {
        double c1 = Math.Cos(theta / 2);
        double s1 = Math.Sin(theta / 2);

        double c2 = Math.Cos(phi / 2);
        double s2 = Math.Sin(phi / 2);

        double c3 = Math.Cos(psi / 2);
        double s3 = Math.Sin(psi / 2);

        double w = c1 * c2 * c3 - s1 * s2 * s3;
        double x = c1 * c2 * s3 + s1 * s2 * c3;
        double y = s1 * c2 * c3 + c1 * s2 * s3;
        double z = c1 * s2 * c3 - s1 * c2 * s3;
        double  angle = 2 * Math.Acos(w);
        double norm = x * x + y * y + z * z;
        if (norm < 0.001)
        {
            x = 1;
            y = z = 0;
        }else
        {
            norm = Math.Sqrt(norm);
            x /= norm;
            y /= norm;
            z /= norm;
        }
 
        return new Vector3((float) z, (float) -x, (float) y) * (float) angle;
    }

    private Vector3 Quaternion2axisAngle(Quaternion orientation)
    {
        if (orientation.w > 1) orientation.Normalize();
        double angle = 2 * Math.Acos(orientation.w);
        double s = Math.Sqrt(1 - orientation.w * orientation.w);

        double x = orientation.x;
        double y = orientation.y;
        double z = orientation.z;

        if (s >= 0.001)
        {
            x /= s;
            y /= s;
            z /= s;
        }

        return new Vector3((float)z, (float)-x, (float)y) * (float)angle;
    }


    private void diaplayTrackerPosInfo()
    {
        trackerPos_x = tracker_pos.x;
        trackerPos_y = tracker_pos.y;
        trackerPos_z = tracker_pos.z;
    }



}
