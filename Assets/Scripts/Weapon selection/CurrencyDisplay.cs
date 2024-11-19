using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CurencyDisplay : MonoBehaviour
{
    [SerializeField] public Text currencyDisplay; 
    public int currency;

    
    private void Update()
    {
    
        int currency = PlayerPrefs.GetInt("Currency");
        currencyDisplay.text = currency.ToString();
        
        
        
    }
       
}