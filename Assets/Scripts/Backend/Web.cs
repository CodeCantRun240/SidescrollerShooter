using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Realtime;
public class Web : MonoBehaviour
{
    public MessageBox messageBox;

    public IEnumerator Login(string username, string password)
{
    WWWForm form = new WWWForm();
    form.AddField("loginUser", username);
    form.AddField("loginPassword", password);

    using UnityWebRequest www = UnityWebRequest.Post("https://www.sidescrollergame.online/SidescrollerShooterBackend/Login.php", form);
    //using UnityWebRequest www = UnityWebRequest.Post("http://localhost/SidescrollerShooterBackend/Login.php", form);
    yield return www.SendWebRequest();

    if (www.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError(www.error);
    }
    else
    {
        string jsonResponse = www.downloadHandler.text;
        Debug.Log(jsonResponse);
        
        // Parse the JSON response into the PlayerData object
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonResponse);

        // Check if the login was successful
        if (playerData.status == "success")
        {
            // Store basic player info
            PlayerPrefs.SetString("Username", playerData.username);
            PlayerPrefs.SetInt("Currency", playerData.currency);
            PlayerPrefs.SetString("Weapon", playerData.currentWeapon);
            PlayerPrefs.SetString("Token", playerData.token);
            PlayerPrefs.Save();
            
            
            
            if (playerData.weapons != null && playerData.weapons.Count > 0)
            {
    
                foreach (WeaponData weapon in playerData.weapons)
                {
                    PlayerPrefs.SetInt(weapon.name + "Ownership", weapon.unlocked ? 1 : 0);
                    Debug.Log($"{weapon.name}Ownership set to {PlayerPrefs.GetInt(weapon.name + "Ownership")}");
                }
                PlayerPrefs.Save();
            }
            else
            {
            Debug.Log("playerData.weapons is null or empty");
            }
           
           

            
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            Debug.LogError("Login failed: " + playerData.message);

            string customMessage = "Login failed";
            messageBox.ShowMessage(playerData.message);
            
        }
    }
}

    public IEnumerator RegisterUser(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("loginUser", username);
        form.AddField("loginPassword", password);
        
        using UnityWebRequest www = UnityWebRequest.Post("https://sidescrollergame.online/SidescrollerShooterBackend/RegisterUser.php", form);
        //using UnityWebRequest www = UnityWebRequest.Post("http://192.168.0.65:8080/SidescrollerShooterBackend/RegisterUser.php", form);
        
        yield return www.SendWebRequest();

        string jsonResponse = www.downloadHandler.text;
        Debug.Log(jsonResponse);

        PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonResponse);

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
            messageBox.ShowMessage(playerData.message);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            messageBox.ShowMessage(playerData.message);
 
        }
    }
    
}

[System.Serializable]
public class WeaponData
{
    public string name;
    public bool unlocked;
}

[System.Serializable]
public class PlayerData
{
    public string status; 
    public string username;
    public int currency;
    public string currentWeapon;
    public List<WeaponData> weapons = new List<WeaponData>();
    public string token;
    public string message;
}



