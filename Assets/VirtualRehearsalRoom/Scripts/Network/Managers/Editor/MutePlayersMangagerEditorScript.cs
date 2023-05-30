using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(MutePlayersManager))]
public class MutePlayersMangagerEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MutePlayersManager mutePlayersManager = (MutePlayersManager)target;

        if (GUILayout.Button("Toggle Audience Mute"))
        {
            mutePlayersManager.ToggleAudienceMute(); 
        }

        if (GUILayout.Button("Toggle Performers Mute"))
        {
            mutePlayersManager.TogglePerformersMute();
        }

        if (GUILayout.Button("Set Audience Megaphone"))
        {
            mutePlayersManager.SetNetworkedAudienceVoiceType(MultiplayerVRConstants.VOICE_TYPE_VALUE_MEGAPHONE);
        }

        if (GUILayout.Button("Set Audience 3D Sound"))
        {
            mutePlayersManager.SetNetworkedAudienceVoiceType(MultiplayerVRConstants.VOICE_TYPE_VALUE_3D);
        }

        if (GUILayout.Button("Set Audience Super Close"))
        {
            mutePlayersManager.SetNetworkedAudienceVoiceType(MultiplayerVRConstants.VOICE_TYPE_VALUE_SUPER_CLOSE);
        }

        if (GUILayout.Button("Set Performers Megaphone"))
        {
            mutePlayersManager.SetNetworkedPerformersVoiceType(MultiplayerVRConstants.VOICE_TYPE_VALUE_MEGAPHONE);
        }

        if (GUILayout.Button("Set Performers 3D Sound"))
        {
            mutePlayersManager.SetNetworkedPerformersVoiceType(MultiplayerVRConstants.VOICE_TYPE_VALUE_3D);
        }

        if (GUILayout.Button("Set Performers Super Close"))
        {
            mutePlayersManager.SetNetworkedPerformersVoiceType(MultiplayerVRConstants.VOICE_TYPE_VALUE_SUPER_CLOSE);
        }
    }
}
