using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorialUnlocker : MonoBehaviour
{
    [SerializeField] private GameObject[] activate;
    [SerializeField] private GameObject[] deactivate;
    [Space(2)]
    [SerializeField] private int gunIndexUnlock;
    [SerializeField] private bool unlockGun;
    [Space(1)]
    [SerializeField] private WeaponSystem ws;

    private void OnTriggerEnter(Collider other)
    {
        foreach(GameObject go in activate)
        {
            go.SetActive(true);
        }

        foreach(GameObject go in deactivate)
        {
            go.SetActive(false);
        }

        if (unlockGun)
            ws.UnlockGun(gunIndexUnlock);

    }
}
