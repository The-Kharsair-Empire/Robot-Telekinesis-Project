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

###TODO:
- Integrate Big-Endian bytes decoder (.py script) into Unity C# to decode robot reading sent on TCP/IP socket
- Work out the correct values for VR Tracker Offset from the Tool Tip Tracker
- Smooth out the robot movement by removing the noise from the VR tracker readings
- Finish Implementing the Sphere Collider used to confine the VR tracker movement in order to prevent robot from moving to its pose limits and enter protective stop.