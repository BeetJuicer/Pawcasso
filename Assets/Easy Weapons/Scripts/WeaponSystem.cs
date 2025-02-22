﻿/// <summary>
/// WeaponSystem.cs
/// Author: MutantGopher
/// This script manages weapon switching.  It's recommended that you attach this to a parent GameObject of all your weapons, but this is not necessary.
/// This script allows the player to switch weapons in two ways, by pressing the numbers corresponding to each weapon, or by scrolling with the mouse.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using AYellowpaper.SerializedCollections;

public class WeaponSystem : MonoBehaviour
{
    public int unlockedWeaponIndex { get; private set; } = -1; // which weapon we're allowed to use
    public GameObject[] weapons;                // The array that holds all the weapons that the player has
    public int startingWeaponIndex = 0;         // The weapon index that the player will start with
    public int WeaponIndex { get; private set; }                    // The current index of the active weapon
    public int CurrentAmmo { get; private set; }
    public int MaxAmmo { get; private set; }
    public float ReloadTimeCurrent { get; private set; }
    public float ReloadTimeTotal { get; private set; }
    public bool IsReloading { get; private set; }

    [SerializeField] protected AudioSource reloadSound;

    // Gauges
    public Dictionary<GunColor, int> gauges { get; private set; }
    public Dictionary<GunColor, int> gaugesMax { get; private set; }

    public void ResetGauge(GunColor color)
    {
        gauges[color] = 0;
    }
    public void SubtractFromGauge(GunColor color, int amount)
    {
        gauges[color] -= amount;
    }

    private void InitializeGauges()
    {
        gauges = new Dictionary<GunColor, int>
        {
            { GunColor.Red, 0 },
            { GunColor.Blue, 0 },
            { GunColor.Yellow, 0 }
        };

        gaugesMax = new Dictionary<GunColor, int>
        {
            { GunColor.Red, 48 },
            { GunColor.Blue, 24 },
            { GunColor.Yellow, 12 }
        };
    }

    public void UnlockGun(int index)
    {
        unlockedWeaponIndex = Mathf.Max(index, unlockedWeaponIndex);
        SetActiveWeapon(index);
    }

    private void Awake()
    {
        InitializeGauges();
    }

    // Use this for initialization
    void Start()
    {
        // Make sure the starting active weapon is the one selected by the user in startingWeaponIndex
        WeaponIndex = startingWeaponIndex;
        // Start be deactivating all weapons
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }

        SetActiveWeapon(WeaponIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance().IsPaused)
            return;

        // Allow the user to instantly switch to any weapon
        if (Input.GetButtonDown("Weapon 1"))
            SetActiveWeapon(0);
        if (Input.GetButtonDown("Weapon 2"))
            SetActiveWeapon(1);
        if (Input.GetButtonDown("Weapon 3"))
            SetActiveWeapon(2);

        // Allow the user to scroll through the weapons
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            NextWeapon();
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
            PreviousWeapon();

        CurrentAmmo = weapons[WeaponIndex].GetComponent<PaintGun>().CurrentAmmo;
        MaxAmmo = weapons[WeaponIndex].GetComponent<PaintGun>().MaxAmmo;
        ReloadTimeTotal = weapons[WeaponIndex].GetComponent<PaintGun>().reloadTime;

        //if firetimer less than 0, it means reloading
        if (weapons[WeaponIndex].GetComponent<PaintGun>().FireTimer < 0f)
        {
            if (!IsReloading)
            {
                reloadSound.Play();
                IsReloading = true;
            }
            ReloadTimeCurrent = -weapons[WeaponIndex].GetComponent<PaintGun>().FireTimer;
        }
        else
        {
            IsReloading = false;
            reloadSound.Stop();
        }
    }

    public void SetActiveWeapon(int index)
    {
        if (unlockedWeaponIndex < index)
        {
            return;
        }

        // Make sure this weapon exists before trying to switch to it
        if (index >= weapons.Length || index < 0)
        {
            Debug.LogWarning("Tried to switch to a weapon that does not exist.  Make sure you have all the correct weapons in your weapons array.");
            return;
        }

        // Send a messsage so that users can do other actions whenever this happens
        SendMessageUpwards("OnEasyWeaponsSwitch", SendMessageOptions.DontRequireReceiver);

        // Make sure the WeaponIndex references the correct weapon
        WeaponIndex = index;

        // Make sure beam game objects aren't left over after weapon switching
        //weapons[index].GetComponent<Weapon>().StopBeam();

        // Start be deactivating all weapons
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(false);
        }

        // Activate the one weapon that we want
        weapons[index].SetActive(true);
    }

    public void NextWeapon()
    {
        if (WeaponIndex + 1 > unlockedWeaponIndex)
            return;
        WeaponIndex++;
        if (WeaponIndex > weapons.Length - 1)
            WeaponIndex = 0;
        SetActiveWeapon(WeaponIndex);
    }

    public void PreviousWeapon()
    {
        WeaponIndex--;
        if (WeaponIndex < 0)
            WeaponIndex = unlockedWeaponIndex;
        SetActiveWeapon(WeaponIndex);
    }
}
