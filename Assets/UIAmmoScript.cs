using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIAmmoScript : MonoBehaviour
{
    [SerializeField] private WeaponSystem weaponSystem;

    private void Update()
    {
        GetComponent<TextMeshProUGUI>().SetText("Ammo: " + weaponSystem.CurrentAmmo + " / " + weaponSystem.MaxAmmo);
    }
}
