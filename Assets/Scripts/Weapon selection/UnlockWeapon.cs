using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockWeapon : MonoBehaviour
{
    public List<GameObject> lockedButtons;  // A list of GameObjects representing locked overlays for each weapon.
    public List<string> weaponNames;        // A list of weapon names corresponding to the locked buttons.

    void Update()
    {
        UpdateWeaponLockStatus();
    }

    void UpdateWeaponLockStatus()
    {
        // Loop through all weapons and update the locked state
        for (int i = 0; i < weaponNames.Count; i++)
        {
            string weaponName = weaponNames[i] + "Ownership"; // Get PlayerPrefs key for the weapon ownership
            bool isUnlocked = PlayerPrefs.GetInt(weaponName) == 1;

            //Debug.Log(weaponNames[i] + " is unlocked: " + isUnlocked);
            lockedButtons[i].SetActive(!isUnlocked);  // Set active if weapon is locked, inactive if unlocked
        }
    }
}