using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerSettingsManager))]
public class PlayerSettingsManagerEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerSettingsManager playerSettingsManager = (PlayerSettingsManager)target;

        if (GUILayout.Button("Activate audience teleporting"))
        {
            playerSettingsManager.SetNetworkedAudienceTeleportation(true);
        }

        if (GUILayout.Button("Deactivate audience teleporting"))
        {
            playerSettingsManager.SetNetworkedAudienceTeleportation(false);
        }
    }
}
