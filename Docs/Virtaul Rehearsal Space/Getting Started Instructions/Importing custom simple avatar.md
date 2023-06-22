Here we will go over how to create a working simple avatar from an .fbx with a head and body.

To make it easier to try this out, we have included an .fbx with a basic avatar in the project at Assets/VirtualRehearsalRoom/Tutorials/Art/SimpleAvatarExample.

![[Pasted image 20230529094225.png]]

Start by making a copy of the tutorial .fbx to Assets/NewProject/Art/Snowman.
![[Pasted image 20230529095152.png]]

![[Pasted image 20230529095330.png]]

Extract the materials from the .fbx so that we can change the colours of the avatar more easily.
![[Pasted image 20230529095737.png]]

The position and rotation of the head and the body matters and the easiest way to get them approximately correct is to use an existing SimpleAvatar prefab as a reference.
Make a folder called Assets/NewProject/SimpleAvatars and copy one of the existing SimpleAvatars prefabs there.
![[Pasted image 20230529101625.png]]
![[Pasted image 20230529101954.png]]
Unpack the prefab and rename it to Snowman.
![[Pasted image 20230529102050.png]]![[Pasted image 20230529114333.png]]

Drag and drop the .fbx you copied into the Art/Snowman folder before into to the new prefab called Snowman. Unpack the .fbx prefab.
![[Pasted image 20230529114900.png]]
Move the objects that should follow the head to the GameObject called Head, and to the same for Body.
![[Pasted image 20230529115253.png]]

Remove the hands and try to align the new head and body parts to the existing ones from the ghost avatar we used as reference.
![[Pasted image 20230529115525.png]]
![[Pasted image 20230529115636.png]]

Now we can remove the old face and body meshes.
![[Pasted image 20230529115749.png]]
All that is left to do in the prefab is to decide the colour of the animated hands. Select the top level game object in the Snowman prefab and to to the inspector. Assign the material you want to use for the hands to the Hands Material field. Assigning M_Avatar_Body from Art/Snowman/Materials will make the hands the same colour as the rest of the avatar for example.
![[Pasted image 20230529120911.png]]
![[Pasted image 20230529120946.png]]

Now we are ready to add our new avatar to the settings object and try it out in VR!
![[Pasted image 20230529121254.png]]
![[Pasted image 20230529131117.png]]