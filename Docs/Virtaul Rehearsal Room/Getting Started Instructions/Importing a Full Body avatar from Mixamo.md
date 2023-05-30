![[Pasted image 20230523164342.png]]We have tried to make it as easy as possible to use characters from https://www.mixamo.com/ for Full Body tracking, but there is one tweak that is needed for them to work well as a VR character.
We need to make sure that the head is invisible for the person that is controlling the character. To make this possible we need the mesh for the head to be separated from the mesh for the rest of the body. This is unfortunately not the case out of the boc for many Maximo characters.

Splitting the mesh up is a relatively simple operation in 3d programs like Blender (https://www.blender.org/download/). I will give a quick example of how to do it in Blender here.

Download the Claire avatar from Mixamo.

![[Pasted image 20230523160927.png]]
![[Pasted image 20230523161114.png]]

Create a folder called Art/Claire under NewProject and place the downloaded .fbx file there.
![[Pasted image 20230523161342.png]]

Open Blender, delete the starting cube, camera and light and import the .fbx file of Claire.
![[Pasted image 20230523161759.png]]

First, hide all meshes except the Girl_Body_Geo mesh and go into Modelling/Edit Mode.
We will first use the Bisect tool with the whole mesh selected to split it in 2 parts at the neck. After we select either just the head of just be body and then press P and separate by selection. (Selecting just the body is much easier in this case)
See this video for a more detailed explanation https://www.youtube.com/watch?v=fVOYv8HdMxI&ab_channel=PIXXO3D
![[Pasted image 20230523162353.png]]
After the split, it should look something like this.
![[Pasted image 20230523164350.png]]
Rename the new Girl_Body_Geo.001 mesh to Girl_Head_Geo.

Try hiding the head to make sure you managed to do the split properly.
![[Pasted image 20230523165800.png]]

Now it's time to export the .fbx back to Unity.
Make sure you delete the camera and light before you do the export.
![[Pasted image 20230523170346.png]]
Save it as a new file, don't overwrite the old one.
![[Pasted image 20230523170523.png]]
Only the original .fbx will have the embedded textures, so lets extract them into a folder called Textures so our new edited avatar can use them.
![[Pasted image 20230523170748.png]]

There clearly an issue with some of the materials on the new version so lets extract them and see if we can fix the problem.
![[Pasted image 20230523171731.png]]
![[Pasted image 20230523171914.png]]

Select the Brows, Eyes and Mouth materials and change the surface type from Opaque to Transparent.
![[Pasted image 20230523172111.png]]
![[Pasted image 20230523172426.png]]
In order for the body tracking to work, the character needs to be rigged as a humanoid.
![[Pasted image 20230523174532.png]]
Now we need to make the .fbx into a prefab.
First, make a folder called FullBodyAvatars under NewProject. Then drag claire_seperate_head from the project view into an open scene and from the scene hierarchy back into the new folder. Select Original Prefab after dragging it back to the folder. Rename it to Clair. Remove it from the scene.
![[Pasted image 20230523172820.png]]
![[Pasted image 20230523172922.png]]
![[Pasted image 20230523173020.png]]
![[Pasted image 20230523173057.png]]
Open the prefab for editing and add the component FullBodyAvatar to the top GameObject (Claire).
![[Pasted image 20230523174812.png]]


![[Pasted image 20230523174842.png]]
Put all mesh renderers that have to do with the head under Head Mesh Renderers and the rest under Body Mesh Renderers.
![[Pasted image 20230523174953.png]]

Now we can finally add the avatar prefab to the settings object (as the first entry) and test it with VR and body tracking!
![[Pasted image 20230523174133.png]]
![[Pasted image 20230523175311.png]]

On a sidenote, there is some good information about about creating rigged characters with Mixamo in this video:
https://www.youtube.com/watch?v=BQIie7O9CVE&ab_channel=MKGraphics