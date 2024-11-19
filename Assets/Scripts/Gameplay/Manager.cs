using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Manager : MonoBehaviourPun
{
    public GameObject playerPrefab;
    public GameObject weaponPrefab;
    public GameObject AKPrefab;
    public GameObject QQ9Prefab;    
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public Transform spawnPoint3;
    public Transform spawnPoint4;
    public Transform spawnPoint5;
    public Transform spawnPoint6;
    private Transform[] spawnPoints;
    public GameObject AkImage;
    public GameObject QQ9Image;
   
    void Start()
    {
        //PhotonNetwork.OfflineMode = true;
        //PhotonNetwork.CreateRoom("TestRoom");
       
        spawnPoints = new Transform[] { spawnPoint1, spawnPoint2, spawnPoint3, spawnPoint4, spawnPoint5, spawnPoint6 };
        
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        int playerCount = PhotonNetwork.PlayerList.Length;
        if (playerCount > spawnPoints.Length)
        {
            Debug.LogError("More players than spawn points available!");
            return;
        }

        // Get the player index based on their Photon ID
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber % spawnPoints.Length;

        Transform chosenSpawnPoint = spawnPoints[playerIndex];
        //PhotonNetwork.Instantiate(testPrefab.name, chosenSpawnPoint.position, chosenSpawnPoint.rotation);

        
        
        GameObject playerInstance = PhotonNetwork.Instantiate(playerPrefab.name, chosenSpawnPoint.position, chosenSpawnPoint.rotation);
        Transform weaponHolder = playerInstance.transform.Find("Torso/WeaponHolder");

        Camera characterCamera = playerInstance.GetComponentInChildren<Camera>();

        string currentWeapon = PlayerPrefs.GetString("Weapon");
        Debug.Log("Current weapon: " + currentWeapon);
        switch (currentWeapon)
        {
            case "AK":
                weaponPrefab = AKPrefab;
                AkImage.SetActive(true);
                break;
            case "QQ9":
                weaponPrefab = QQ9Prefab;
                QQ9Image.SetActive(true);
                break;
            default:
                Debug.LogError("Invalid weapon name: " + currentWeapon);
                break;
        }
        GameObject weaponInstance = PhotonNetwork.Instantiate(weaponPrefab.name, weaponHolder.transform.position, weaponHolder.transform.rotation);
        
        weaponInstance.transform.SetParent(weaponHolder);

        PhotonView playerPhotonView = playerInstance.GetComponent<PhotonView>();
        PhotonView weaponPhotonView = weaponInstance.GetComponent<PhotonView>();

        PlayerController playerScript = playerInstance.GetComponent<PlayerController>();

        if (playerScript != null)
        {
            playerScript.AssignWeapon(weaponInstance);
        }

        Weapon weaponScript = weaponInstance.GetComponent<Weapon>();
        
        
        TextMeshProUGUI bulletCountText = GameObject.Find("UI/Weapon UI/Bullet count").GetComponent<TextMeshProUGUI>();

        if (weaponScript != null && characterCamera != null)
        {
            weaponScript.AssignCamera(characterCamera);
            weaponScript.AssignBulletCount(bulletCountText);
            

        }
        CameraMovement cameraScript = playerInstance.GetComponentInChildren<CameraMovement>();
        if (cameraScript != null)
        {
            Transform Weapon = weaponInstance.transform.Find("Weapon");
            cameraScript.SetWeapon(Weapon);
        }

        
        if (playerPhotonView.IsMine) // Only do this if the player is the local player
        {
            weaponPhotonView.RPC("SetParentRPC", RpcTarget.AllBuffered, playerPhotonView.ViewID, weaponPhotonView.ViewID);
        }

    }



    public Transform GetRandomSpawnPoint()
    {
        int index = Random.Range(0, spawnPoints.Length);
        return spawnPoints[index];
    }

    
}
