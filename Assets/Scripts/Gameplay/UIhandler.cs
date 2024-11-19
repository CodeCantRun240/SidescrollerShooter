using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
public class UIhandler : MonoBehaviourPunCallbacks
{
    public TMP_InputField createRoomTF;
    public TMP_InputField joinRoomTF;
    private bool isPrivateMatch = false;
    public MessageBox messageBox;
    public void OnClick_JoinRoom(){
        isPrivateMatch = true;
        PlayerPrefs.SetInt("IsPrivateMatch", isPrivateMatch ? 1 : 0);
        PhotonNetwork.JoinRoom(joinRoomTF.text, null);
        
    }

    public void OnClick_CreateRoom(){

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 6; 
        roomOptions.IsVisible = false; 
        roomOptions.IsOpen = true; 

        isPrivateMatch = true;
        PlayerPrefs.SetInt("IsPrivateMatch", isPrivateMatch ? 1 : 0);
        string newRoomName = Random.Range(1000, 10000).ToString();
        PhotonNetwork.CreateRoom(newRoomName, roomOptions);
        
    }

    public void OnClick_JoinRandomRoom()
    {
         
        isPrivateMatch = false;
        PlayerPrefs.SetInt("IsPrivateMatch", isPrivateMatch ? 1 : 0);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom(){
        print("Room Joined Successful");
        PlayerPrefs.SetString("RoomName", PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel(2);
    }

     public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No random room available, creating a new one.");
        isPrivateMatch = false;
        PlayerPrefs.SetInt("IsPrivateMatch", isPrivateMatch ? 1 : 0);
        string newRoomName = Random.Range(1000, 10000).ToString();
        PhotonNetwork.CreateRoom(newRoomName, new Photon.Realtime.RoomOptions { MaxPlayers = 6 }, null);
    }
        public override void OnJoinRoomFailed(short returnCode, string message){
        print("RoomFailed" + returnCode + "Message" + message);
        string NoRoom = "Room ID does not exist";
        messageBox.ShowMessage(NoRoom);
    }

    
}
