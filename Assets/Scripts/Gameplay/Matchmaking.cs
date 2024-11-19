using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class Matchmaking : MonoBehaviourPunCallbacks, IPunObservable
{
    public TextMeshProUGUI playerCountText;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI gameCountDown;
    public int countdownTime = 5;
    public GameObject playerCount;
    public GameObject countdown; 

    private int currentCountdown;
    private Coroutine countdownCoroutine;
    public PhotonView photonView;
    
    public int matchDuration = 180; 
    public int currentMatchDuration;
    //private bool countdownRunning = false;
    public GameObject finalize;
    public TextMeshProUGUI roomNameText;
    private UIhandler uiHandler;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (playerCountText == null || countdownText == null)
        {
            Debug.LogError("Player Count Text or Countdown Text is not assigned in the Matchmaking script.");
        }

        UpdatePlayerCountText();

        bool isPrivateMatch = PlayerPrefs.GetInt("IsPrivateMatch", 1) == 1;
        uiHandler = FindObjectOfType<UIhandler>();
        if (isPrivateMatch)
        {   
        string roomName = PlayerPrefs.GetString("RoomName", "Unknown Room");
        roomNameText.text = "Room ID: " + roomName;
        }
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
       
        UpdatePlayerCountText();
        HandlePlayerCountChange();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        
        UpdatePlayerCountText();
        HandlePlayerCountChange();
    }

    void UpdatePlayerCountText()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            playerCountText.text = "Players in Room: " + playerCount;
        }
    }

    void HandlePlayerCountChange()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        if (playerCount >= 2 )
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (countdownCoroutine == null)
                {
                    photonView.RPC("StartCountdownRPC", RpcTarget.All);
                }
                else
                {
                    photonView.RPC("ResetCountdownRPC", RpcTarget.All);
                }
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("StopCountdownRPC", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void StartCountdownRPC()
    {
        currentCountdown = countdownTime;
        if (countdownCoroutine == null)
        {
            countdownCoroutine = StartCoroutine(Countdown());
        }
    }

    [PunRPC]
    void ResetCountdownRPC()
    {
        if (countdownCoroutine != null)
    {
        StopCoroutine(countdownCoroutine);
        countdownCoroutine = null;
    }

    currentCountdown = countdownTime;
    countdownCoroutine = StartCoroutine(Countdown());
    }

    [PunRPC]
    void StopCountdownRPC()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
            countdownText.text = "";
        }
    }

    IEnumerator Countdown()
    {
        while (currentCountdown > 0)
        {
            countdownText.text = "Match starts in: " + currentCountdown;
            yield return new WaitForSeconds(1f);
            currentCountdown--;
            Debug.Log(currentCountdown);

            if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
            {
                countdownText.text = "";
                yield break;
            }
        }

        countdownText.text = "Match starts now!";
        
        StartMatchRPC();
    }

    [PunRPC]
    void StartMatchRPC()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        Weapon[] weapons = FindObjectsOfType<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            weapon.photonView.RPC("StartMatch", RpcTarget.All);
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerController>().StartMatch();
        }
        playerCount.SetActive(false);
        countdown.SetActive(false); 

        if (PhotonNetwork.IsMasterClient)        
        {
            currentMatchDuration = matchDuration;
            StartCoroutine(MatchCountdown());
            
        }
    }

    [PunRPC]
    void MatchCountdownRPC()
    {

    currentMatchDuration = matchDuration;
    countdownCoroutine = StartCoroutine(MatchCountdown());
    }


    private IEnumerator MatchCountdown()
    {
        
        while (currentMatchDuration > 0)
        {
            int minutes = currentMatchDuration / 60;
            int seconds = currentMatchDuration % 60;
            //gameCountDown.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
            photonView.RPC("UpdateCountdown", RpcTarget.All, minutes, seconds);


            yield return new WaitForSeconds(1f);
            
            if (PhotonNetwork.IsMasterClient)
            {
            currentMatchDuration--;
            }
            
            
            
            
        }

        photonView.RPC("UpdateCountdown", RpcTarget.All, 0, 0);
        photonView.RPC("MatchmakingEndMatch", RpcTarget.All);
    }   

    [PunRPC]
    private void UpdateCountdown(int minutes, int seconds)
    {
        gameCountDown.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }

    [PunRPC]
    public void MatchmakingEndMatch()
    {
        Weapon[] weapons = FindObjectsOfType<Weapon>();
       
        foreach (Weapon weapon in weapons)
        {
            weapon.photonView.RPC("MatchEnd", RpcTarget.All);
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerController>().MatchEnd();
        }
        finalize.SetActive(true);
        GameManager.instance.ManagerEndMatch();
        //finalizeScoreBoard = GetComponent<ScoreBoard>();
        //finalizeScoreBoard.UpdateFinalScoreboardUI();
        //scoreBoard.UpdateFinalScoreboardUI();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(currentMatchDuration);
            }
            else
            {
                this.currentMatchDuration = (int)stream.ReceiveNext();
            }
        }
        
    }
}
