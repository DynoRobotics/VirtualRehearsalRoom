using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xsens;
using UnityEngine.InputSystem;
using Photon.Pun;

public class FullBodyAvatarManager : MonoBehaviour
{
    public GameObject FullBodyAvatar;

    public GameObject XsensStreamReader;

    public PlayerConfigurator m_PlayerConfigurator;

    public XsLiveAnimator m_XsLiveAnimator;
    public XsensXRSynchronizer m_XsensXRSynchronizer;

    public GameObject CharacterGameObject;

    public string PlayerDisplayName = "Unknown Name";

    public TMPro.TextMeshProUGUI PlayerDisplayNameText;

    public FullBodyAvatarImporter AvatarImporter;

    [SerializeField]
    InputActionReference ResetRotationAction;
    public InputActionReference resetRotationAction
    {
        get => ResetRotationAction;
        set => ResetRotationAction = value;
    }

    private bool _avatarIsRemote;

    private void OnEnable()
    {
        FullBodyAvatar.SetActive(true);
    }

    private void OnDisable()
    {
        FullBodyAvatar.SetActive(false);
    }

    public void Update()
    {
        if (ResetRotationAction.action.triggered)
        {
            if (m_PlayerConfigurator.NetworkGameObject.GetComponent<PhotonView>().IsMine)
            {
                if (AvatarImporter.InstantiatedAvatarGameObject != null)
                {
                    Debug.Log("Resetting XRRig camera");
                    AvatarImporter.InstantiatedAvatarGameObject.GetComponent<XsensXRSynchronizer>().ResetCameraRotation();
                }
                else
                {
                    Debug.LogWarning("No full body avatar loaded, cant reset XRRig camera");
                }
            }
        }
   }

    public void SetAvtarSelectionNumber(int newAvatarSelectionNumber)
    {
        if (newAvatarSelectionNumber <= VRRSettingsAccessor.Instance.Settings.FullBodyAvatars.Length && newAvatarSelectionNumber >= 0)
        {
            AvatarImporter.AvatarSelectionNumber = newAvatarSelectionNumber;
        } else
        {
            Debug.LogWarning("Avatar selection number: " + newAvatarSelectionNumber.ToString() + " is out of range, not updating!");
        }
    }

    public void UpdateAvatar()
    {
        PlayerDisplayNameText.text = PlayerDisplayName;
        CharacterGameObject = AvatarImporter.ImportSelectedAvatar();
        UpdateAvatarIsRemote();
    }

    private void UpdateAvatarIsRemote()
    {
        if (CharacterGameObject == null)
        {
            Debug.LogWarning("Full body avatar is not set, can't configure remote/local settings");
            return;
        }

        if (_avatarIsRemote)
        {
            XsensStreamReader.SetActive(false);

            CharacterGameObject.GetComponent<VRRXsLiveAnimator>().enabled = false;
            CharacterGameObject.GetComponent<XsensXRSynchronizer>().enabled = false;
            CharacterGameObject.GetComponent<VRHandsLiveAnimator>().enabled = false;
        }
        else
        {
            XsensStreamReader.SetActive(true);

            CharacterGameObject.GetComponent<VRRXsLiveAnimator>().enabled = true;
            CharacterGameObject.GetComponent<XsensXRSynchronizer>().enabled = true;
            CharacterGameObject.GetComponent<VRHandsLiveAnimator>().enabled = true;

            CharacterGameObject.GetComponent<XsensXRSynchronizer>().ReparentAvatarToXRRig();

            DisplayLookTarget(CharacterGameObject.transform);
        }

        SetLayersForCameraVisibility();
    }

    public void DisplayLookTarget(Transform trans)
    {

        foreach (Transform child in trans)
        {
            if (child.gameObject.name == "LookTarget")
            {
                child.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }

            if (child.childCount > 0)
            {
                DisplayLookTarget(child);
            }
        }
    }

    public void SetAvatarIsRemote(bool value)
    {
        _avatarIsRemote = value;
        UpdateAvatarIsRemote();
    }

    public void SetLayersForCameraVisibility()
    {
        SkinnedMeshRenderer[] headMeshRenderers = CharacterGameObject.GetComponent<FullBodyAvatar>().m_HeadMeshRenderers;
        foreach (SkinnedMeshRenderer meshRenderer in headMeshRenderers)
        {
            if (!_avatarIsRemote)
            {
                meshRenderer.gameObject.layer = 11;
            }
            else
            {
                meshRenderer.gameObject.layer = 0;
            }
        }
    }

    void SetLayerRecursively(GameObject go, int layerNumber)
    {
        if (go == null) return;
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

}
