To connect the Xsens tracking suit to the Unity application, you will need MVN Animate with a Pro license.
The software can be downloaded from https://www.movella.com/support/software-documentation
Tutorials for setting up and calibrating the tracking suit for MVN Animate can be found here: https://tutorial.movella.com/

After the program is running with a calibrated suit and active licence, we need to tweak some settings in the network streamer.
![[Pasted image 20230523153654.png]]

To connect the suit to the Unity editor running on the same computer as MVN Animate make sure to use the following settings.![[Pasted image 20230523153951.png]]

We can also send the tracking data directly to a Quest headset that is running the application natively by changing the IP from localhost (127.0.0.1) to the IP of the headset we want to send the data to.

Start the login scene in VR by pressing Login Scene in Unity.
![[Pasted image 20230523154421.png]]

Select Full Body and press connect in VR.
![[Pasted image 20230523154711.png]]

The VR character should now be controlled by the Xsens tracking!
![[Pasted image 20230523155158.png]]