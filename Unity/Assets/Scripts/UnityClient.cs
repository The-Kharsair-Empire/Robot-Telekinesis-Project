using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Web;
using System;
using System.Threading;
using System.Threading.Tasks;

public class UnityClient : MonoBehaviour
{
    
    private TcpClient client;
    private NetworkStream stream;

    private StreamReader inChannel;
    private StreamWriter outChannel;

    //private GameObject End_effector_virtual_plane;

    private List<Action> trajectoryQueue;
    private Dictionary<int, char> link_index_to_axis_mapper;

    public string host_ip = "localhost";
    public int host_port = 27;

    public GameObject VRController;
    public GameObject Virtual_end_effector;
    public GameObject Movement_indicator; //assigned a cube of different color to Virtual_end_effector


    public GameObject UR3;
    public GameObject[] joint_links;

    //display zone;
    public float trackerPos_x = 0f;
    public float trackerPos_y = 0f;
    public float trackerPos_z = 0f;
    public Vector3 controller_endEffector_offset;
    public Vector3 desired_pos;
    public Quaternion desired_orientation_q;
    public Vector3 desired_orientation_e;
    public Vector3 controller_pos;

    public Vector3 previous_controller_pos;
    public Vector3 previous_tcp_pos;
    public Quaternion previous_ee_orientation;

    

    // Start is called before the first frame update
    void Start()
    {
        trajectoryQueue = new List<Action>();

        link_index_to_axis_mapper = new Dictionary<int, char>() {
            {0, 'y' },
            {1, 'x' },
            {2, 'y' },
            {3, 'x' },
            {4, 'y' },
            {5, 'z' }
        };
        //y, x, y, x, y, z

        Debug.Log("Wait for host to response");

        client = new TcpClient(host_ip, host_port);
        Debug.Log("Connected to relay server");
        stream = client.GetStream();
        inChannel = new StreamReader(client.GetStream());
        outChannel = new StreamWriter(client.GetStream());

        new Thread(new ThreadStart(recvJointStateLoop)).Start();
        StartCoroutine(executeJointTrajectory());
    }

    public void activate(GameObject virtual_plane_on_tcp)
    {
        controller_endEffector_offset = Virtual_end_effector.transform.position - VRController.transform.position;
        previous_controller_pos = VRController.transform.position;
        //End_effector_virtual_plane = virtual_plane_on_tcp;
        previous_ee_orientation = virtual_plane_on_tcp.transform.rotation;

        desired_pos = virtual_plane_on_tcp.transform.position;
        Virtual_end_effector.transform.position = desired_pos;
        //Virtual_end_effector.transform.rotation = End_effector_virtual_plane.transform.rotation;

       /* desired_orientation_q =  Virtual_end_effector.transform.rotation * Quaternion.identity;
        Virtual_end_effector.transform.rotation = desired_orientation_q;*/
    }

    public void interact(GameObject virtual_plane_on_tcp)
    {

        virtual_plane_on_tcp.transform.position = VRController.transform.position + controller_endEffector_offset;
        virtual_plane_on_tcp.transform.rotation = VRController.transform.rotation;
        controller_pos = VRController.transform.position;
        Vector3 actual_movement = controller_pos - previous_controller_pos;
        Quaternion rotation_diff = virtual_plane_on_tcp.transform.rotation * Quaternion.Inverse(previous_ee_orientation) ;
        
        desired_orientation_q = rotation_diff * Virtual_end_effector.transform.rotation;
        desired_orientation_e = virtual_plane_on_tcp.transform.rotation.eulerAngles;
        desired_pos = virtual_plane_on_tcp.transform.position;

        Virtual_end_effector.transform.position = desired_pos;
        Virtual_end_effector.transform.rotation = desired_orientation_q;
    
        //Virtual_end_effector.transform.rotation = desired_orientation_q;

        if (actual_movement.magnitude > 0.01) //0.01, 0.008
        {
            /*Vector3 actual_tcp_movement =  (VRtracker_TCP_offset + tracker_pos) - previous_tcp_pos;
            Vector3 wanted_tcp_movement = actual_tcp_movement / 10;
            desired_pos = previous_tcp_pos + wanted_tcp_movement;*/
   
            Movement_indicator.transform.position = desired_pos;
            Movement_indicator.transform.rotation = desired_orientation_q;

            string cmd = packCommand(desired_pos, desired_orientation_q);
            //Debug.Log("pos sent to relay server: " + cmd);
            outChannel.Write(cmd);
            outChannel.Flush();

            previous_controller_pos = controller_pos;
            // previous_tcp_pos = desired_pos;

        }
        previous_ee_orientation = virtual_plane_on_tcp.transform.rotation;

        //transform.position = desired_pos;

        diaplayControllerPosInfo();
    }



    private IEnumerator executeJointTrajectory()
    {
        while (true)
        {
            while (trajectoryQueue.Count > 0)
            {
                Action executeOneJointState = trajectoryQueue[0];
                trajectoryQueue.RemoveAt(0);

                //Debug.Log("Trajectory Execute");
                executeOneJointState();
                //yield return null;
            }
            yield return null;
        }
        

    }


