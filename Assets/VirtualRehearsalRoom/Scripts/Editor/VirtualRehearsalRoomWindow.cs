using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Photon.Pun;

using System.Threading.Tasks;

using System;
using System.IO;
using System.Linq;
using System.Collections.Concurrent;
using System.Security.Cryptography;

using Unity.Services.CCD.Management;
using Unity.Services.CCD.Management.Apis.Entries;
using Unity.Services.CCD.Management.Http;
using Unity.Services.CCD.Management.Apis.Content;
using Unity.Services.CCD.Management.Apis.Releases;
using Unity.Services.CCD.Management.Entries;
using Unity.Services.CCD.Management.Content;
using System.Net;
using Unity.Services.CCD.Management.Apis.Badges;
using Unity.Services.CCD.Management.Releases;
using Unity.Services.CCD.Management.Badges;
using Unity.Services.CCD.Management.Models;


public class VirtualRehearsalRoomWindow : EditorWindow 
{
    [MenuItem("Window/Virtual Rehearsal Room")]
    static void Init()
    {
        VirtualRehearsalRoomWindow window = (VirtualRehearsalRoomWindow)EditorWindow.GetWindow(typeof(VirtualRehearsalRoomWindow));
        window.titleContent.text = "Virtual Rehearsal Room";
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Virtual Rehearsal Room", EditorStyles.boldLabel);

        VRRSettings activeSettings = VRRSettingsAccessor.Instance.Settings;
        VRRSettings[] availableSettings = GetAllInstances<VRRSettings>();
        if (activeSettings == null)
        {
            VRRSettingsAccessor.Instance.Settings = availableSettings[0];
            activeSettings = VRRSettingsAccessor.Instance.Settings;
        }

        List<string> options = new List<string>();
        for (int i = 0; i < availableSettings.Length; i++)
        {
            options.Add(availableSettings[i].name);
        }

        int previouslySelected = options.FindIndex(x => x.Contains(activeSettings.name));
        int selected = EditorGUILayout.Popup("Active Settings", previouslySelected, options.ToArray());
        if (selected != previouslySelected)
        {
            VRRSettingsAccessor.Instance.Settings = availableSettings[selected];
            EditorUtility.SetDirty(VRRSettingsAccessor.Instance.VRR);
            UpdateFromSettings();

            activeSettings = VRRSettingsAccessor.Instance.Settings;
            Selection.objects = new UnityEngine.Object[] { activeSettings };
            EditorGUIUtility.PingObject(activeSettings);
        }    

        bool UseDevAppId = EditorGUILayout.Toggle("Use Dev App IDs", activeSettings.UseDevAppId);
        if (UseDevAppId != activeSettings.UseDevAppId)
        {
            activeSettings.UseDevAppId = UseDevAppId;
            EditorUtility.SetDirty(VRRSettingsAccessor.Instance.Settings);
            UpdateFromSettings();
        }

        if (GUILayout.Button("Force Update Settings"))
        {
            UpdateFromSettings();
        }

        if (GUILayout.Button("Highlight Active Settings"))
        {
            Selection.objects = new UnityEngine.Object[] { activeSettings };
            EditorGUIUtility.PingObject(activeSettings);
        }

        GUILayout.Label("Play", EditorStyles.boldLabel);
        if (GUILayout.Button("Login Scene"))
        {
            EditorSceneManager.OpenScene("Assets/VirtualRehearsalRoom/Scenes/Required/Login.unity");
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }

        if (GUILayout.Button("Auto Login Scene"))
        {
            EditorSceneManager.OpenScene("Assets/VirtualRehearsalRoom/Scenes/Required/AutoLoginDesktop.unity");
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }
    }

    void UpdateFromSettings()
    {
        EditorUtility.SetDirty(VRRSettingsAccessor.Instance.VRR);
        VRRSettings settings = VRRSettingsAccessor.Instance.Settings;
        EditorUtility.SetDirty(settings);

        ServerSettings photonSettings = PhotonNetwork.PhotonServerSettings;

        if (settings.UseDevAppId)
        {
            photonSettings.AppSettings.AppIdRealtime = settings.DevAppIdPUN;
            photonSettings.AppSettings.AppIdVoice = settings.DevAppIdVoice;
        } else
        {
            photonSettings.AppSettings.AppIdRealtime = settings.AppIdPUN;
            photonSettings.AppSettings.AppIdVoice = settings.AppIdVoice;
        }
        EditorUtility.SetDirty(photonSettings);

        PlayerSettings.productName = settings.ProductName;
    }

    private T[] GetAllInstances<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
        T[] a = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return a;
    }
}
