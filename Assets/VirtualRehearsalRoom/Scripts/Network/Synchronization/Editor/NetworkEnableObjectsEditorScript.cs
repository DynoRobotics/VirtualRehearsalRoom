using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NetworkEnableObjects))]
public class NetworkEnableObjectsEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NetworkEnableObjects networkEnableObjects = (NetworkEnableObjects)target;
        if (GUILayout.Button("Enable Objects"))
        {
            networkEnableObjects.EnableObjectsForAll();
        }
        if (GUILayout.Button("Disable Objects"))
        {
            networkEnableObjects.DisableObjectsForAll();
        }


    }
}
