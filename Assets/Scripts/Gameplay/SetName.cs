using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using System.IO;

public class SetName : MonoBehaviour
{
    public TMP_InputField NameTF;
    public Button NameBtn;

    private string playerNameFilePath;

    private void Start()
    {
        NameBtn.interactable = false;
        NameTF.onValueChanged.AddListener(OnTFName);
        NameBtn.onClick.AddListener(OnClick_SetName);

        // Set the default nickname
        string defaultName = GetDefaultName();
        PhotonNetwork.NickName = defaultName;
        Debug.Log("Default nickname set to: " + defaultName);
    }

    private string GetDefaultName()
    {
        string defaultName = PlayerPrefs.GetString("Username");
        return defaultName;
        
        
    }

    public void OnTFName(string value)
    {
        if (value.Length > 3)
        {
            NameBtn.interactable = true;
        }
        else
        {
            NameBtn.interactable = false;
        }
    }

    public void OnClick_SetName()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.LocalPlayer != null)
        {
            if (string.IsNullOrEmpty(NameTF.text))
            {
                // Use the default name if the input field is empty
                string defaultName = GetDefaultName();
                PhotonNetwork.NickName = defaultName;
                Debug.Log("Setting nickname to default: " + PhotonNetwork.NickName);
            }
            else
            {
                PhotonNetwork.NickName = NameTF.text;
                Debug.Log("Setting nickname to: " + PhotonNetwork.NickName);
            }
        }
        else
        {
            Debug.LogError("Photon is not connected or LocalPlayer is null.");
        }
    }
}
