
using UnityEngine;
using System.Collections;
using Photon.Pun;
public class Shoot : MonoBehaviour
{
    [SerializeField] private float fireRate = 0.3f;
    public GameObject bulletPrefab; 
    public Transform muzzle; 
    private Animator animator;
    private PlayerController playerMovement;
    private float timer = Mathf.Infinity;
    private int bulletCount = 3000;
    private bool isReloading = false;
    public AudioSource shootingSound;
    private void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerController>();
        shootingSound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && timer > fireRate && bulletCount > 0) //*&& !isReloading*//)
        {
            Fire();
            timer = 0; 
        }
        timer += Time.deltaTime;

        if (isReloading)
        return;


        //if (Input.GetKey(KeyCode.C))
    {
       // StartCoroutine(Timeout());
    }
        
    }

    private void Fire()
    {
        
        animator.SetTrigger("Fire");
        shootingSound.Play();
        float spreadAngle = Random.Range(-3f, 3f);
        Quaternion spreadRotation = Quaternion.Euler(0f, 0f, spreadAngle);

        //*GameObject bullet =*//
        PhotonNetwork.Instantiate (bulletPrefab.name, muzzle.position, muzzle.rotation*spreadRotation);
        bulletCount--;
        //Debug.Log("Bullet Count: " + bulletCount);
        
    }
    IEnumerator Timeout()
{
    isReloading = true;
    animator.SetTrigger("Reload");

    yield return new WaitForSeconds(1.5f); 

    bulletCount = 30;
    isReloading = false;
}
}