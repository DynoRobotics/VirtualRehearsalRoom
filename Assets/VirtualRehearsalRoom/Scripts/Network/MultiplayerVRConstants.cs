
public static class MultiplayerVRConstants
{
    [System.Serializable]
    public enum VoiceTypeEnum
    {
        Megaphone,
        Spatial,
        SuperClose,
    }
    public static string GetVoiceTypeString(VoiceTypeEnum voiceType)
    {
        string voiceTypeString = "";
        switch (voiceType) {
            case VoiceTypeEnum.Megaphone:
                voiceTypeString = VOICE_TYPE_VALUE_MEGAPHONE;
                break;
            case VoiceTypeEnum.Spatial:
                voiceTypeString = VOICE_TYPE_VALUE_3D;
                break;
            case VoiceTypeEnum.SuperClose:
                voiceTypeString = VOICE_TYPE_VALUE_SUPER_CLOSE;
                break;
        }

        return voiceTypeString;
    }

    public const string SCENE_NAME_KEY = "scene_name";

    public const string AUDIENCE_MUTED_KEY = "audience_muted";
    public const string PERFORMERS_MUTED_KEY = "performers_muted";
    public const string AUDIENCE_VOICE_TYPE_KEY = "audience_voice_type";
    public const string PERFORMERS_VOICE_TYPE_KEY = "performers_voice_type";
    public const string AUDIENCE_TELEPORTATION_KEY = "audience_teleportation";

    public const string MUSIC_PLAYING_KEY = "music_playing";
    public const string MUSIC_VOLUME_KEY = "music_volume_key";
    public const string MUSIC_SELECTED_TRACK_KEY = "music_selected_track";
    public const string MUSIC_PLAYING_START_TIME_KEY = "music_playing_start_time";
    public const string PLAYER_HIDE_AVATAR_KEY = "hide_avatar";
    public const string PLAYER_AUDIENCE_MEMBER_KEY = "audience_member";
    public const string PLAYER_TYPE = "Player_Type";

    public const string AVATAR_SELECTION_NUMBER = "avatar_selection_number";
    public const string FULL_BODY_AVATAR_SELECTION_NUMBER = "full_body_avatar_selection_number";
    public const string PLAYER_TYPE_VALUE_GENERIC_VR = "generic_vr";
    public const string PLAYER_TYPE_VALUE_FULL_BODY_VR = "full_body_vr";
    public const string PLAYER_TYPE_VALUE_DESKTOP = "desktop";
    public const string PLAYER_TYPE_VALUE_AI = "ai";

    public const string VOICE_TYPE_VALUE_MEGAPHONE = "megaphone";
    public const string VOICE_TYPE_VALUE_3D = "3D";
    public const string VOICE_TYPE_VALUE_SUPER_CLOSE = "super_close";


}
