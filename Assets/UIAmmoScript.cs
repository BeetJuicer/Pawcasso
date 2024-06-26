using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIAmmoScript : MonoBehaviour
{
    [SerializeField] private WeaponSystem weaponSystem;
    [SerializeField] private TextMeshProUGUI currentAmmo;
    [SerializeField] private TextMeshProUGUI maxAmmo;
    [SerializeField] private GameObject textDash;
    [SerializeField] private Slider reloadSlider;

    private void Start()
    {
        reloadSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (weaponSystem.IsReloading)
        {
            reloadSlider.gameObject.SetActive(true);
            currentAmmo.gameObject.SetActive(false);
            maxAmmo.gameObject.SetActive(false);
            textDash.SetActive(false);

            reloadSlider.value = weaponSystem.ReloadTimeCurrent / weaponSystem.ReloadTimeTotal;
        }
        else
        {
            reloadSlider.gameObject.SetActive(false);
            currentAmmo.gameObject.SetActive(true);
            maxAmmo.gameObject.SetActive(true);
            textDash.SetActive(true);

            currentAmmo.text = weaponSystem.CurrentAmmo.ToString();
            maxAmmo.text = weaponSystem.MaxAmmo.ToString();
        }
    }
}
