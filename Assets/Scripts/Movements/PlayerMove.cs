using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMove : MonoBehaviourPun, IPunObservable
{
    public PhotonView photonview;
    [SerializeField] public float speed;
    [SerializeField] public float jumpPower;
    private Vector3 smoothMove;
    public Rigidbody2D[] characterRigidbodies;
    private GameObject sceneCamera;
    public GameObject playerCamera;

    private void Start()
    {
        PopulateRigidbodiesArray();

        if (photonview.IsMine)
        {
            playerCamera = GameObject.Find("Main Camera");
            sceneCamera.SetActive(false);
            playerCamera.SetActive(true);
        }
       
        
    }

    
    void Update()
    {
        if (photonview.IsMine)
        {
            ProcessInputs();
        }
        
        else {
            Movement();
       }
        }

    
     private void PopulateRigidbodiesArray()
    {
        // Find all Rigidbody components attached to child GameObjects
        characterRigidbodies = GetComponentsInChildren<Rigidbody2D>();
    }
    
    private void Movement(){
        transform.position = Vector3.Lerp(transform.position, smoothMove, speed * Time.deltaTime *100);
    }
    private void ProcessInputs()
    {
        
    float horizontalInput = Input.GetAxisRaw("Horizontal");

    // Calculate movement direction based on input
    Vector3 moveDirection = new Vector2(horizontalInput, 0).normalized;

    // Apply movement to the Rigidbody components
    foreach (Rigidbody2D rb in characterRigidbodies)
    {
        rb.velocity = moveDirection * speed;
    }    
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
           stream.SendNext(transform.position);
        }
        else if (stream.IsReading)
        {
            smoothMove = (Vector3)stream.ReceiveNext();
            
        }
    }
}
