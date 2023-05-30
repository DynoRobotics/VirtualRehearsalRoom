using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRRSettingsAccessor 
{
    private static VRRSettingsAccessor instance;
    public static VRRSettingsAccessor Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new VRRSettingsAccessor();
                return instance;
            }
            else
            {
                return instance;
            }
        }
    }

    public const string VirtualRehearsalRoomFileName = "VirtualRehearsalRoom";
    public VirtualRehearsalRoom VRR;

    public VRRSettings Settings
    { 
        get
        {
            if (VRR == null)
            {
                LoadOrCreateSettings();
            }
            return VRR.settings;
        }
        set 
        {
            VRR.settings = value; 
        }
    }

    public void LoadOrCreateSettings()
    {
        if (VRR != null)
        {
            Debug.LogWarning("VirtualRehersalRoomSettings is not null. Will not load LoadOrCreateSettings().");
            return;
        }

        VRR = Resources.Load<VirtualRehearsalRoom>(VirtualRehearsalRoomFileName);
        if (VRR != null)
        {
            if (VRR.settings == null)
            {
                Debug.LogError("No setting assigned to Virtual Rehersal Room Scriptagle Object");
                return;
            }
            return;
        }

    }
}
