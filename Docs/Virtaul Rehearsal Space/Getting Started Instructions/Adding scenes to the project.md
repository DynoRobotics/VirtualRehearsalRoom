Every VRR project has a number of scenes associated with it. After logging in, the avatars will show up in the "Starting Scene" and it will be possible for non audience member to change between the other available scenes after that.

There is a Unity Prefab called [VRR Managers] that needs to be in the scene for everything to work. The easiest way to make a new custom scene is probably by duplicating one of the default scenes. You can do this by selecting it in the project view and pressing Ctrl-D.

![[Pasted image 20230522163602.png]]
![[Pasted image 20230522164727.png]]

You now should add the new scene to the new settings object. A fast way to highlight the active settings object is to use the "Highlight Active Settings" button in the "Virtual Rehearsal Room" window.
![[Pasted image 20230522165240.png]]

Navigate to the Scenes property and press the plus button.
![[Pasted image 20230523115500.png]]
Drag and drop the new scene into the field and press the Add button.
![[Pasted image 20230523120503.png]]
Press Add as Enabled in the popup.
![[Pasted image 20230523120530.png]]
The indicator light should now be green on for the scene, indicating that it has been added to the build and can be loaded by the the networking system.
![[Pasted image 20230523121138.png]]