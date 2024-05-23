using UnityEngine;
using UnityEngine.UI;

public class GunManager : MonoBehaviour
{
    // Reference to the UI Image component that displays the gun
    public Image gunImage;

    // Array of gun sprites to display
    public Sprite[] gunSprites;

    // Index of the current gun sprite
    private int currentGunIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the UI image with the first gun sprite
        if (gunImage != null && gunSprites.Length > 0)
        {
            gunImage.sprite = gunSprites[currentGunIndex];
        }
    }

    // Function to change the displayed gun to the next one
    public void ChangeToNextGun()
    {
        // Increment the current gun index
        currentGunIndex++;

        // If the index exceeds the array length, loop back to the beginning
        if (currentGunIndex >= gunSprites.Length)
        {
            currentGunIndex = 0;
        }

        // Update the UI image with the new gun sprite
        if (gunImage != null)
        {
            gunImage.sprite = gunSprites[currentGunIndex];
        }
    }

    // Function to change the displayed gun to the previous one
    public void ChangeToPreviousGun()
    {
        // Decrement the current gun index
        currentGunIndex--;

        // If the index is less than zero, loop to the end of the array
        if (currentGunIndex < 0)
        {
            currentGunIndex = gunSprites.Length - 1;
        }

        // Update the UI image with the new gun sprite
        if (gunImage != null)
        {
            gunImage.sprite = gunSprites[currentGunIndex];
        }
    }
}
