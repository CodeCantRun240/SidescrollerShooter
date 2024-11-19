using JetBrains.Annotations;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    public PhotonView photonview;
    [SerializeField] public float speed;
    [SerializeField] public float jumpPower;
    [SerializeField] public GameObject torso;
    [SerializeField] public GameObject head;
    private Vector3 smoothMove;
    [SerializeField] public Camera sceneCamera;
    //public GameObject playerCamera;
    [SerializeField] public GameObject rightRotatingCenter;
    [SerializeField] public GameObject headRotating;
    public bool facingRight = true;
    [SerializeField] public Camera characterCamera;
    [SerializeField] private Rigidbody2D body;
    private Animator torsoAnimator;
    private Vector3 standingPos;
    private Vector3 runningPos;
    private Vector3 headPos;
    private Vector3 headRunningPos;
    float jumpTimer = 0f;
    float jumpCooldown = 0.5f; 
    private BoxCollider2D boxCollider;
    
    [SerializeField] private LayerMask groundLayer;
    public TMP_Text nameText;
    private bool isJumping = false;
    [SerializeField] private float jumpTime;
    Vector2 vecGravity;
    [SerializeField] private float jumpMultiplier;
    private float jumpCounter;
    public bool isRunning = false;
    public bool matchStarted = false;



    private void Awake()
    {
        
    }
    private void Start()
    {
        
    //PhotonNetwork.OfflineMode = true;
    //PhotonNetwork.CreateRoom("TestRoom");

    PhotonNetwork.SendRate = 35;
    PhotonNetwork.SerializationRate = 35;
        
        if (photonview.IsMine)
        {
            nameText.text = PhotonNetwork.NickName;

            body = GetComponent<Rigidbody2D>();
            torsoAnimator = torso.GetComponent<Animator>();
            photonview = GetComponent<PhotonView>();
            boxCollider = GetComponent<BoxCollider2D>();
            

            standingPos = rightRotatingCenter.transform.localPosition;
            headPos = headRotating.transform.localPosition;

            runningPos = standingPos + new Vector3(0.15f, -0.09f, 0);
            headRunningPos = headPos + new Vector3(0.18f, -0.1f, 0);
            
            //playerCamera = GameObject.Find("Character camera");
            Camera.main.gameObject.SetActive(false);
            //characterCamera.SetActive(true);
            SetupCamera();
            vecGravity = new Vector2(0, -Physics2D.gravity.y);

            gameObject.layer = LayerMask.NameToLayer("Friendly");
            
        }
        else {
            nameText.text = photonview.Owner.NickName;
        }
    
        
    }

    
    void Update()
    {
        Animator torsoAnimator = torso.GetComponent<Animator>();
        
        if (photonview.IsMine)
        {
            ProcessInputs();
        }
        else {
            Movement();
        }
        }

[PunRPC]
private void Flip()
{
    facingRight = !facingRight;
    torso.transform.Rotate(0f, 180f, 0f);
}
    
    private void Movement(){
        transform.position = Vector3.Lerp(transform.position, smoothMove, speed * Time.deltaTime * 15);
    }
    private void ProcessInputs()
    {
        float horizontalInput = 0f;
        if (Input.GetKey(KeyCode.A) && matchStarted)
            horizontalInput = -1f;
        else if (Input.GetKey(KeyCode.D) && matchStarted)
            horizontalInput = 1f;
        
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        
        if (Input.GetKeyDown(KeyCode.W) && isGrounded() && Time.time > jumpTimer && matchStarted)
        {
        
        Jumping();
        isJumping = true;
        jumpCounter = 0;
        jumpTimer = Time.time + jumpCooldown;
        }

        if (body.velocity.y > 0 && isJumping)
        {
            jumpCounter += Time.deltaTime;
            if (jumpCounter > jumpTime) isJumping = false;
            body.velocity += vecGravity*jumpMultiplier*Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            isJumping = false;
            torsoAnimator.SetTrigger("Jump");
        }
        if (body.velocity.y < 0 )
        {
            body.velocity -= vecGravity*Time.deltaTime;
        }
          
        bool isRunning = horizontalInput != 0f;
        
        Vector3 mousePos = characterCamera.ScreenToWorldPoint(Input.mousePosition);
        if (mousePos.x < transform.position.x && facingRight)
        {       
            photonView.RPC("Flip", RpcTarget.AllBuffered);
        }
        else if (mousePos.x > transform.position.x && !facingRight)
        {
            photonView.RPC("Flip", RpcTarget.AllBuffered);
        }

        torsoAnimator.SetBool("Is Grounded", isGrounded());
        
        // Swapping arm and head position while running
        if (isGrounded())
        {
            if (isRunning)
            {
                rightRotatingCenter.transform.localPosition = runningPos;
                headRotating.transform.localPosition = headRunningPos;
            }
            else
            {
                rightRotatingCenter.transform.localPosition = standingPos;
                headRotating.transform.localPosition = headPos;
            }
        }
        else
        {
            isRunning = false;
            rightRotatingCenter.transform.localPosition = standingPos;
            headRotating.transform.localPosition = headPos;
        }

        torsoAnimator.SetBool("Is Running", isRunning);
        
        
        
        
        
        }
    
    [PunRPC]
    private void OnEnable()
    {
       SetupCamera();
    }

    private void SetupCamera()
    {
        if (photonView.IsMine)
        {
            characterCamera.gameObject.SetActive(true);
        }
        else
        {
            characterCamera.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    public void StartMatch()
    {
        matchStarted = true;
    }

    [PunRPC]
    public void MatchEnd()
    {
        matchStarted = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    if (stream.IsWriting)
    {
        stream.SendNext(transform.position);
        stream.SendNext(facingRight);
        
    }
    else if (stream.IsReading)
    {
        smoothMove = (Vector3)stream.ReceiveNext();
        facingRight = (bool)stream.ReceiveNext();

        
    }
}

    
    private void Jumping()
    {
        body.velocity = new Vector2(body.velocity.x, jumpPower);
        torsoAnimator.SetTrigger("Jump");
        
    }

    private bool isGrounded()
{
    RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
   
    if (raycastHit.collider != null)
    {
        return true;
    }
    else
    {
        return false;
    }

   

}

    public void AssignWeapon(GameObject AKObject)
    {
        rightRotatingCenter = AKObject;
        // Additional setup if needed
    }

    
}
