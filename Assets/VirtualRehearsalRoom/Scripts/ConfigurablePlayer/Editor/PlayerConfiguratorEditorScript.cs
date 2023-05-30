using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerConfigurator))]
public class PlayerConfiguratorEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("This script is responisble for configuring the local and remote players and avatars", MessageType.Info);

        DrawDefaultInspector();

        PlayerConfigurator playerConfigurator = (PlayerConfigurator)target;

        if (Application.IsPlaying(playerConfigurator))
        {
            if (GUILayout.Button("Update Avatar"))
            {
                playerConfigurator.UpdateAvatar();
            }

            if (GUILayout.Button("Hide Avatar"))
            {
                playerConfigurator.SetHideAvatar(true);
            }
            
            if (GUILayout.Button("Show Avatar"))
            {
                playerConfigurator.SetHideAvatar(false);
            }
        }
    }

}
