using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject instructionPanel;
    public void OnClick_ToggleSettings()
    {   
        settingsPanel.SetActive(true);
    }
    public void OnClick_TurnOffSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void OnClick_ToggleInstruction()
    {   
        instructionPanel.SetActive(true);
    }
    public void OnClick_TurnOffInstruction()
    {
        instructionPanel.SetActive(false);
    }
    
}