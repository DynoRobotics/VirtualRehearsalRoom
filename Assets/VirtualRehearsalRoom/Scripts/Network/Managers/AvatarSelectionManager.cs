using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AvatarSelectionManager : MonoBehaviour
{
    public PlayerFinder m_PlayerFinder;

    public bool m_LocalPlayerFound = false;

    public PlayerConfigurator m_LocalPlayerConfigurator;
    public PlayerNetworkManager m_LocalPlayerNetworkManager;

    private void Awake()
    {
        m_PlayerFinder = FindObjectOfType<PlayerFinder>();
    }

    private void Update()
    {
        if (!m_LocalPlayerFound)
        {
            if (m_PlayerFinder.LocalPlayerConfigurator != null)
            {
                m_LocalPlayerConfigurator = m_PlayerFinder.LocalPlayerConfigurator;
                m_LocalPlayerNetworkManager = m_LocalPlayerConfigurator.NetworkGameObject.GetComponent<PlayerNetworkManager>();
                m_LocalPlayerFound = true;
            }
        }
    }

    public void SelectNextAvatar()
    {
        if (m_LocalPlayerFound)
        {
            m_LocalPlayerNetworkManager.SelectNextSimpleAvatar();
        }
        else
        {
            Debug.LogWarning("No local player found, could not select next avatar");
        }
    }
    public void SelectPreviousAvatar()
    {
        if (m_LocalPlayerFound)
        {
            m_LocalPlayerNetworkManager.SelectPreviousSimpleAvatar();
        }
        else
        {
            Debug.LogWarning("No local player found, could not select previous avatar");
        }
    }

    public void SelectAvatarByName(string avatarName)
    {
        if (m_LocalPlayerFound)
        {
            m_LocalPlayerNetworkManager.SelectSimpleAvatarByName(avatarName);
        }
        else
        {
            Debug.LogWarning("No local player found, could not select avatar");
        }
    }

    public void SelectFullBodyAvtarByName(string avatarName)
    {
        if (m_LocalPlayerFound)
        {
            m_LocalPlayerNetworkManager.SelectFullBodyAvatarByName(avatarName);
        }
        else
        {
            Debug.LogWarning("No local player found, could not select avatar");
        }
    }

}
