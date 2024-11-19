using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Testspawn : MonoBehaviourPun
{
    public GameObject playerPrefab;

    public GameObject childPrefab;
    public string torso;
    public Transform spawnPoint1;
   
    // Start is called before the first frame update
    void Start()
    {
    PhotonNetwork.OfflineMode = true;
    PhotonNetwork.CreateRoom("TestRoom");
        
    GameObject localPlayer = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint1.position, spawnPoint1.rotation);
    Transform subParent = localPlayer.transform.Find("Torso/WeaponHolder");

    Camera characterCamera = localPlayer.GetComponentInChildren<Camera>();

    GameObject childObject = PhotonNetwork.Instantiate(childPrefab.name, subParent.transform.position, subParent.transform.rotation);
    childObject.transform.SetParent(subParent);
    //childObject.transform.localPosition = Vector3.zero;
    //childObject.transform.localRotation = Quaternion.identity;
    PlayerController playerScript = localPlayer.GetComponent<PlayerController>();

        if (playerScript != null)
        {
            playerScript.AssignWeapon(childObject);
        }

    Weapon weaponScript = childObject.GetComponent<Weapon>();

        if (weaponScript != null && characterCamera != null)
        {
            weaponScript.AssignCamera(characterCamera);
        }

    }

    



    // Update is called once per frame
    void Update()
    {
        
    }
}
