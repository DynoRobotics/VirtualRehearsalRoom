using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using xsens;


public class FullBodyAvatarImporter : MonoBehaviour
{
    public int AvatarSelectionNumber;

    public GameObject FullBodyAvatarGameObject;
    public GameObject FullBodyAvatarComponentsGameObject;

    public MultiplayerXsensSynchronization NetworkSynchronizer;
    public PhotonVoiceManager VoiceManager;
    public FullBodyNamePlateMover NameplateMover;

    public GameObject InstantiatedAvatarGameObject;
    public GameObject InstantiatedHandTrackingGameObject;

    public RuntimeAnimatorController HandTrackingAnimatorController;

    public GameObject LookTarget;

    void Start()                                                                                                                                      
    {
    }

    void Update()
    {
        FixAvatarTransforms();
    }

    public GameObject ImportSelectedAvatar()
    {
        FullBodyAvatar[] fullBodyAvatars = VRRSettingsAccessor.Instance.Settings.FullBodyAvatars;
        if (AvatarSelectionNumber >= fullBodyAvatars.Length)
        {
            Debug.LogWarning("Full body avatar selection number is out of range");
            AvatarSelectionNumber = 0;
        }

        GameObject oldInstantiatedHandTrackingGameObject = InstantiatedHandTrackingGameObject;
        InstantiatedHandTrackingGameObject = Instantiate(fullBodyAvatars[AvatarSelectionNumber].gameObject, FullBodyAvatarGameObject.transform);
        InstantiatedHandTrackingGameObject.name += " HandTrackingCharacter";

        FullBodyAvatar fullBodyAvatar = InstantiatedHandTrackingGameObject.GetComponent<FullBodyAvatar>();
        fullBodyAvatar.DisableMeshRenderers();

        Animator handTrackingAnimator = InstantiatedHandTrackingGameObject.GetComponent<Animator>();
        handTrackingAnimator.runtimeAnimatorController = HandTrackingAnimatorController;
        handTrackingAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        CopyHandTrackingAvatarComponents();

        GameObject oldAvatarGameObject = InstantiatedAvatarGameObject;
        InstantiatedAvatarGameObject = Instantiate(fullBodyAvatars[AvatarSelectionNumber].gameObject, FullBodyAvatarGameObject.transform);

        NetworkSynchronizer.Character = InstantiatedAvatarGameObject.transform;
        
        VRHandsLiveAnimator handsAnimator = FullBodyAvatarComponentsGameObject.GetComponent<VRHandsLiveAnimator>();
        handsAnimator.HandTrackingAnimator = InstantiatedHandTrackingGameObject.GetComponent<Animator>();
        handsAnimator.CharacterAnimator = InstantiatedAvatarGameObject.GetComponent<Animator>();

        CopyAvatarComponents();
        // Deactivate animation controller to allow xsens live animation to work
        InstantiatedAvatarGameObject.GetComponent<Animator>().runtimeAnimatorController = null;

        NetworkSynchronizer.BeginSetup();

        UpdateNamplateTransforms();

        Animator animator = InstantiatedAvatarGameObject.GetComponent<Animator>();
        VoiceManager.m_FullBodyAvatarHead = animator.GetBoneTransform(HumanBodyBones.Head).gameObject;


        if (oldAvatarGameObject != null)
        {
            if (Application.isPlaying)
            {
                Destroy(oldAvatarGameObject);
            } else if (Application.isEditor)
            {
                DestroyImmediate(oldAvatarGameObject);
            }
        } else
        {
            // Debug.LogWarning("Old avatar gameobjet is null, can't destroy");
        }

    if (oldInstantiatedHandTrackingGameObject != null)
        {
            if (Application.isPlaying)
            {
                Destroy(oldInstantiatedHandTrackingGameObject);
            } else if (Application.isEditor)
            {
                DestroyImmediate(oldInstantiatedHandTrackingGameObject);
            }
        } else
        {
            // Debug.LogWarning("Old hand tracking gameobjet is null, can't destroy");
        }

        // TODO(sam): Maybe restore this later. Optional?
        // GameObject lookTarget  = Instantiate(LookTarget, animator.GetBoneTransform(HumanBodyBones.Head));
        // lookTarget.name = "LookTarget";

        return InstantiatedAvatarGameObject;
    }

    private void CopyHandTrackingAvatarComponents()
    {
        HandController handController = FullBodyAvatarComponentsGameObject.GetComponent<HandController>();
        HandController newHandController = CopyComponent(handController, InstantiatedHandTrackingGameObject);

        newHandController.FieldsHaveBeenAssigned = true;    
        // Trigger OnEnable to register input actions
        newHandController.enabled = false;
        newHandController.enabled = true;
    }

    private void CopyAvatarComponents()
    {
        XsensXRSynchronizer xrSynchronizer = FullBodyAvatarComponentsGameObject.GetComponent<XsensXRSynchronizer>();
        VRRXsLiveAnimator xsLiveAnimator = FullBodyAvatarComponentsGameObject.GetComponent<VRRXsLiveAnimator>();
        VRHandsLiveAnimator handsAnimator = FullBodyAvatarComponentsGameObject.GetComponent<VRHandsLiveAnimator>();

        CopyComponent(xrSynchronizer, InstantiatedAvatarGameObject);
        CopyComponent(xsLiveAnimator, InstantiatedAvatarGameObject);
        CopyComponent(handsAnimator, InstantiatedAvatarGameObject);

        xrSynchronizer.enabled = false;
        xsLiveAnimator.enabled = false;
        handsAnimator.enabled = false;

        XsensXRSynchronizer newXRSynchronizer = InstantiatedAvatarGameObject.GetComponent<XsensXRSynchronizer>();
        newXRSynchronizer.Init();
    }

    private void UpdateNamplateTransforms()
    {
        Animator animator = InstantiatedAvatarGameObject.GetComponent<Animator>();

        NameplateMover.FullBodyAvatarHead = animator.GetBoneTransform(HumanBodyBones.Head);
        NameplateMover.FullBodyAvatarBody = animator.GetBoneTransform(HumanBodyBones.Chest);
    }

    private void FixAvatarTransforms()
    {
        if (InstantiatedAvatarGameObject == null)
        {
            return;
        }

        InstantiatedAvatarGameObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }


    T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }
}
