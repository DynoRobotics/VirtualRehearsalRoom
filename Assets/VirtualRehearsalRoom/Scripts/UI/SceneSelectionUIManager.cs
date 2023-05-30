using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class SceneSelectionUIManager : MonoBehaviour
{
    public GameObject LayoutGroup;
    public GameObject SceneSelectionButtonPrefab;

    [HideInInspector]
    public UnityEvent<string> SceneSelectedEvent = new();

    void Start()
    {
        VRRSettings settings = VRRSettingsAccessor.Instance.Settings;
        string[] enabledScenes = settings.GetEnabledScenes();

        foreach (string sceneName in enabledScenes)
        {
            GameObject buttonGameObject = Instantiate(SceneSelectionButtonPrefab);
            buttonGameObject.transform.SetParent(LayoutGroup.transform, false);
            GameObject textObject = buttonGameObject.transform.GetChild(0).gameObject;
            TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
            text.text = sceneName;
            Button button = buttonGameObject.GetComponent<Button>();
            button.onClick.AddListener(delegate{ SelectScene(sceneName); });
        }
        
    }

    private void SelectScene(string sceneName)
    {
        SceneSelectedEvent.Invoke(sceneName);
    }

}
