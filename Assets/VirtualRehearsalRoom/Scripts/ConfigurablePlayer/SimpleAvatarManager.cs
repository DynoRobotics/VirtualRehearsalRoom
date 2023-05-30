using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAvatarManager : MonoBehaviour
{
    public PlayerConfigurator m_PlayerConfigurator;

    public int AvatarSelectionNumber;

    public GameObject SimpleAvatarGameObject;

    public string PlayerDisplayName = "Unknown Name";

    public TMPro.TextMeshProUGUI PlayerDisplayNameText;
    public TMPro.TextMeshProUGUI PlayerBackDisplayNameText;

    public Transform CameraOffsetTransform;

    public GameObject AnimatedHandRightGameObject;
    public GameObject AnimatedHandLeftGameObject;

    public SimpleAvatarHandController LeftHandAnimationController;
    public SimpleAvatarHandController RightHandAnimationController;

    public GameObject AvatarUIGameObject;

    private SimpleAvatar[] _avatarModelPrefabs;

    private GameObject _selectedAvatarGameObject = null;

    private AvatarInputConverter _avatarInputConverter;

    private bool _avatarIsRemote = false;

    private Vector3 _frontTextLocalPos;
    private Vector3 _backTextLocalPos;

    public void SetAvatarIsRemote(bool value)
    {
        _avatarIsRemote = value;

        if (_avatarIsRemote)
        {
            _avatarInputConverter.enabled = false;
            LeftHandAnimationController.enabled = false;
            RightHandAnimationController.enabled = false;
        } else
        {
            _avatarInputConverter.enabled = true;
            LeftHandAnimationController.enabled = true;
            RightHandAnimationController.enabled = true;
        }
    }

    private void Awake()
    {
        _avatarModelPrefabs = VRRSettingsAccessor.Instance.Settings.SimpleAvatars;
        _frontTextLocalPos = PlayerDisplayNameText.gameObject.GetComponent<RectTransform>().localPosition;
        _backTextLocalPos = PlayerBackDisplayNameText.gameObject.GetComponent<RectTransform>().localPosition;

        _avatarInputConverter = SimpleAvatarGameObject.GetComponent<AvatarInputConverter>();
    }

    private void OnEnable()
    {
        SimpleAvatarGameObject.SetActive(true);
    }

    private void OnDisable()
    {
        SimpleAvatarGameObject.SetActive(false);
    }

    public void SetPlayerType(PlayerConfigurator.PlayerTypeEnum PlayerType)
    {
        _avatarInputConverter.PlayerType = PlayerType;
    }

    public void SetAvtarSelectionNumber(int newAvatarSelectionNumber)
    {
        if (newAvatarSelectionNumber <= _avatarModelPrefabs.Length && newAvatarSelectionNumber >= 0)
        {
            AvatarSelectionNumber = newAvatarSelectionNumber;
        } else
        {
            Debug.LogWarning("Avatar selection number: " + newAvatarSelectionNumber.ToString() + " is out of range, not updating!");
        }
    }

    public void SelectAvatar(int newAvatarSelectionNumber)
    {
        if (newAvatarSelectionNumber < _avatarModelPrefabs.Length)
        {
            AvatarSelectionNumber = newAvatarSelectionNumber;
        } else
        {
            Debug.LogWarning("Avatar selection number: " + newAvatarSelectionNumber + " is out of range, aborting!");

            return;
        }

        UpdateAvatar();
    }

    public void UpdateAvatar()
    {
        UpdateAvatarSelection();
        SetLayersForCameraVisibility();
        PlayerDisplayNameText.text = PlayerDisplayName; // TODO(sam): Make setter instead?
        PlayerBackDisplayNameText.text = PlayerDisplayName; // TODO(sam): Make setter instead?
    }

    
    public void SetLayersForCameraVisibility()
    {
        if (_avatarIsRemote || _avatarInputConverter.PlayerType == PlayerConfigurator.PlayerTypeEnum.AI)
        {
            // Make visible
            SetLayerRecursively(_avatarInputConverter.AvatarHead.gameObject, 0);
            SetLayerRecursively(_avatarInputConverter.AvatarBody.gameObject, 0);
            SetLayerRecursively(AvatarUIGameObject, 18);
        } else 
        { 
            // Make invisible
            SetLayerRecursively(_avatarInputConverter.AvatarHead.gameObject, 11);
            SetLayerRecursively(_avatarInputConverter.AvatarBody.gameObject, 12);
            SetLayerRecursively(AvatarUIGameObject, 18);
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

    public void UpdateAvatarSelection()
    {
        DestroyCurrentAvatarModel();
        InstantiateSelectedAvatarModel();
        ConfigureHands();
        ReparentAvatarBodyparts();
    }

    private void ConfigureHands()
    {
        SimpleAvatar simpleAvatar = _selectedAvatarGameObject.GetComponent<SimpleAvatar>();

        SkinnedMeshRenderer leftHandRenderer = AnimatedHandLeftGameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer rightHandRenderer = AnimatedHandRightGameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        if (simpleAvatar.HandsMaterial != null)
        {
            leftHandRenderer.material = simpleAvatar.HandsMaterial;
            rightHandRenderer.material = simpleAvatar.HandsMaterial;
        }

        AnimatedHandRightGameObject.SetActive(true);
        AnimatedHandLeftGameObject.SetActive(true);
    }

    private void DestroyCurrentAvatarModel()
    {
        if (_selectedAvatarGameObject != null)
        {
            SimpleAvatar simpleAvatar = _selectedAvatarGameObject.GetComponent<SimpleAvatar>();

            simpleAvatar.HeadTransform.SetParent(_selectedAvatarGameObject.transform);
            simpleAvatar.BodyTransform.SetParent(_selectedAvatarGameObject.transform);
            // avatarHolder.HandLeftTransform.SetParent(_selectedAvatarGameObject.transform);
            // avatarHolder.HandRightTransform.SetParent(_selectedAvatarGameObject.transform);

            Destroy(_selectedAvatarGameObject);
            _selectedAvatarGameObject = null;
        }
    }

    private void InstantiateSelectedAvatarModel()
    {
        _selectedAvatarGameObject = Instantiate(_avatarModelPrefabs[AvatarSelectionNumber].gameObject, SimpleAvatarGameObject.transform);
    }

    private void ReparentAvatarBodyparts()
    {
        SimpleAvatar simpleAvatar = _selectedAvatarGameObject.GetComponent<SimpleAvatar>();

        ReparentAvatarBodypart(simpleAvatar.HeadTransform, _avatarInputConverter.AvatarHead);
        ReparentAvatarBodypart(simpleAvatar.BodyTransform, _avatarInputConverter.AvatarBody);
        // ReparentAvatarBodypart(avatarHolder.HandLeftTransform, _avatarInputConverter.AvatarHand_Left);
        // ReparentAvatarBodypart(avatarHolder.HandRightTransform, _avatarInputConverter.AvatarHand_Right);
    }

    private void ReparentAvatarBodypart(Transform avatarModelTransform, Transform mainAvatarTransform)
    {
        avatarModelTransform.SetParent(mainAvatarTransform);
        avatarModelTransform.localPosition = Vector3.zero;
        avatarModelTransform.localRotation = Quaternion.identity;
    }
}
