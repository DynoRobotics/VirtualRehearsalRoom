This section of the guide assumes that you have completed [[Setting up an OSC light]]
The light we set up in that guide will only be updated locally on the computer or headset that receives the signals over OSC. Let's fix that!

We start by copying one of the default scenes for VRR that already has all the necessary standard components for a multiplayer VRR scene.

![[Pasted image 20230529163249.png]]
![[Pasted image 20230529163706.png]]
Let's rename it to BlackBoxWIthMirrorAndSpotlight.
![[Pasted image 20230529163706.png]]

We want to use the light we created in [[Setting up a networked OSC light]] as a base for the networked light, so open the SingleOscSpotlight scene and save it as a prefab.
![[Pasted image 20230529164503.png]]
Open the BlackBoxWIthMirrorAndSpotlight scene and place the light prefab somewhere by the mirror and box.
![[Pasted image 20230529164902.png]]

Add the components Photon View, Photon Transform View and Light Synchronization to the lx1 GameObject.

![[Pasted image 20230529165744.png]]

Add the scene to the settings object and make sure the light is green.
![[Pasted image 20230529170158.png]]
Change the starting scene to the the scene we just added.
![[Pasted image 20230529170329.png]]

The Osc In component in our new scene defaults to 8000 (it was 7000 in the last scene) so let us change that setting in OSC/Pilot (or whatever you are using)
![[Pasted image 20230529170704.png]]

It is possible to test that the networking works locally on you computer by using project cloner to clone the Unity project and open the same project twice. Another option could be to build an apk and run in locally on your Quest headset.
![[OCSLightNetworking.gif]]

There is currently a limitation in the system that makes it so that only the fist player that enters the networked room is able to control the lights for everyone. Hopefully we will find a workaround for this that is easy to use and free of critical bugs at some point. This guide will be updated at that time.