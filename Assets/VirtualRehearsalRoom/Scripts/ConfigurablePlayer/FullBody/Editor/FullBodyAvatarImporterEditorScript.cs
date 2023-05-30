using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FullBodyAvatarManager))]
public class FullBodyAvatarImporterEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FullBodyAvatarManager avatarManager = (FullBodyAvatarManager)target;

        if (GUILayout.Button("Import avatar"))
        {
            avatarManager.UpdateAvatar();
        }
    }
}
