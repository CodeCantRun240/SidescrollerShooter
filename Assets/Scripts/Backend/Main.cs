using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public static Main Instance;
    public Web web;
    
    void Start()
    {
        Instance = this;
        web = GetComponent<Web>(); 
    }

    
}
