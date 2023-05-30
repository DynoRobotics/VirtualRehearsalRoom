using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

[CreateAssetMenu(fileName = "VRRSettings", menuName = "VirtualRehearsalRoomSettings")]
public class VRRSettings : ScriptableObject
{
    public string ProductName = "VirtualRehearsalRoom";
    
    public string AppIdPUN;
    public string AppIdVoice;

    public bool UseDevAppId = false;
    public string DevAppIdPUN;
    public string DevAppIdVoice;

    [HideInInspector]
    public string AddressableProfile = "";

    public bool EnableAudienceMenu = true;
    public bool RandomizeAudienceAvatar = false;

    public float VRMoveSpeed = 1.5f;
    public bool ShowDesktopOverlayUI = false;

    public MultiplayerVRConstants.VoiceTypeEnum PerfomerDefaultVoiceType = MultiplayerVRConstants.VoiceTypeEnum.Spatial;
    public MultiplayerVRConstants.VoiceTypeEnum AudienceDefaultVoiceType = MultiplayerVRConstants.VoiceTypeEnum.Spatial;

    [HideInInspector]
    public string StartingSceneName = "";
    public List<SceneReference> Scenes;

    public SimpleAvatar[] SimpleAvatars;
    public FullBodyAvatar[] FullBodyAvatars;

    public string GetPerformerDefaultVoiceType()
    {
        return MultiplayerVRConstants.GetVoiceTypeString(PerfomerDefaultVoiceType);
    }
    public string GetAudienceDefaultVoiceType()
    {
        return MultiplayerVRConstants.GetVoiceTypeString(AudienceDefaultVoiceType);
    }

    public string[] GetEnabledScenes()
    {
        if (Scenes == null)
        {
            return new string[0];
        }

        SceneReference[] scenes = Scenes.ToArray<SceneReference>();
        SceneReference[] enabledScenes = scenes.Where(scene => scene.SceneEnabled).ToArray();
        string[] enabledSceneNames = enabledScenes.Select(scene => Path.GetFileNameWithoutExtension(scene.ScenePath)).ToArray();
        return enabledSceneNames;
    }

    public string[] GetSimpleAvatarNames()
    {
        return SimpleAvatars.Select(prefab => prefab.name).ToArray();
    }
    public string[] GetFullBodyAvatarNames()
    {
        return FullBodyAvatars.Select(avatar => avatar.gameObject.name).ToArray();
    }

}
