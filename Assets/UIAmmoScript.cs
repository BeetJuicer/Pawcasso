using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIAmmoScript : MonoBehaviour
{
    [SerializeField] private WeaponSystem weaponSystem;
    [SerializeField] private TextMeshProUGUI currentAmmo;
    [SerializeField] private TextMeshProUGUI maxAmmo;

    private void Update()
    {
        if (weaponSystem.IsReloading)
        {
            currentAmmo.text = "...";
            maxAmmo.text = "...";
        }
        else
        {
            currentAmmo.text = weaponSystem.CurrentAmmo.ToString();
            maxAmmo.text = weaponSystem.MaxAmmo.ToString();
        }
    }
}
