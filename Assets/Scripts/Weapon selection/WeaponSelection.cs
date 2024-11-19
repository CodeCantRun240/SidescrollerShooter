using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class WeaponSelection : MonoBehaviour
{
    
    public string weaponName;
    public Button equipButton;
    public Button unlockButton;
    public int price;
    public MessageBox messageBox;
    public void OnClick_SelectAK()
    {
        ShowEquipButton();
        weaponName = "AK";
    }
    public void OnClick_SelectQQ9()
    {
        ShowEquipButton();
        weaponName = "QQ9";
    }

    public void OnClick_UnlockQQ9()
    {
        weaponName = "QQ9";
        price = 350;
        ShowUnlockButton();
        
    }
    public void Onclick_EquipWeapon()
    {
        PlayerPrefs.SetString("Weapon", weaponName);    
        StartCoroutine(UpdateWeaponDatabase());
        string weaponEquip = "Equipped " + weaponName;
        messageBox.ShowMessage(weaponEquip);
        
    }
    public void OnClick_UnlockWeapon()
    {
        int currency = PlayerPrefs.GetInt("Currency");
        if (currency >= price && PlayerPrefs.GetInt(weaponName + "Ownership") == 0)
        {
            currency -= price;
            string weaponOwnership = weaponName + "Ownership";
            PlayerPrefs.SetInt(weaponOwnership, 1) ;
            PlayerPrefs.SetInt("Currency", currency);
            ShowEquipButton();
            StartCoroutine(UpdateCurrencyDatabase());
            StartCoroutine(UpdateWeaponOwnershipDatabase());
            string weaponUnlock = "Weapon unlocked sucessfully";
            messageBox.ShowMessage(weaponUnlock);
        }
        else
        {
            string notEnough = "Not enough currency";
            messageBox.ShowMessage(notEnough);
        }
    }
    IEnumerator UpdateWeaponDatabase()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", PlayerPrefs.GetString("Username"));
        form.AddField("token", PlayerPrefs.GetString("Token"));
        form.AddField("weapon", weaponName);
        Debug.Log("Username: " + PlayerPrefs.GetString("Username"));
        Debug.Log("Token: " + PlayerPrefs.GetString("Token"));
        Debug.Log("New weapon: " + weaponName);

        UnityWebRequest www = UnityWebRequest.Post("https://www.sidescrollergame.online/SidescrollerShooterBackend/UpdateDB.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Weapon updated successfully");
        }
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

    IEnumerator UpdateWeaponOwnershipDatabase()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", PlayerPrefs.GetString("Username"));
        form.AddField("token", PlayerPrefs.GetString("Token"));
        form.AddField("weapon", weaponName);

        UnityWebRequest www = UnityWebRequest.Post("https://www.sidescrollergame.online/SidescrollerShooterBackend/UpdateWeaponOwnership.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Ownership updated successfully");
        }
    }
    public void ShowEquipButton()
    {
        equipButton.gameObject.SetActive(true);
        unlockButton.gameObject.SetActive(false);
    }
    public void ShowUnlockButton()
    {
        unlockButton.gameObject.SetActive(true);
        equipButton.gameObject.SetActive(false);
    }


}   