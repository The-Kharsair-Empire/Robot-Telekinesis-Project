# This project is undergoing
## Robot Telekinesis Project by Immersive Analytics Lab at Monash University
## Under GNU General Public License

-----
## The Aim of this project:
#### Create a VR-based collaborative robot control interface in which: 
#### 1) human user issuing gesture command from Virtual Reality environment. 
#### 2) the Real Collaborative Robot respond to the commands and perform desired actions. 
#### 3) Robot Reply with its sensor readings and human user is updated with the real-time information of the robot state.
#### And Explore its use under various scenarios


-----
## Where it can be useful:
#### - This project is suitable for use in human-robot collaborative working environment where tasks are too dangerous or
#### hazardous for human to perform. 
#### - We are still exploring the scenarios to apply such technology.

-----
## Relatives:
#### - Universal Robot UR3
#### - Unity-engine based Virtual Reality

-----

### TODO:

- Re-architecture the project and introduce an relay server (unityClient.cs that connects to relay server plus VirtualEndEffectorTrackeer.cs)
- Implement multithreading to allow interrupt current move execution and override the goal
- Integrate a UR3 model from UR developer forum
- Finish Implementing the Controller and Virtual Plane technique for manipulating the robot goal pose.

### Pending:
- Finish Implementing the Sphere Collider used to confine the VR tracker movement in order to prevent robot from moving to its pose limits and enter protective stop.
- Implement a Loop structure in URscript to allow re-connection to the Unity Host if the Host restart
- Verify whether the Rotation Conversion from Unity to URrobot is correct.
- Verify whether the value for VR Tracker Offset from the Tool Tip Tracker is correct.
- Adjust the distance param to trigger command that smooth out the robot movement.
