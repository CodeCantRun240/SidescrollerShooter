using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;
using System.Runtime.CompilerServices;


public class ScoreBoard : MonoBehaviour
{
    public GameObject playerHolder;  
    [Header("Options")]
    public float refreshRate = 1f;  
    [Header("UI")]
    public GameObject[] slots;
    [Space]
    public GameObject[] finalizeSlots;
    [Space]
    public TextMeshProUGUI[] killTexts;
    public TextMeshProUGUI[] deathTexts;
    public TextMeshProUGUI[] nameTexts;

    public TextMeshProUGUI[] finalizeKillTexts;
    public TextMeshProUGUI[] finalizeDeathTexts;
    public TextMeshProUGUI[] finalizeNameTexts;

    private void Start()
    {
        InvokeRepeating(nameof(Refresh), 1f, refreshRate);
        InvokeRepeating(nameof(UpdateFinalScoreboardUI), 3f, refreshRate);
    }

    public void Refresh()
    {
        foreach (var slot in slots)
        {
            slot.SetActive(false);
        }
        var sortedPlayerList = 
            (from player in PhotonNetwork.PlayerList orderby player.GetScore() descending select player).ToList();

        int i = 0;
        foreach (var player in sortedPlayerList )
        {
            slots[i].SetActive(true);

            nameTexts[i].text = player.NickName;
            killTexts[i].text = player.GetScore().ToString();

            if (player.CustomProperties["kills"] != null)
            {
                killTexts[i].text = player.CustomProperties["kills"] + "";
            }
            else
            {
                killTexts[i].text = "0";
                
            }
            if (player.CustomProperties["deaths"] != null)
            {
                deathTexts[i].text = player.CustomProperties["deaths"] + "";
            }
            else
            {
                
                deathTexts[i].text = "0";
            }
            
            i++;
        }
        
    }
    private void Update()
    {
        playerHolder.SetActive(Input.GetKey(KeyCode.Tab));
    }

    public void UpdateFinalScoreboardUI()
    {
        foreach (var finalizeSlot in finalizeSlots)
        {
            finalizeSlot.SetActive(false);
        }
        var sortedPlayerList = 
            (from player in PhotonNetwork.PlayerList orderby player.GetScore() descending select player).ToList();

        int i = 0;

        foreach (var player in sortedPlayerList )
        {
            finalizeSlots[i].SetActive(true);

            finalizeNameTexts[i].text = player.NickName;
            finalizeKillTexts[i].text = player.GetScore().ToString();

            if (player.CustomProperties["kills"] != null)
            {
                finalizeKillTexts[i].text = player.CustomProperties["kills"] + "";
            }
            else
            {
                finalizeKillTexts[i].text = "0";
                
            }
            if (player.CustomProperties["deaths"] != null)
            {
                finalizeDeathTexts[i].text = player.CustomProperties["deaths"] + "";
            }
            else
            {
                
                finalizeDeathTexts[i].text = "0";
            }
            
            i++;
        }
    }
}
