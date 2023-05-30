You will need a couple of so called App ID's from photon in order for the networking and voice chat to work for your project.

Start by going to https://dashboard.photonengine.com/ and logging in. (If you don't already have an account you will need to create one.)

You need to create one App ID for something called PUN (general networking) and one for Voice.

After logging in, there should be a button ladled "CREATE A NEW APP".
![[Pasted image 20230523104019.png]]

Clicking that will take you to the following screen. Start by selecting Pun in the dropdown list and entering an application name.
![[Pasted image 20230523104951.png]]
Your new PUN app should now show up back in the dashboard view.
![[Pasted image 20230523105338.png]]
Click "CREATE NEW APP" again and create a Voice app for the project.
![[Pasted image 20230523105138.png]]![[Pasted image 20230523105517.png]]

Before we connect the apps to Unity, it is recommended that you configure them to only have one "Allowed Region". If we don't do this, people who connect to the app from different parts of the world will only be able to interact with those connected to the same region.

Start by pressing MANAGE on the PUN app.
![[Pasted image 20230523110426.png]]
Scroll down to the "Regions Allowlist" and edit the list.
![[Pasted image 20230523110824.png]]
Enter a single region of you choosing.
![[Pasted image 20230523110911.png]]
Repeat this for the Voice app and make sure both have the same single region in their respective allowlists.

Now it is time to copy over the app IDs to the settings object for your new project in Unity!
First highlight the settings object in the inspector using the Virtual Rehearsal Room Window or by clicking on it in the Unity Project explorer.
![[Pasted image 20230523112037.png]]
![[Pasted image 20230523112239.png]]

Click on the text for the App ID on the PUN app in the web dashboard to reveal the full ID.
![[Pasted image 20230523113101.png]]
Now copy the ID to the settings object in Unity.
![[Pasted image 20230523113225.png]]
Repeat the same steps for the Voice app.
![[Pasted image 20230523113400.png]]

You most likely need to force an update of the settings for them to take effect. That is done by pressing "Force Update Settings" in the Virtual Rehearsal Room Window.
![[Pasted image 20230523113935.png]]
To verify that the new network settings are in use, highlight the Photon Server Settings and check that the correct App IDs have been set there.
![[Pasted image 20230523114451.png]]
![[Pasted image 20230523114626.png]]

The reason we don't put the App IDs directly into the Photon Server Settings object is mainly so that we can work on multiple projects/shows with different App IDs in the same Unity project and quickly switch between them.

That's it, networking should now be configured for your project!

