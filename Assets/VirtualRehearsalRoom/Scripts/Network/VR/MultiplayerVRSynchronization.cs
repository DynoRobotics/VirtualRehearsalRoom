using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class MultiplayerVRSynchronization : MonoBehaviour, IPunObservable
{

    public PhotonView m_PhotonView;

    ////Main VRPlayer Transform Synch
    //[Header("Main VRPlayer Transform Synch")]
    //public Transform generalVRPlayerTransform;

    //Position
    private float m_Distance_GeneralVRPlayer;
    private Vector3 m_Direction_GeneralVRPlayer;
    private Vector3 m_NetworkPosition_GeneralVRPlayer;
    private Vector3 m_StoredPosition_GeneralVRPlayer;

    //Rotation
    private Quaternion m_NetworkRotation_GeneralVRPlayer;
    private float m_Angle_GeneralVRPlayer;

    //Main Avatar Transform Synch
    [Header("Main Avatar Transform Synch")]
    public Transform mainAvatarTransform;

    //Position
    private float m_Distance_MainAvatar;
    private Vector3 m_Direction_MainAvatar;
    private Vector3 m_NetworkPosition_MainAvatar;
    private Vector3 m_StoredPosition_MainAvatar;

    //Rotation
    private Quaternion m_NetworkRotation_MainAvatar;
    private float m_Angle_MainAvatar;

    //Head  Synch
    //Rotation
    [Header("Avatar Head Transform Synch")]
    public Transform headTransform;

    private Quaternion m_NetworkRotation_Head;
    private float m_Angle_Head;

    //Body Synch
    //Rotation
    [Header("Avatar Body Transform Synch")]
    public Transform bodyTransform;

    private Quaternion m_NetworkRotation_Body;
    private float m_Angle_Body;

    //Hands Synch
    public bool SynchAnimation = false;

    [Header("Hands Transform Synch")]
    public Transform leftHandTransform;
    public Transform rightHandTransform;

    [Header("Hands Animation Synch")]
    public Animator LeftHandAnimator;
    public Animator RightHandAnimator;

    //Left Hand Sync
    //Position
    private float m_Distance_LeftHand;

    private Vector3 m_Direction_LeftHand;
    private Vector3 m_NetworkPosition_LeftHand;
    private Vector3 m_StoredPosition_LeftHand;

    //Rotation
    private Quaternion m_NetworkRotation_LeftHand;
    private float m_Angle_LeftHand;

    //Right Hand Synch
    //Position
    private float m_Distance_RightHand;

    private Vector3 m_Direction_RightHand;
    private Vector3 m_NetworkPosition_RightHand;
    private Vector3 m_StoredPosition_RightHand;

    //Rotation
    private Quaternion m_NetworkRotation_RightHand;
    private float m_Angle_RightHand;

    bool m_firstTake = false;

    public void Awake()
    {
        //Main Avatar Synch Init
        m_StoredPosition_MainAvatar = mainAvatarTransform.position;
        m_NetworkPosition_MainAvatar = Vector3.zero;
        m_NetworkRotation_MainAvatar = Quaternion.identity;

        //Head Synch Init
        m_NetworkRotation_Head = Quaternion.identity;

        //Body Synch Init
        m_NetworkRotation_Body = Quaternion.identity;

        //Left Hand Synch Init
        m_StoredPosition_LeftHand = leftHandTransform.localPosition;
        m_NetworkPosition_LeftHand = Vector3.zero;
        m_NetworkRotation_LeftHand = Quaternion.identity;

        //Right Hand Synch Init
        m_StoredPosition_RightHand = rightHandTransform.localPosition;
        m_NetworkPosition_RightHand = Vector3.zero;
        m_NetworkRotation_RightHand = Quaternion.identity;
    }

    void OnEnable()
    {
        m_firstTake = true;
    }

    public void Update()
    {
        if (!this.m_PhotonView.IsMine)
        {
            mainAvatarTransform.position = Vector3.MoveTowards(mainAvatarTransform.position, this.m_NetworkPosition_MainAvatar, this.m_Distance_MainAvatar * (1.0f / PhotonNetwork.SerializationRate));
            mainAvatarTransform.rotation = Quaternion.RotateTowards(mainAvatarTransform.rotation, this.m_NetworkRotation_MainAvatar, this.m_Angle_MainAvatar * (1.0f / PhotonNetwork.SerializationRate));

            headTransform.localRotation = Quaternion.RotateTowards(headTransform.localRotation, this.m_NetworkRotation_Head, this.m_Angle_Head * (1.0f / PhotonNetwork.SerializationRate));

            bodyTransform.localRotation = Quaternion.RotateTowards(bodyTransform.localRotation, this.m_NetworkRotation_Body, this.m_Angle_Body * (1.0f / PhotonNetwork.SerializationRate));

            leftHandTransform.localPosition = Vector3.MoveTowards(leftHandTransform.localPosition, this.m_NetworkPosition_LeftHand, this.m_Distance_LeftHand * (1.0f / PhotonNetwork.SerializationRate));
            leftHandTransform.localRotation = Quaternion.RotateTowards(leftHandTransform.localRotation, this.m_NetworkRotation_LeftHand, this.m_Angle_LeftHand * (1.0f / PhotonNetwork.SerializationRate));

            rightHandTransform.localPosition = Vector3.MoveTowards(rightHandTransform.localPosition, this.m_NetworkPosition_RightHand, this.m_Distance_RightHand * (1.0f / PhotonNetwork.SerializationRate));
            rightHandTransform.localRotation = Quaternion.RotateTowards(rightHandTransform.localRotation, this.m_NetworkRotation_RightHand, this.m_Angle_RightHand * (1.0f / PhotonNetwork.SerializationRate));
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //////////////////////////////////////////////////////////////////
            //Main Avatar Transform Synch

            //Send Main Avatar position data
            this.m_Direction_MainAvatar = mainAvatarTransform.position - this.m_StoredPosition_MainAvatar;
            this.m_StoredPosition_MainAvatar = mainAvatarTransform.position;

            stream.SendNext(mainAvatarTransform.position);
            stream.SendNext(this.m_Direction_MainAvatar);

            //Send Main Avatar rotation data
            stream.SendNext(mainAvatarTransform.rotation);

            ///////////////////////////////////////////////////////////////////
            //Head rotation synch

            //Send Head rotation data
            stream.SendNext(headTransform.localRotation);

            ///////////////////////////////////////////////////////////////////
            //Body rotation synch

            //Send Body rotation data
            stream.SendNext(bodyTransform.localRotation);

            ///////////////////////////////////////////////////////////////////
            //Hands Transform Synch
            //Left Hand
            //Send Left Hand position data
            this.m_Direction_LeftHand = leftHandTransform.localPosition - this.m_StoredPosition_LeftHand;
            this.m_StoredPosition_LeftHand = leftHandTransform.localPosition;

            stream.SendNext(leftHandTransform.localPosition);
            stream.SendNext(this.m_Direction_LeftHand);

            //Send Left Hand rotation data
            stream.SendNext(leftHandTransform.localRotation);

            //Right Hand
            //Send Right Hand position data
            this.m_Direction_RightHand = rightHandTransform.localPosition - this.m_StoredPosition_RightHand;
            this.m_StoredPosition_RightHand = rightHandTransform.localPosition;

            stream.SendNext(rightHandTransform.localPosition);
            stream.SendNext(this.m_Direction_RightHand);

            //Send Right Hand rotation data
            stream.SendNext(rightHandTransform.localRotation);

            ///////////////////////////////////////////////////////////////////
            //Hands Animation Synch

            if (SynchAnimation)
            {
                // Send Right Hand Animation Data
                stream.SendNext(RightHandAnimator.GetFloat("Grip"));
                stream.SendNext(RightHandAnimator.GetFloat("Trigger"));

                // Send Left Hand Animation Data
                stream.SendNext(LeftHandAnimator.GetFloat("Grip"));
                stream.SendNext(LeftHandAnimator.GetFloat("Trigger"));
            }

        }
        else
        {
            ///////////////////////////////////////////////////////////////////
            //Main Avatar Transform Synch

            //Get Main Avatar position data
            this.m_NetworkPosition_MainAvatar = (Vector3)stream.ReceiveNext();
            this.m_Direction_MainAvatar = (Vector3)stream.ReceiveNext();

            if (m_firstTake)
            {
                mainAvatarTransform.position = this.m_NetworkPosition_MainAvatar;
                this.m_Distance_MainAvatar = 0f;
            }
            else
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                this.m_NetworkPosition_MainAvatar += this.m_Direction_MainAvatar * lag;
                this.m_Distance_MainAvatar = Vector3.Distance(mainAvatarTransform.position, this.m_NetworkPosition_MainAvatar);
            }

            //Get Main Avatar rotation data
            this.m_NetworkRotation_MainAvatar = (Quaternion)stream.ReceiveNext();
            if (m_firstTake)
            {
                this.m_Angle_MainAvatar = 0f;
                mainAvatarTransform.rotation = this.m_NetworkRotation_MainAvatar;
            }
            else
            {
                this.m_Angle_MainAvatar = Quaternion.Angle(mainAvatarTransform.rotation, this.m_NetworkRotation_MainAvatar);
            }


            ///////////////////////////////////////////////////////////////////
            //Head rotation synch
            //Get Head rotation data 
            this.m_NetworkRotation_Head = (Quaternion)stream.ReceiveNext();

            if (m_firstTake)
            {
                this.m_Angle_Head = 0f;
                headTransform.localRotation = this.m_NetworkRotation_Head;
            }
            else
            {
                this.m_Angle_Head = Quaternion.Angle(headTransform.localRotation, this.m_NetworkRotation_Head);
            }

            ///////////////////////////////////////////////////////////////////
            //Body rotation synch
            //Get Body rotation data 
            this.m_NetworkRotation_Body = (Quaternion)stream.ReceiveNext();

            if (m_firstTake)
            {
                this.m_Angle_Body = 0f;
                bodyTransform.localRotation = this.m_NetworkRotation_Body;
            }
            else
            {
                this.m_Angle_Body = Quaternion.Angle(bodyTransform.localRotation, this.m_NetworkRotation_Body);
            }

            ///////////////////////////////////////////////////////////////////
            //Hands Transform Synch
            //Get Left Hand position data
            this.m_NetworkPosition_LeftHand = (Vector3)stream.ReceiveNext();
            this.m_Direction_LeftHand = (Vector3)stream.ReceiveNext();

            if (m_firstTake)
            {
                leftHandTransform.localPosition = this.m_NetworkPosition_LeftHand;
                this.m_Distance_LeftHand = 0f;
            }
            else
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                this.m_NetworkPosition_LeftHand += this.m_Direction_LeftHand * lag;
                this.m_Distance_LeftHand = Vector3.Distance(leftHandTransform.localPosition, this.m_NetworkPosition_LeftHand);
            }

            //Get Left Hand rotation data
            this.m_NetworkRotation_LeftHand = (Quaternion)stream.ReceiveNext();
            if (m_firstTake)
            {
                this.m_Angle_LeftHand = 0f;
                leftHandTransform.localRotation = this.m_NetworkRotation_LeftHand;
            }
            else
            {
                this.m_Angle_LeftHand = Quaternion.Angle(leftHandTransform.localRotation, this.m_NetworkRotation_LeftHand);
            }

            //Get Right Hand position data
            this.m_NetworkPosition_RightHand = (Vector3)stream.ReceiveNext();
            this.m_Direction_RightHand = (Vector3)stream.ReceiveNext();

            if (m_firstTake)
            {
                rightHandTransform.localPosition = this.m_NetworkPosition_RightHand;
                this.m_Distance_RightHand = 0f;
            }
            else
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                this.m_NetworkPosition_RightHand += this.m_Direction_RightHand * lag;
                this.m_Distance_RightHand = Vector3.Distance(rightHandTransform.localPosition, this.m_NetworkPosition_RightHand);
            }

            //Get Right Hand rotation data
            this.m_NetworkRotation_RightHand = (Quaternion)stream.ReceiveNext();
            if (m_firstTake)
            {
                this.m_Angle_RightHand = 0f;
                rightHandTransform.localRotation = this.m_NetworkRotation_RightHand;
            }
            else
            {
                this.m_Angle_RightHand = Quaternion.Angle(rightHandTransform.localRotation, this.m_NetworkRotation_RightHand);
            }
            if (m_firstTake)
            {
                m_firstTake = false;
            }


            ///////////////////////////////////////////////////////////////////
            //Hands Animation Synch

            if (SynchAnimation)
            {
                // Send Right Hand Animation Data
                RightHandAnimator.SetFloat("Grip", (float)stream.ReceiveNext());
                RightHandAnimator.SetFloat("Trigger", (float)stream.ReceiveNext());

                // Send Left Hand Animation Data
                LeftHandAnimator.SetFloat("Grip", (float)stream.ReceiveNext());
                LeftHandAnimator.SetFloat("Trigger", (float)stream.ReceiveNext());
            } 
        }
    }

}
