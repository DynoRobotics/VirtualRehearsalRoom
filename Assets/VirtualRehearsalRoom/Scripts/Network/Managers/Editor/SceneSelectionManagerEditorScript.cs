using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneSelectionManager))]
public class SceneSelectionManagerEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SceneSelectionManager sceneSelectionManager = (SceneSelectionManager)target;

        if (GUILayout.Button("Reset Scene")) {
            sceneSelectionManager.ResetScene();
        }

        foreach (string sceneName in VRRSettingsAccessor.Instance.Settings.GetEnabledScenes())
        {
            if (GUILayout.Button(sceneName))
            {
                sceneSelectionManager.ChangeSceneForAll(sceneName);
            }
        }
    }
}
