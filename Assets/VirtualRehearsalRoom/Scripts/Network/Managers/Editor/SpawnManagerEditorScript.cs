using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnManager))]
public class SpawnManagerEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SpawnManager spawnManager = (SpawnManager)target;

        if (GUILayout.Button("Spawn AI Player"))
        {
            spawnManager.SpawnAIPlayer();
        }

        if (GUILayout.Button("Spawn 10 AI Players"))
        {
            for (int i = 0; i < 10; i++)
            {
                spawnManager.SpawnAIPlayer();
            }
        }
    }
}
