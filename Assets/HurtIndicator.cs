using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtIndicator : MonoBehaviour
{

	[SerializeField] GameObject hurtElement;
    public void Hurt(Vector3 position)
	{

		// Calculate the direction of the hurt based on position difference
		Vector3 myPosition = transform.position; // Assuming this holds your object's position
		Vector3 hurtDirection = position - myPosition;

		// Determine rotation based on direction vector
		float angle = Mathf.Atan2(hurtDirection.y, hurtDirection.x) * Mathf.Rad2Deg;

		// Set rotation based on quadrants
		if (hurtDirection.x > 0 && hurtDirection.y > 0)
		{
			angle = 45; // Right and Up (hurt from bottom right)
		}
		else if (hurtDirection.x < 0 && hurtDirection.y > 0)
		{
			angle = 135; // Left and Up (hurt from bottom left)
		}
		else if (hurtDirection.x > 0 && hurtDirection.y < 0)
		{
			angle = 315; // Right and Down (hurt from top right)
		}
		else if (hurtDirection.x < 0 && hurtDirection.y < 0)
		{
			angle = 225; // Left and Down (hurt from top left)
		}
		else if (hurtDirection.x == 0 && hurtDirection.y > 0)
		{
			angle = 90; // Up only (hurt from directly below)
		}
		else if (hurtDirection.x == 0 && hurtDirection.y < 0)
		{
			angle = 270; // Down only (hurt from directly above)
		}
		else if (hurtDirection.x > 0)
		{
			angle = 0; // Right only (hurt from directly right)
		}
		else
		{
			angle = 180; // Left only (hurt from directly left)
		}

		// Apply the rotation to the hurtIndicator
		hurtElement.transform.rotation = Quaternion.Euler(0, 0, angle);
		hurtElement.SetActive(true);
	}

	public void Hurt()
	{
		hurtElement.SetActive(true);
	}
}
