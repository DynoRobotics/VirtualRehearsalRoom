using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class AvatarSelectionUIManager : MonoBehaviour
{
    public GameObject LayoutGroup;
    public GameObject AvatarSelectionButtonPrefab;

    public PlayerConfigurator m_PlayerConfigurator;

    [HideInInspector]
    public UnityEvent<string> AvatarSelectedEvent = new();

    void Start()
    {
        VRRSettings settings = VRRSettingsAccessor.Instance.Settings;

        string[] avatarNames = { }; 
        if (m_PlayerConfigurator.PlayerType == PlayerConfigurator.PlayerTypeEnum.GenericVR || m_PlayerConfigurator.PlayerType == PlayerConfigurator.PlayerTypeEnum.Desktop)
        {
            avatarNames = settings.GetSimpleAvatarNames();
        } else if (m_PlayerConfigurator.PlayerType == PlayerConfigurator.PlayerTypeEnum.FullBodyVR)
        {
            avatarNames = settings.GetFullBodyAvatarNames();
        }

        foreach (string avatarName in avatarNames)
        {
            GameObject buttonGameObject = Instantiate(AvatarSelectionButtonPrefab);
            buttonGameObject.transform.SetParent(LayoutGroup.transform, false);
            GameObject textObject = buttonGameObject.transform.GetChild(0).gameObject;
            TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
            text.text = avatarName;
            Button button = buttonGameObject.GetComponent<Button>();
            button.onClick.AddListener(delegate{ SelectAvatar(avatarName); });
        }
        
    }

    private void SelectAvatar(string avatarName)
    {
        AvatarSelectedEvent.Invoke(avatarName);
    }
}
