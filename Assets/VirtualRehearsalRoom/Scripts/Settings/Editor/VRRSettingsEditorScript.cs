using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(VRRSettings))]
public class VRRSettingsEditorScritp : Editor
{
    public override void OnInspectorGUI()
    {

        VRRSettings settings = (VRRSettings)target;

        string[] options = settings.GetEnabledScenes();

        if (options.Length == 0)
        {
            EditorGUILayout.HelpBox("No enabled scenes. Please add or enable some.", MessageType.Error);
        } else
        {
            List<string> optionsList = new List<string>(options);
            int selected = optionsList.IndexOf(settings.StartingSceneName);
            if (selected == -1)
            {
                selected = 0;
            }

            selected = EditorGUILayout.Popup("Starting Scene", selected, options);
            settings.StartingSceneName = options[selected];
            EditorUtility.SetDirty(settings);
        }

        // if (GUILayout.Button("Create New Scene"))
        // {
        //     CreateNewSceneWindow window = (CreateNewSceneWindow)CreateNewSceneWindow.GetWindow(typeof(CreateNewSceneWindow));
        //     window.Settings = settings;
        //     window.ShowUtility();
        // }

        DrawDefaultInspector();
    }
}

public class CreateNewSceneWindow : EditorWindow
{
    public VRRSettings Settings;

    string sceneName = "";
    string sceneDirectory = "Assets/VirtualRehersalRoom/Scenes/";
    string createdScenePath;

    void OnGUI()
    {
        sceneName = EditorGUILayout.TextField("Choose a scene name: ", sceneName);
        if (GUILayout.Button("Create new scene"))
        {
            createdScenePath = sceneDirectory + "Custom/" + sceneName + ".unity";
            AssetDatabase.CopyAsset(sceneDirectory + "Required/TemplateScene.unity", 
                                    createdScenePath);

            SceneReference scene = new SceneReference();
            scene.ScenePath = createdScenePath;
            Settings.Scenes.Add(scene);

            Debug.Log(scene.ScenePath);

            Close();
        }

        if (GUILayout.Button("Abort"))
        {
            Close();
        }
    }
}
