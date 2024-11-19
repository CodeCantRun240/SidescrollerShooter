using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Unity.VisualScripting;
using UnityEngine.UI;
using System.Linq; 
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Photon.Pun.Demo.Cockpit;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [HideInInspector]
    public int kills = 0;
    [HideInInspector]
    public int deaths = 0;
    private Weapon weapon;
    public TMP_Text bulletCountText;
    public TMP_Text finalPositionText;
    public Image score;
    public TMP_Text scoreCount;
    public Image opponentScore;
    public TMP_Text opponentScoreCount;
    public TMP_Text resultText;
    //public PhotonView photonView;
    public Matchmaking matchmaking;
    public PhotonView photonView;
    public GameObject midGameMenu;
    public TMP_Text rewardText;

    Color victory = new Color(13/255f, 0f, 1f, 1f); 
    Color defeat = new Color(1f, 0f, 13/255f, 1f);

    [SerializeField]
    private Texture2D cursorTexture;
    private UIhandler uiHandler;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        weapon = FindObjectOfType<Weapon>();
        Vector2 hotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2 + 0.5f);
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
        uiHandler = FindObjectOfType<UIhandler>();
    }

    void Update()
    {
        
        int highestKills = GetHighestKills();
        if (weapon != null)
        {
            UpdateBulletUI(weapon.bulletCount, weapon.maxBulletCount);
        }

    
        score.fillAmount = kills / 12f;
        scoreCount.text = kills.ToString();

        opponentScore.fillAmount = highestKills / 12f;
        opponentScoreCount.text = highestKills.ToString();


        if (kills >= 12 || highestKills >= 12)
        {
            matchmaking.matchDuration = 0;
        }
        
        if (Input.GetKeyDown(KeyCode.M)) 
        {
            ToggleMidGameMenu();
        }

    }

    public void UpdateBulletUI(int bulletCount, int maxBulletCount)
    {
        if (bulletCountText != null)
        {
            bulletCountText.text = bulletCount.ToString() + "/" + maxBulletCount.ToString();
        }
    }
    
        public void SetHashes(){
        try
        {
            
            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash["kills"] = kills;
            hash["deaths"] = deaths;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        }
        catch   
        {
            
        }
    }

    private int GetHighestKills()
    {
    int highestKills = 0;

    foreach (var player in PhotonNetwork.PlayerList)
    {
        // Skip the local player
        if (player == PhotonNetwork.LocalPlayer)
        {
            continue;
        }

        int playerKills = player.CustomProperties.ContainsKey("kills") ? (int)player.CustomProperties["kills"] : 0;
        if (playerKills > highestKills)
        {
            highestKills = playerKills;
        }
    }

    return highestKills;
}
    
    [PunRPC]
    public void ManagerEndMatch()
    {
        List<PlayerStatsData> playerStats = new List<PlayerStatsData>();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            int kills = player.CustomProperties.ContainsKey("kills") ? (int)player.CustomProperties["kills"] : 0;
            int deaths = player.CustomProperties.ContainsKey("deaths") ? (int)player.CustomProperties["deaths"] : 0;
            playerStats.Add(new PlayerStatsData(player.NickName, kills, deaths, player));
        }

        // Sort players by kills (descending), then by deaths (ascending)
        playerStats = playerStats.OrderByDescending(p => p.kills).ThenBy(p => p.deaths).ToList();

        // Display final positions
        DisplayFinalPositions(playerStats);

        int localPlayerPosition = GetLocalPlayerPosition(playerStats);
        // Send final positions to each player
        /*for (int i = 0; i < playerStats.Count; i++)
        {
        string position = GetOrdinal(i + 1);
        Photon.Realtime.Player photonPlayer = PhotonNetwork.PlayerList.FirstOrDefault(p => p.NickName == playerStats[i].playerName);

        if (photonPlayer != null)
        {
            // Call the RPC on the player's client
            photonView.RPC("UpdatePlayerPositionRPC", photonPlayer, position);
        }
    }*/
    }

    private void DisplayFinalPositions(List<PlayerStatsData> playerStats)
    {
        bool isPrivateMatch = PlayerPrefs.GetInt("IsPrivateMatch", 1) == 1;
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        int localPlayerPosition = GetLocalPlayerPosition(playerStats);
        if (playerCount <= 3){
            switch (localPlayerPosition)
            {
                case 0:
                    finalPositionText.text = "1st place";
                    resultText.text = "Victory";
                    resultText.color = victory;
                    AddCurrency(300);
                    rewardText.text = "+300";
                    break;
                case 1:
                    finalPositionText.text = "2nd place";
                    resultText.text = "Defeat";
                    resultText.color = defeat;
                    AddCurrency(200);
                    rewardText.text = "+200";
                    break;
                case 2: 
                    finalPositionText.text = "3rd place";
                    resultText.text = "Defeat";
                    resultText.color = defeat;
                    AddCurrency(100);
                    rewardText.text = "+100";
                    break;

            }
            
        }
        else{
        switch (localPlayerPosition)
        {
            case 0:
                finalPositionText.text = "1st place";
                resultText.text = "Victory";
                resultText.color = victory;
                if (!isPrivateMatch)
                {
                AddCurrency(500);
                rewardText.text = "+500";
                }
                else
                {
                rewardText.text = "+500";
                }
                break;
            case 1:
                finalPositionText.text = "2nd place";
                resultText.text = "Victory";
                resultText.color = victory;
                if (!isPrivateMatch)
                {
                AddCurrency(350);
                rewardText.text = "+350";
                }
                else
                {
                rewardText.text = "+350";
                }
                break;
            case 2:
                finalPositionText.text = "3rd place";
                resultText.text = "Victory";
                resultText.color = victory;
                if (!isPrivateMatch)
                {
                AddCurrency(250);
                rewardText.text = "+200";
                }
                else
                {
                rewardText.text = "+200";
                }
                break;
            case 3:
                finalPositionText.text = "4th place";
                resultText.text = "Defeat";
                resultText.color = defeat;
                if (!isPrivateMatch)
                {
                AddCurrency(150);
                rewardText.text = "+150";
                }
                else
                {
                rewardText.text = "+150";
                }
                break;
            case 4:
                finalPositionText.text = "5th place";
                resultText.text = "Defeat";
                resultText.color = defeat;
                if (!isPrivateMatch)
                {
                AddCurrency(50);
                rewardText.text = "+50";
                }
                else
                {
                rewardText.text = "+50";
                }
                break;
            case 5:
                finalPositionText.text = "6th place";
                resultText.text = "Defeat";
                resultText.color = defeat;
                AddCurrency(0);
                rewardText.text = "+0";
                break;
        }
        }


    }
        public void AddCurrency(int amount)
    {
        int currency = PlayerPrefs.GetInt("Currency");
        currency += amount;
        PlayerPrefs.SetInt("Currency", currency);
        StartCoroutine(UpdateCurrencyDatabase());
    }
        IEnumerator UpdateCurrencyDatabase()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", PlayerPrefs.GetString("Username"));
        form.AddField("token", PlayerPrefs.GetString("Token"));
        int currency = PlayerPrefs.GetInt("Currency");    
        form.AddField("currency", currency);
        Debug.Log("Username: " + PlayerPrefs.GetString("Username"));
        Debug.Log("Token: " + PlayerPrefs.GetString("Token"));
        Debug.Log("Updated currency : " + currency);
        
        UnityWebRequest www = UnityWebRequest.Post("https://www.sidescrollergame.online/SidescrollerShooterBackend/UpdateCurrency.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Currency updated successfully");
        }
    }
    private int GetLocalPlayerPosition(List<PlayerStatsData> playerStats)
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;
        for (int i = 0; i < playerStats.Count; i++)
        {
            if (playerStats[i].player == localPlayer)
            {
                return i;
            }
        }
        return -1; 
    }

    public void OnClick_LeaveRoom()
    {
    PhotonNetwork.LeaveRoom();
    SceneManager.LoadScene("SampleScene");
    }

    /*[PunRPC]
    void UpdatePlayerPositionRPC(string position)
    {
    // Display the position on the player's screen
        finalPositionText.text = position;
    }
    private string GetOrdinal(int number)
    {
    if (number % 100 >= 11 && number % 100 <= 13)
    {
        return number + "th";
    }
    switch (number % 10)
    {
        case 1: return number + "st";
        case 2: return number + "nd";
        case 3: return number + "rd";
        default: return number + "th";
    }
}*/

    public void ToggleMidGameMenu()
    {
        midGameMenu.SetActive(!midGameMenu.activeSelf);
    }
}
 














public class PlayerStatsData
{
    public Player player;
    public string playerName;
    public int kills;
    public int deaths;

    public PlayerStatsData( string playerName, int kills, int deaths, Player player)
    {
        this.player = player;
        this.playerName = playerName;
        this.kills = kills;
        this.deaths = deaths;
    }
        
    
}
