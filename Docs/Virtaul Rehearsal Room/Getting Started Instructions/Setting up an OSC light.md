The OSC interface and spotlight in this example use the following assets from the Unity Asset Store.
https://assetstore.unity.com/packages/tools/network/osc-simpl-53710
https://assetstore.unity.com/packages/vfx/shaders/volumetric-light-beam-99888

Purchase the assets in the webstore and import them to the project using the package manager.
![[Pasted image 20230530102020.png]]
![[Pasted image 20230530102232.png]]
![[Pasted image 20230530102318.png]]
![[Pasted image 20230530102558.png]]
![[Pasted image 20230530103806.png]]
![[Pasted image 20230530103957.png]]
![[Pasted image 20230530104225.png]]
![[Pasted image 20230530104307.png]]

Now that we have the packages imported, we can import the VRR OSC integration without causing any errors.
![[Pasted image 20230530104509.png]]
![[Pasted image 20230530104542.png]]

Create a new URP scene and save it at Assets/Tutorials/OSC Light/Scenes/SingleOscSpotlight.

![[Pasted image 20230529133152.png]]

The default skybox is pretty bright and will make it hard to see the spotlight, so let's change it to be all black.
![[Pasted image 20230529133412.png]]
![[Pasted image 20230529133425.png]]

Now we add the light that we want to control using OSC. We will use "SD Beam/3D Beam and Spotlight" from Volumetric Light Beam as a base.

![[Pasted image 20230530105442.png]]

Rename the light to lx1, change the rotation to X=90 and move it up to position Y=2.0.
![[Pasted image 20230529133851.png]]

![[Pasted image 20230529133943.png]]

Next we add a new GameObject called OSC In to the scene. 
![[Pasted image 20230529134236.png]]
Add the component Osc In to the new GameObject and set Open On Awake to true.

![[Pasted image 20230530110447.png]]

Now go to the lx1 GameObject inspector and add OSC_LX.
![[Pasted image 20230529134909.png]]

Find Soft Intersections Blending Distances in the Volumetric Light Beam component and set them both to 0/off. (The light beam will not be visible in the camera otherwise)
![[Pasted image 20230529135319.png]]

Also, check the "Track changes during Playtime options". Nothing will happen visually to the light beam when you change the properties if this is not activated.
![[Pasted image 20230529142357.png]]

Press play in Unity and make sure you can see the light beam in the Game window.
![[Pasted image 20230529135419.png]]

It's time to connect an external OSC control system to the light and see what we can do with it!

I will be using a tool called OSC/Pilot in this guide for the sake of simplicity, but there are plenty of other tools you can use. We have also used Qlab and TouchDesigner with this project, for example.

Start by changing the port in OSC/Pilot from 7000 to match the OSC In component.
![[Pasted image 20230529140723.png]]

Now let's add our first slider! 

Just put the slider anywhere on the control surface for now, and use the OSC address `/unity/light/intensity/lx1`
Let OSC LOW be 0.0 and OSC HIGH be 1.0 (defaults).

It will control the intensity of the light while Unity is in play mode.
![[OCSLightIntensity.gif]]

Next up is pan/tilt.

Lets use a pad for this and map the outputs between 180 and -180 degrees for now.
Here we use the addresses  `/unity/light/pan/lx1` and `/unity/light/tilt/lx1`.
![[OCSLightPanTilt.gif]]

There are several additional controls available for the lights, but lets finish up by adding just one more set of controls. Colour!

![[OCSLightColour.gif]]

The addresses are `/unity/light/r/lx1`, `/unity/light/g/lx1` and `/unity/light/b/lx1`. The range should be between 0.0 and 1.0.

You can find the OSC/Pilot project from above at Assets/Tutorials/OSC Light/Etc/TutorialOSCPilotExample.

