using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseTracker : MonoBehaviour
{
    public RobotConnector robotConnector;

    public float interval = 0.05f;
    // Update is called once per frame

    public GameObject testObject;

    private float counter = 0;
    void Update()
    {
        counter += Time.deltaTime;
        if (counter > interval)
        {
            double[] tcp_pose = robotConnector.get_current_tcp();
            setTestObjectPose(tcp_pose);

            double x = transform.position.z;
            double y = -transform.position.x;
            double z = transform.position.y;

            robotConnector.movel(x, y, z, transform.rotation.x, transform.rotation.y, transform.rotation.z, 0.5, 0.1, 0, 0);
            //TODO: this rotation is 4-tuple quanterion (x, y, z, w), it is incorrect, use Euler angle and convert it to right-hand rotation coordinate used in UR robot

            counter = 0;
        }
        
    }

    private void setTestObjectPose(double[] tcp_pose)
    {
        
        testObject.transform.position = new Vector3((float)tcp_pose[0], (float)tcp_pose[1], (float)tcp_pose[2]);
        
    }
}
