using UnityEngine;
using UnityEngine.UI;

public class GunManager : MonoBehaviour
{
    // Array to hold icons of guns
    public Image[] weaponIcons;

    // Reference to the WeaponSystem script
    [SerializeField] private GameObject wsObj;
    private WeaponSystem weaponSystem;
    private int currentIcon;

    void Start()
    {
        // Deactivate all weapon icons initially
        foreach (Image icon in weaponIcons)
        {
            icon.gameObject.SetActive(false);
        }

        weaponSystem = wsObj.GetComponent<WeaponSystem>();

        // Activate the starting weapon icon
        if (weaponSystem != null)
        {
            weaponIcons[weaponSystem.WeaponIndex].gameObject.SetActive(true);
        }
        else
        {
            print("WeaponSystem is null !");
        }
    }

    void Update()
    {
        // Ensure weaponSystem reference is not null
        if (weaponSystem != null)
        {
            print("current: " + weaponSystem.WeaponIndex);
            if(currentIcon != weaponSystem.WeaponIndex)
            {
                weaponIcons[currentIcon].gameObject.SetActive(false);
                currentIcon = weaponSystem.WeaponIndex;
                weaponIcons[currentIcon].gameObject.SetActive(true);
            }
        }
        else
        {
            print("Weapon system null !");
        }
    }
}
