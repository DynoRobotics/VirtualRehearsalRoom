using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarInputConverter : MonoBehaviour
{
    public PlayerConfigurator.PlayerTypeEnum PlayerType;

    [Space]
    [Header("Avatar")]
    public Transform MainAvatarTransform;
    public Transform AvatarHead;
    public Transform AvatarBody;

    public Transform AvatarHand_Left;
    public Transform AvatarHand_Right;

    [Space]
    [Header("Generic VR Player")]
    public Transform XRHead;
    public Transform XRHandLeft;
    public Transform XRHandRight;
    public Vector3 GenericVRHeadPositionOffset = new Vector3(0, -0.9f, 0);
    public Vector3 GenericVRHandRotationOffset;

    [Space]
    [Header("Desktop Player")]
    public Transform DesktopHead;
    public Transform DesktopHandLeft;
    public Transform DesktopHandRight;
    public Vector3 DesktopHeadPositionOffset = new Vector3(0, -0.9f, 0);

    [Space]
    [Header("AI Player")]
    public Transform AIHead;
    public Transform AIHandLeft;
    public Transform AIHandRight;
    public Vector3 AIHeadPositionOffset = new Vector3(0, -0.9f, 0);

    // Update is called once per frame
    void Update()
    {
        switch (PlayerType)
        {
            case (PlayerConfigurator.PlayerTypeEnum.GenericVR):
                UpdateFromGenericVRPlayer();
                break;
            case (PlayerConfigurator.PlayerTypeEnum.Desktop):
                UpdateFromDesktopPlayer();
                break;
            case (PlayerConfigurator.PlayerTypeEnum.AI):
                UpdateFromAIPlayer();
                break;
            default:
                break;
        }
    }

    private void UpdateFromAIPlayer()
    {
        // Head and Body synch
        MainAvatarTransform.position = Vector3.Lerp(MainAvatarTransform.position, AIHead.position + AIHeadPositionOffset, 0.5f);
        AvatarHead.rotation = Quaternion.Lerp(AvatarHead.rotation, AIHead.rotation, 0.5f);
        AvatarBody.rotation = Quaternion.Lerp(AvatarBody.rotation, Quaternion.Euler(new Vector3(0, AvatarHead.rotation.eulerAngles.y, 0)), 0.01f);

        // Hands synch
        AvatarHand_Right.position = Vector3.Lerp(AvatarHand_Right.position, AIHandRight.position,0.5f);
        AvatarHand_Right.rotation = Quaternion.Lerp(AvatarHand_Right.rotation, AIHandRight.rotation, 0.5f);

        AvatarHand_Left.position = Vector3.Lerp(AvatarHand_Left.position, AIHandLeft.position, 0.5f);
        AvatarHand_Left.rotation = Quaternion.Lerp(AvatarHand_Left.rotation, AIHandLeft.rotation, 0.5f);
    }

    private void UpdateFromGenericVRPlayer()
    {
        // Head and Body synch
        MainAvatarTransform.position = Vector3.Lerp(MainAvatarTransform.position, XRHead.position + GenericVRHeadPositionOffset, 0.5f);
        AvatarHead.rotation = Quaternion.Lerp(AvatarHead.rotation, XRHead.rotation, 0.5f);
        AvatarBody.rotation = Quaternion.Lerp(AvatarBody.rotation, Quaternion.Euler(new Vector3(0, AvatarHead.rotation.eulerAngles.y, 0)), 0.05f);

        // Hands synch
        AvatarHand_Right.position = Vector3.Lerp(AvatarHand_Right.position, XRHandRight.position, 0.5f);
        AvatarHand_Right.rotation = Quaternion.Lerp(AvatarHand_Right.rotation, XRHandRight.rotation, 0.5f)*Quaternion.Euler(GenericVRHandRotationOffset);

        AvatarHand_Left.position = Vector3.Lerp(AvatarHand_Left.position, XRHandLeft.position, 0.5f);
        AvatarHand_Left.rotation = Quaternion.Lerp(AvatarHand_Left.rotation, XRHandLeft.rotation, 0.5f)*Quaternion.Euler(GenericVRHandRotationOffset);
    }

    private void UpdateFromDesktopPlayer()
    {
        // Head and Body synch
        MainAvatarTransform.position = Vector3.Lerp(MainAvatarTransform.position, DesktopHead.position + DesktopHeadPositionOffset, 0.5f);
        AvatarHead.rotation = Quaternion.Lerp(AvatarHead.rotation, DesktopHead.rotation, 0.5f);
        AvatarBody.rotation = Quaternion.Lerp(AvatarBody.rotation, Quaternion.Euler(new Vector3(0, AvatarHead.rotation.eulerAngles.y, 0)), 0.05f);

        // Hands synch
        AvatarHand_Right.position = Vector3.Lerp(AvatarHand_Right.position, DesktopHandRight.position,0.5f);
        AvatarHand_Right.rotation = Quaternion.Lerp(AvatarHand_Right.rotation, DesktopHandRight.rotation,0.5f) * Quaternion.Euler(GenericVRHandRotationOffset);

        AvatarHand_Left.position = Vector3.Lerp(AvatarHand_Left.position, DesktopHandLeft.position, 0.5f);
        AvatarHand_Left.rotation = Quaternion.Lerp(AvatarHand_Left.rotation, DesktopHandLeft.rotation, 0.5f) * Quaternion.Euler(GenericVRHandRotationOffset);
    }
}
