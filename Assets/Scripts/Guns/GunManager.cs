using UnityEngine;
using UnityEngine.UI;

public class GunManager : MonoBehaviour
{
    // Array to hold icons of guns
    public Image[] weaponIcons;

    // Reference to the WeaponSystem script
    public WeaponSystem weaponSystem;

    void Start()
    {
        // Deactivate all weapon icons initially
        foreach (Image icon in weaponIcons)
        {
            icon.gameObject.SetActive(false);
        }

        // Activate the starting weapon icon
        if (weaponSystem != null)
        {
            int initialIndex = weaponSystem.WeaponIndex;
            if (initialIndex >= 0 && initialIndex < weaponIcons.Length)
            {
                weaponIcons[initialIndex].gameObject.SetActive(true);
            }
        }
    }

    void Update()
    {
        // Ensure weaponSystem reference is not null
        if (weaponSystem != null)
        {
            int currentWeaponIndex = weaponSystem.WeaponIndex;

            // Deactivate all weapon icons
            for (int i = 0; i < weaponIcons.Length; i++)
            {
                if (i == currentWeaponIndex)
                {
                    weaponIcons[i].gameObject.SetActive(true);
                }
                else
                {
                    weaponIcons[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
