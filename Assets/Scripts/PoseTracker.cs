using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseTracker : MonoBehaviour
{
    public RobotConnector robotConnector;

    public float interval = 0.05f;
    // Update is called once per frame

    private float counter = 0;
    void Update()
    {
        counter += Time.deltaTime;
        if (counter > interval)
        {
            double x = transform.position.z;
            double y = -transform.position.x;
            double z = transform.position.y;

            robotConnector.movel(x, y, z, transform.rotation.x, transform.rotation.y, transform.rotation.z, 0.5, 0.1, 0, 0);

            counter = 0;
        }
        
    }
}
