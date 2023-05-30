using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NetworkedAudioTrigger))]
public class NetworkedAudioTriggerEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NetworkedAudioTrigger networkedAudioTrigger = (NetworkedAudioTrigger)target;
        if (GUILayout.Button("Trigger"))
        {
            networkedAudioTrigger.TriggerSoundForAll();
        }
    }
}
