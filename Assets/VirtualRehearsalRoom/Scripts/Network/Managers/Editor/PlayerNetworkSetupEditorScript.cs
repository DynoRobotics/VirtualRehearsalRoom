using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerNetworkManager))]
public class PlayerNetworkSetupEditorScript : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerNetworkManager playerNetworkSetup = (PlayerNetworkManager)target;

        if (Application.IsPlaying(playerNetworkSetup))
        {
            if (GUILayout.Button("Select Next Avatar")) 
            {
                playerNetworkSetup.SelectNextAvatar();
            }

            if (GUILayout.Button("Select Previous Avatar")) 
            {
                playerNetworkSetup.SelectPreviousAvatar();
            }

            //if (GUILayout.Button("Toggle Mute Audience")) 
            //{
            //    playerNetworkSetup.ToggleMuteAudience();
            //}

            //if (GUILayout.Button("Toggle Mute Performers")) 
            //{
            //    playerNetworkSetup.ToggleMutePerformers();
            //}
        }
    }

}
