using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerUIManager))]
public class PlayerUIManagerEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerUIManager playerUIManager = (PlayerUIManager)target;

        if (GUILayout.Button("Toggle Invisibility"))
        {
            playerUIManager.ToggleInvisibility();
        }

        if (GUILayout.Button("Toggle Safety Rail"))
        {
            playerUIManager.m_SafetyRailManager.ToggleSafetyRail();
        }
    }
}
