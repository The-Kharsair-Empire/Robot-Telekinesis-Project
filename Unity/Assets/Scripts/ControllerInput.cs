using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerInput : MonoBehaviour
{

    public SteamVR_Action_Boolean touched; 

    public GameObject controller;

    private GameObject virtual_plane_on_controller;
    private GameObject virtual_plane_on_tcp;

    public GameObject virtualPlanePrefab;


    public GameObject virtual_end_effector;

    public UnityClient unity_client;

 
    // Update is called once per frame
    void Update()
    {
   
        if (touched.GetStateDown(SteamVR_Input_Sources.Any))
        {
            virtual_plane_on_controller = Instantiate(virtualPlanePrefab, controller.transform.position, Quaternion.identity);
            virtual_plane_on_tcp = Instantiate(virtualPlanePrefab, virtual_end_effector.transform.position, controller.transform.rotation);
            unity_client.activate(virtual_plane_on_tcp);
            Debug.Log("touch pad touched");
        } else if (touched.GetStateUp(SteamVR_Input_Sources.Any))
        {
            Destroy(virtual_plane_on_controller);
            Destroy(virtual_plane_on_tcp);
            Debug.Log("touch pad untouched");
        }
        
        if (virtual_plane_on_controller != null)
        {
            virtual_plane_on_controller.transform.position = controller.transform.position;
            virtual_plane_on_controller.transform.rotation = controller.transform.rotation;
            unity_client.interact(virtual_plane_on_tcp);
        }
    }
}