    // Update is called once per frame
    private void recvJointStateLoop()
    {
        while (true)
        {
            float[] res = Recv6Tuple();
          /*  res[2] += 263.208458;
            res[3] += 83.968;*/
            //Debug.Log("JOINT STATE RECV");
           

            Action executeOneJointState = () =>
            {
                for (int i = 0; i < res.Length; i++)
                {
                    move(joint_links[i], i,  res[i]);
                }
            };

            trajectoryQueue.Add(executeOneJointState);
        }

    }

    void OnDestroy()
    {
        inChannel.Close();
        outChannel.Close();
        stream.Close();
        client.Close();
        Debug.Log("Client close");
    }

    private float[] Recv6Tuple()
    {
        string res = inChannel.ReadLine();
        //res = HttpUtility.UrlDecode(res, Encoding.UTF8);
        print("Res was " + res);
       // Debug.Log(res);
        res = res.Trim(new char[] { '[', ']', 'p' });
        //Debug.Log(res);
        string[] each_double = res.Split(',');
        float[] pose_result = new float[6];
        int i = 0;
        foreach (var each in each_double)
        {
            /*print("Parsing");
            print("each was  "  + each);*/
            pose_result[i] = float.Parse(each);
            //print("pose result was " +pose_result[i]);
            pose_result[i] = rad2deg(pose_result[i]);// no need if return 6 tuple has p[]
           // print("Pose result is " + pose_result[i]);
            //Debug.Log(pose_result[i]);
            i++;
        }
        return pose_result;

    }

    private void move(GameObject joint_link, int of_index, float to)
    {
        char move_which_axis;
        if (link_index_to_axis_mapper.TryGetValue(of_index, out move_which_axis))
        {
           // Debug.Log("rotating joint of index: " + of_index + " along: " + move_which_axis + " axis, to degree: " + to);
            if (move_which_axis == 'x')
            {
                //Debug.Log(move_which_axis);
                print("x Was " + joint_link.transform.localRotation.eulerAngles.x);
                joint_link.transform.localRotation = Quaternion.Euler(to, joint_link.transform.localRotation.eulerAngles.y, joint_link.transform.localRotation.eulerAngles.z);
                print("x is " + joint_link.transform.localRotation.eulerAngles.x);
            }
            else if (move_which_axis == 'y')
            {
                //Debug.Log(move_which_axis);
                joint_link.transform.localRotation = Quaternion.Euler(joint_link.transform.localRotation.eulerAngles.x, to, joint_link.transform.localRotation.eulerAngles.z);
            }
            else if (move_which_axis == 'z')
            {
                //Debug.Log(move_which_axis);
                joint_link.transform.localRotation = Quaternion.Euler(joint_link.transform.localRotation.eulerAngles.x, joint_link.transform.localRotation.eulerAngles.y, to);
            }
        }
        else
        {
            Debug.LogError("cannot find the joint to move !!!");
        }
    }

    private string packCommand(Vector3 desired_pos, Quaternion desired_orientation)
    {

        Vector3 FLU = Unity2RobotCoor(desired_pos);

        Vector3 axisAngle = Quaternion2axisAngle(desired_orientation);


        string pose_6_tuple = "(" + FLU.x + "," + FLU.y + "," + FLU.z + ","
            + axisAngle.x + "," + axisAngle.y + "," + axisAngle.z + ")";
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
    private float rad2deg(double rad)
    {
    /*    Debug.Log("was: " + rad);
        Debug.Log("is " + (float)(180 * rad / Math.PI));*/
        return (float)(180 * rad / Math.PI);

    }


    private Vector3 Robot2UnityCoor(Vector3 FLU) //FLU 2 RUF
    {

        return new Vector3(-FLU.y, FLU.z ,FLU.x);
    }

    private Vector3 Unity2RobotCoor(Vector3 RUF) //RUF 2 FLU
    {
        return new Vector3(RUF.z, -RUF.x, RUF.y);
    }

    private Vector3 Eular2axisAngle(double theta, double phi, double psi) //RUF 2 FLU conversion is done
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
        double angle = 2 * Math.Acos(w);
        double norm = x * x + y * y + z * z;
        if (norm < 0.001)
        {
            x = 1;
            y = z = 0;
        }
        else
        {
            norm = Math.Sqrt(norm);
            x /= norm;
            y /= norm;
            z /= norm;
        }

        return new Vector3((float)z, (float)-x, (float)y) * (float)angle;
    }

    private Vector3 Quaternion2axisAngle(Quaternion orientation) //RUF 2 FLU conversion is done
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
        Vector3 axisAngle = new Vector3((float)z, (float)-x, (float)y) * (float)angle;
        //Debug.Log("orientation conversion result: " + axisAngle);
        //should this be converted to radian?
        return axisAngle;
    }


    private void diaplayControllerPosInfo()
    {
        trackerPos_x = controller_pos.x;
        trackerPos_y = controller_pos.y;
        trackerPos_z = controller_pos.z;
    }
}
