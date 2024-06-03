using InfimaGames.LowPolyShooterPack;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{

    [SerializeField] private Inventory inventory;
    [SerializeField] private TMP_Text ammoText;

    void Start()
    {
        ammoText = GetComponent<TMP_Text>();
        UpdateAmmoText();
    }

 
    public void UpdateAmmoText()
    {
       
    }
}
