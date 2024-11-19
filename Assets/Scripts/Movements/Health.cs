using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;

public class Health : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] public float maxHealth ;
    [SerializeField] public float maxArmor;
    public float currentHealth;
    private float currentArmor;
    [SerializeField] private GameObject ragDollRight;
    [SerializeField] private GameObject ragDollLeft;
    [SerializeField] private GameObject propRight;
    [SerializeField] private GameObject propLeft;
    public SpriteRenderer torsoSprite;
    public PlayerController playerController;
    public Image borderBar;
    public Image healthBar;
    public Image armorBar;
    public Image enemyBorderBar;
    public Image enemyHealthBar;
    public Image enemyArmorBar;
    private bool propped = false;

    public float regenDelay = 5f; 
    public float regenRate = 30f; 

    private float timeSinceLastDamage; 
    private bool right;
    public Manager gameManager;
    public Camera characterCamera;
    private bool spawned;

    public Transform[] spawnPoints;
    private bool dead;

    //private PlayerController playerController;
    
    private void Start()
    {
        gameManager = FindObjectOfType<Manager>();
        playerController = GetComponent<PlayerController>();

        PhotonView photonView = GetComponent<PhotonView>();

        currentHealth = maxHealth;
        currentArmor = maxArmor;
         
        bool isFacingRight = playerController.facingRight;
        if (photonView.IsMine)
        {
            //torsoSprite = GetComponent<SpriteRenderer>();
            
            
            borderBar.gameObject.SetActive(true);
            healthBar.gameObject.SetActive(true);
            armorBar.gameObject.SetActive(true);
            
        }
        else
        {
            enemyBorderBar.gameObject.SetActive(true);
            enemyHealthBar.gameObject.SetActive(true);
            enemyArmorBar.gameObject.SetActive(true);
        }

        
       
    }

    private void Update()
    {
        
        timeSinceLastDamage += Time.deltaTime;
        if (timeSinceLastDamage >= regenDelay)
        {
            currentArmor += regenRate * Time.deltaTime;
            if (currentArmor > maxArmor)
            {
                currentArmor = maxArmor;
            }
        }
        if (photonView.IsMine)
        {
        
        
        //if (Input.GetKeyDown(KeyCode.H))
        //{
           //TakeDamage(20f);
        //}
        healthBar.fillAmount = currentHealth / maxHealth;
        armorBar.fillAmount = currentArmor / maxArmor;

        enemyHealthBar.fillAmount = currentHealth / 100f;
        enemyArmorBar.fillAmount = currentArmor / 100f;
        
        }

        enemyHealthBar.fillAmount = currentHealth / 100f;
        enemyArmorBar.fillAmount = currentArmor / 100f;
        
        
        
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        timeSinceLastDamage = 0f;

        currentArmor -= damage;
        if (currentArmor <= 0)
        {
            currentArmor = 0;
           currentHealth -= damage;
           if (currentHealth <= 0)
           {
               currentHealth = 0;
           }
        }
        
        Debug.Log("current health:" + currentHealth);
        Debug.Log("damage taken:" + damage);

        StartCoroutine(DamageEffect());

        if (currentHealth == 0)
        {
            
            
            photonView.RPC("Die", RpcTarget.AllBuffered);
            
        }
    }

    [PunRPC]
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        healthBar.fillAmount = currentHealth / maxHealth;
    }
    [PunRPC]
    IEnumerator DamageEffect()
    {
        
        torsoSprite.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        torsoSprite.color = Color.white;
    }
    
    [PunRPC]
    void Die()
    {
        //Debug.Log("Player Died");
        regenDelay = 10f;
        playerController.torso.SetActive(false);
        playerController.MatchEnd();
        if(photonView.IsMine)
        {
            if (playerController.facingRight == true){
                ragDollRight.SetActive(true);
            }
            else
            {
                ragDollLeft.SetActive(true);
            }
            borderBar.gameObject.SetActive(false);
            healthBar.gameObject.SetActive(false);
            armorBar.gameObject.SetActive(false);  
            
            if(!dead)
            {
                dead = true;
                GameManager.instance.deaths++;
                GameManager.instance.SetHashes();
            }
            
        }
        else
        {
            if (playerController.facingRight == true ){
                if (!propped){
                GameObject prop = Instantiate(propRight, transform.position, transform.rotation);
                Destroy(prop, 5f);
                
                propped = true;
                }
            }
            else
            {
                if (!propped){
                GameObject prop = Instantiate(propLeft, transform.position, transform.rotation);
                Destroy(prop, 5f);
                
                propped = true;
                }
            }
            enemyBorderBar.gameObject.SetActive(false);
            enemyHealthBar.gameObject.SetActive(false);
            enemyArmorBar.gameObject.SetActive(false);
        }
        
        gameObject.layer = LayerMask.NameToLayer("Ragdoll");
        
        
        photonView.RPC("Respawn", RpcTarget.AllBuffered);
        
        
    
    }
    [PunRPC]
    void StartRespawnCoroutine()
    {
        StartCoroutine(Respawn());
    }

    [PunRPC]
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f); 
        playerController.StartMatch();
        photonView.RPC("ResetPlayer", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ResetPlayer()
    {

        if (photonView.IsMine)
        {
            dead = false;
            // Teleport player to a new spawn point
            Transform newSpawnPoint = gameManager.GetRandomSpawnPoint();
            transform.position = newSpawnPoint.position;
            transform.rotation = newSpawnPoint.rotation;
            gameObject.layer = LayerMask.NameToLayer("Friendly");
            // Reset health and armor
            currentHealth = maxHealth;
            currentArmor = maxArmor;

            // Enable player components
            
            playerController.torso.SetActive(true);
            borderBar.gameObject.SetActive(true);
            healthBar.gameObject.SetActive(true);
            armorBar.gameObject.SetActive(true);
            ragDollRight.SetActive(false);
            ragDollLeft.SetActive(false);

            
        }
        else
        {
            propped = false;
            Transform newSpawnPoint = gameManager.GetRandomSpawnPoint();
            transform.position = newSpawnPoint.position;
            transform.rotation = newSpawnPoint.rotation;
            gameObject.layer = LayerMask.NameToLayer("Default");
            currentHealth = maxHealth;
            currentArmor = maxArmor;
            playerController.torso.SetActive(true);
            enemyBorderBar.gameObject.SetActive(true);
            enemyHealthBar.gameObject.SetActive(true);
            enemyArmorBar.gameObject.SetActive(true);
            
            
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    if (stream.IsWriting)
    {
        stream.SendNext(currentHealth);
        stream.SendNext(currentArmor);
        stream.SendNext(playerController.facingRight);
        
    }
    else if (stream.IsReading)
    {
        currentHealth = (float)stream.ReceiveNext();
        currentArmor = (float)stream.ReceiveNext();
        playerController.facingRight = (bool)stream.ReceiveNext();

        
    }



    
}
}