using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   

public class RegisterManager : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public InputField confirmPasswordInput;
    public Button registerButton;
    public MessageBox messageBox;

    void Start()
    {
        
        registerButton.onClick.AddListener(() =>
        {
            if(passwordInput.text != confirmPasswordInput.text)
            {
                Debug.Log("Passwords do not match"); 
                string message = "Passwords do not match";
                messageBox.ShowMessage(message);   
                return;
            }
            StartCoroutine(Main.Instance.web.RegisterUser(usernameInput.text, passwordInput.text));
            
        });
    }

}
