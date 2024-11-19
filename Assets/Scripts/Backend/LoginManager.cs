using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;   

public class LoginManager : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public Button loginButton;

    void Start()
    {
        
        loginButton.onClick.AddListener(() =>
        {
            StartCoroutine(Main.Instance.web.Login(usernameInput.text, passwordInput.text));
            //StartCoroutine(Main.Instance.web.Login("NewUser", "12345678"));
        });
    }

}
