using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullBodyNamePlateMover : MonoBehaviour
{
    public float NamePlateVerticalOffset = 0.2f;
    public Transform FullBodyAvatarHead;
    public Transform FullBodyAvatarBody;

    void Update()
    {
        if (FullBodyAvatarHead != null && FullBodyAvatarBody != null)
        {
            Vector3 headPosition = FullBodyAvatarHead.transform.position;

            headPosition.y += NamePlateVerticalOffset;

            transform.position = headPosition;

            //Vector3 bodyRotation = FullBodyAvatarBody.eulerAngles;

            Vector3 namePlateRotation = transform.eulerAngles;
            namePlateRotation.y = FullBodyAvatarBody.eulerAngles.y;

            transform.eulerAngles = namePlateRotation;
        }
    }
}
