using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject backButton;
    public GameObject newAccountButton;
    public Button returnButton;
    public Button showRegisterButton;
    

    public void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        backButton.SetActive(true);
        newAccountButton.SetActive(false);

    }

    public void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        backButton.SetActive(false);
        newAccountButton.SetActive(true);
    }

    public void Onclick_ShowRegisterPanel()
    {
        showRegisterButton.onClick.AddListener(ShowRegisterPanel);
    }

    public void Onclick_ShowLoginPanel()
    {
        returnButton.onClick.AddListener(ShowLoginPanel);
    }
   
}
