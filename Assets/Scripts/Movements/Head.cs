using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
public class Head :MonoBehaviourPun, IPunObservable

{
   
    [SerializeField] private Transform head;
    [SerializeField] private float headDistance;

    [SerializeField] private Camera characterCamera;
    private bool facingRight = true;

    private Vector3 headPos;
    private Quaternion headRot;
    public PhotonView photonview;

    void Start()
    {
        //PhotonNetwork.OfflineMode = true;
        //PhotonNetwork.CreateRoom("TestRoom");

        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 30;
    }
    void Update()
    {
        if (photonView.IsMine)
        {
            HandleInput();
        }
        else
        {
           Sync();
        }
        
        }
        
    private void HandleInput()
    {
        Vector3 mousePos = characterCamera.ScreenToWorldPoint(Input.mousePosition);
        
        Vector3 headDirection = mousePos - transform.position;
        float headAngle = Mathf.Atan2(headDirection.y, headDirection.x) * Mathf.Rad2Deg;

        head.transform.rotation = Quaternion.Euler(new Vector3(0, 0, headAngle));
        head.transform.position = transform.position + Quaternion.Euler(0, 0, headAngle) * new Vector3(headDistance, 0, 0.1f);
        
        if (mousePos.x < head.transform.position.x && facingRight)
        {
            photonView.RPC("RpcFlip", RpcTarget.All);
        }
        else if (mousePos.x > head.transform.position.x && !facingRight)
        {
            photonView.RPC("RpcFlip", RpcTarget.All);
        }
        
    }

    [PunRPC]
    private void Sync()
    {
        head.rotation = Quaternion.Slerp(head.transform.rotation, headRot, Time.deltaTime * 100);
        head.position = Vector3.Lerp(head.transform.position, headPos, Time.deltaTime * 100);
        
    }

    [PunRPC]
    public void RpcFlip()
    {
        facingRight = !facingRight;
        head.localScale = new Vector3(head.localScale.x, head.localScale.y * -1, head.localScale.z);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(head.transform.rotation);
            stream.SendNext(head.transform.position);
            
            stream.SendNext(facingRight);
        }
        else
        {
            headRot = (Quaternion)stream.ReceiveNext();
            headPos = (Vector3)stream.ReceiveNext();
            
            facingRight = (bool)stream.ReceiveNext();
        }
    }
   
}
