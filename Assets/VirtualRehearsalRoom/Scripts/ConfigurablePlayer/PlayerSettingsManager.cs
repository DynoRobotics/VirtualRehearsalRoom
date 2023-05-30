using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public class PlayerSettingsManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
    }

    public void SetNetworkedAudienceTeleportation(bool value)
    {
        Debug.Log("Setting audience teleportation: " + value.ToString());
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() {
            {MultiplayerVRConstants.AUDIENCE_TELEPORTATION_KEY, value} };

        PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
    }
}
