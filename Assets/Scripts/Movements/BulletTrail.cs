using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class BulletTrail : MonoBehaviourPun
{
    
    [SerializeField] public float minSpeed;
    [SerializeField] public float maxSpeed;
    [SerializeField] public float maxRange;
    [SerializeField] public float damage;
    public Rigidbody2D rb;
    //private BoxCollider2D boxCollider;
    private Vector2 initialPosition;
    private TrailRenderer trailRenderer;
    
    [SerializeField] public GameObject impactEffect;

    private PhotonView shooterView;
    private Health playerHealth;
    
    

    void Start()
    {
        PhotonNetwork.SendRate = 40;

        float randomSpeed = Random.Range(minSpeed, maxSpeed);
        initialPosition = rb.transform.position; 
        rb.velocity = transform.right * randomSpeed;
        
        Health playerHealth = GetComponent<Health>();

 
    }
   void Update()
    {
    if (Vector2.Distance(initialPosition, transform.position) >= maxRange)
    {
        GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.All);
        
    }
    
    trailRenderer = GetComponent<TrailRenderer>();
    }

    [PunRPC]
    void DestroyObject()
    {
        Destroy(gameObject);
        Debug.Log("Destroyed");
       
           
        
        
        
    }

    public void SetShooter(PhotonView shooter)
    {
        shooterView = shooter;
    }

    [PunRPC]
    void OnTriggerEnter2D(Collider2D collision)
    {
    if (collision.gameObject.GetComponent<BoxCollider2D>() != null && GetComponent<PhotonView>().IsMine)
    {
        Health targetHealth = collision.gameObject.GetComponent<Health>();
        Health TakeDamage = collision.gameObject.GetComponent<Health>();
        
        
        if (targetHealth != null)
        {
          
            targetHealth.photonView.RPC("TakeDamage", RpcTarget.All, damage);
           
        
        

        if (damage > targetHealth.currentHealth)
        {
            Debug.Log("Kill confirmed. Updating kill count.");
           GameManager.instance.kills++;
           GameManager.instance.SetHashes();
           //GameManager.instance.UpdateScore();
        }
        }
        GetComponent<PhotonView>().RPC("DestroyObject", RpcTarget.All);
        GameObject effect = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effect, 0.2f);
        
    }
}


    
   
}

