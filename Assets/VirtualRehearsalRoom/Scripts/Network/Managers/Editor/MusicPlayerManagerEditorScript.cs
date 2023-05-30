using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MusicPlayerManager))]
public class MusicPlayerManagerEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MusicPlayerManager musicPlayerManager = (MusicPlayerManager)target;

        if (GUILayout.Button("Play Music For All"))
        {
            musicPlayerManager.PlayMusicForAll();
        }

        if (GUILayout.Button("Stop Music For All"))
        {
            musicPlayerManager.StopMusicForAll();
        }

        if (GUILayout.Button("Select Next Track"))
        {
            musicPlayerManager.SelectNextTrack();
        }

        if (GUILayout.Button("Select Previous Track"))
        {
            musicPlayerManager.SelectPreviousTrack();
        }
    }
}
