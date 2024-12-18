using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
public class Weapon : MonoBehaviourPun, IPunObservable
{
    public GameObject rightRotatingCenter;
    [SerializeField] private Animator weaponAnim;
    [SerializeField] private Animator flashAnim;
    [SerializeField] private Animator lightAnim;
    [SerializeField] private Animator backArmAnim;
    [SerializeField] public Transform weapon;
    [SerializeField] private Transform backArm;
    [SerializeField] private float weaponDistance;
    [SerializeField] private float backArmDistance;
    [SerializeField] private Camera characterCamera;
    private Rigidbody2D weaponRb;
    private bool facingRight = true;
    private Vector3 weaponPos;
    private Quaternion weaponRot;
    private Vector3 backArmPos;
    private Quaternion backArmRot;
    private Vector3 centerPos;
    public PhotonView photonview;

    [SerializeField] public float fireRate = 0.3f;
    public GameObject bulletPrefab; 
    public Transform muzzle; 
    
    private PlayerController playerMovement;
    private float timer = Mathf.Infinity;
    public int bulletCount ;
    public int maxBulletCount ;
    private bool isReloading = false;
    public TMP_Text bulletCountText; 

    public bool matchStarted = false;

    [SerializeField] private float minRange;

    [SerializeField] private float maxRange;

    public AudioSource shootingSound;
    public AudioSource reloadSound;

    private void Start()
    {
        

        PhotonNetwork.SendRate = 35;
        PhotonNetwork.SerializationRate = 35;
        weaponRb = weapon.GetComponent<Rigidbody2D>();
        
        //weaponAnim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerController>();
        //bool isRunning = playerMovement.isRunning;
         AudioSource[] audioSources = GetComponents<AudioSource>();
        shootingSound = audioSources[0];  // First AudioSource
        reloadSound = audioSources[1]; 
    }

    private void Update()
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

        Vector3 weaponDirection = mousePos - rightRotatingCenter.transform.position;
        float weaponAngle = Mathf.Atan2(weaponDirection.y, weaponDirection.x) * Mathf.Rad2Deg;

        weapon.transform.rotation = Quaternion.Euler(new Vector3(0, 0, weaponAngle));
        weapon.transform.position = transform.position + Quaternion.Euler(0, 0, weaponAngle) * new Vector3(weaponDistance, 0, -0.02f);

        backArm.transform.rotation = Quaternion.Euler(new Vector3(0, 0, weaponAngle));
        backArm.transform.position = transform.position + Quaternion.Euler(0, 0, weaponAngle) * new Vector3(backArmDistance, 0, 2f);

        if (mousePos.x < weapon.transform.position.x && facingRight)
        {
            photonView.RPC("RpcFlip", RpcTarget.All);
        }
        else if (mousePos.x > weapon.transform.position.x && !facingRight)
        {
            photonView.RPC("RpcFlip", RpcTarget.All);
        }

        if (Input.GetMouseButton(0) && timer > fireRate && bulletCount > 0 && !isReloading && matchStarted)
        {
            Shoot();
            timer = 0; 
        }
        timer += Time.deltaTime;

        if (isReloading)
        return;


        if (Input.GetKey(KeyCode.R) && bulletCount < maxBulletCount)
    {
       StartCoroutine(Timeout());
    }
        
    }
    

    [PunRPC]
    private void Sync()
    {
        //rightRotatingCenter.transform.position = Vector3.Lerp(rightRotatingCenter.transform.position, centerPos, Time.deltaTime * 10);
        weapon.rotation = Quaternion.Slerp(weapon.transform.rotation, weaponRot, Time.deltaTime * 100);
        weapon.position = Vector3.Lerp(weapon.transform.position, weaponPos, Time.deltaTime * 100);
        backArm.rotation = Quaternion.Slerp(backArm.transform.rotation, backArmRot, Time.deltaTime * 100);
        backArm.position = Vector3.Lerp(backArm.transform.position, backArmPos, Time.deltaTime * 100);
        
    }

    [PunRPC]
    public void RpcFlip()
    {
        facingRight = !facingRight;
        weapon.localScale = new Vector3(weapon.localScale.x, weapon.localScale.y * -1, weapon.localScale.z);
        backArm.localScale = new Vector3(backArm.localScale.x, backArm.localScale.y * -1, backArm.localScale.z);

    }

    [PunRPC]
    public void Shoot()
    {
        weaponAnim.SetTrigger("Shoot");
        flashAnim.SetTrigger("Shoot");
        lightAnim.SetTrigger("Shoot");
        backArmAnim.SetTrigger("Shoot");
        shootingSound.Play();
        float spreadAngle = UnityEngine.Random.Range(minRange, maxRange);
        
        Quaternion spreadRotation = Quaternion.Euler(0f, 0f, spreadAngle);

        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, muzzle.position, muzzle.rotation * spreadRotation);
        BulletTrail bulletTrail = bullet.GetComponent<BulletTrail>();

        //if (bulletTrail != null)
        //{
        // Set the shooter information
        //bulletTrail.photonView.RPC("SetShooter", RpcTarget.AllBuffered, photonView);
        //}
        bulletCount--;
        
    }
    
    [PunRPC]
    IEnumerator Timeout()
    {
        isReloading = true;
        weaponAnim.SetTrigger("Reload");
        backArmAnim.SetTrigger("Reload");
        reloadSound.Play();
        yield return new WaitForSeconds(2.1f); 

        bulletCount = maxBulletCount;
        isReloading = false;
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
           
           // We own this player: send the others our data
        stream.SendNext(weapon.transform.rotation);
        stream.SendNext(weapon.transform.position);
        stream.SendNext(backArm.transform.rotation);
        stream.SendNext(backArm.transform.position);
        stream.SendNext(facingRight);
        //stream.SendNext(weapon.transform.rotation);
        }
        else if (stream.IsReading)
        {
            // Network player, receive data
        weaponRot = (Quaternion)stream.ReceiveNext();
        weaponPos = (Vector3)stream.ReceiveNext();
        backArmRot = (Quaternion)stream.ReceiveNext();
        backArmPos = (Vector3)stream.ReceiveNext();
        facingRight = (bool)stream.ReceiveNext();
        
        //Quaternion targetRotation = (Quaternion)stream.ReceiveNext();
        //weapon.transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }

    public void AssignCamera(Camera CameraObject)
    {
        characterCamera = CameraObject;
    }

    public void AssignBulletCount(TMP_Text bulletCount)
    {
        bulletCountText = bulletCount;
    }

    [PunRPC]
    public void SetParentRPC(int playerViewID, int weaponViewID)
    {     
        PhotonView playerPhotonView = PhotonView.Find(playerViewID);
        PhotonView weaponPhotonView = PhotonView.Find(weaponViewID);

        if (weaponPhotonView != null && playerPhotonView != null)
        {
            Transform weaponHolder = playerPhotonView.transform.Find("Torso/WeaponHolder");
            weaponPhotonView.transform.SetParent(weaponHolder);
        }
    }
}
