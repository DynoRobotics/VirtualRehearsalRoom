using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AvatarSelectionManager))]
public class AvatarSelectionManagerEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AvatarSelectionManager avatarSelectionManager = (AvatarSelectionManager)target;

        if (avatarSelectionManager.m_LocalPlayerFound)
        {
            PlayerConfigurator playerConfigurator = avatarSelectionManager.m_LocalPlayerConfigurator;


            if (playerConfigurator.PlayerType == PlayerConfigurator.PlayerTypeEnum.GenericVR || playerConfigurator.PlayerType == PlayerConfigurator.PlayerTypeEnum.Desktop)
            {
                foreach (string avatarName in VRRSettingsAccessor.Instance.Settings.GetSimpleAvatarNames())
                {
                    if (GUILayout.Button(avatarName))
                    {
                        avatarSelectionManager.m_LocalPlayerNetworkManager.SelectSimpleAvatarByName(avatarName);
                    }
                }
            } else
            {
                foreach (string avatarName in VRRSettingsAccessor.Instance.Settings.GetFullBodyAvatarNames())
                {
                    if (GUILayout.Button(avatarName))
                    {
                        avatarSelectionManager.m_LocalPlayerNetworkManager.SelectFullBodyAvatarByName(avatarName);
                    }
                }
            }
            
            if (GUILayout.Button("Select Next Avatar"))
            {
                avatarSelectionManager.SelectNextAvatar();
            }

            if (GUILayout.Button("Select Previous Avatar"))
            {
                avatarSelectionManager.SelectPreviousAvatar();
            }
        }
    }
}
