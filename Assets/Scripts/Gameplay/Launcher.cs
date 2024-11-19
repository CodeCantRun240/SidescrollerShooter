using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public GameObject connectedScreen;
    public GameObject disconnectedScreen;
    public GameObject connectionScreen; 
    private bool isConnecting = false;

    public void OnClick_ConnectBtn()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if(isConnecting == false)
        {
        disconnectedScreen.SetActive(true); 
        }
    }

    public override void OnJoinedLobby()
    {
        if(disconnectedScreen.activeSelf)
            disconnectedScreen.SetActive(false);
        else{
            connectedScreen.SetActive(true);
        }
        
    }
    
    public void OnClick_returnToMenu()
    {
        PhotonNetwork.Disconnect();
        connectedScreen.SetActive(false);
        connectionScreen.SetActive(true);
        isConnecting = true;
    }
    
}
