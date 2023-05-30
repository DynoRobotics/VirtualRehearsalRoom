using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoginUIManager : MonoBehaviour
{
    public GameObject UsernameInputField;

    public GameObject LoginPanel;
    public GameObject LoadingPanel;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(UsernameInputField);
    }

    void Update()
    {
        
    }

    public void OnConnectClicked()
    {
        LoginPanel.SetActive(false);
        LoadingPanel.SetActive(true);
    }
}
