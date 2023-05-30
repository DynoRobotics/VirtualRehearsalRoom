using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

[CustomEditor(typeof(LoginManager))]
public class LoginManagerEditorScript : Editor 
{
    public override void OnInspectorGUI()
    {
        LoginManager loginManager = (LoginManager)target;

        EditorGUILayout.HelpBox("This script is responsible of connecting to Photon servers.", MessageType.Info);

        DrawDefaultInspector();

        if (GUILayout.Button("Connect"))
        {
            loginManager.ConnectToPhotonServer();
        }

    }
}
