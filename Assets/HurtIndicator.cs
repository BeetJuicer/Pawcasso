using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtIndicator : MonoBehaviour
{

	[SerializeField] GameObject hurtElement;
    public void Hurt(Vector3 player, Vector3 hurterPosition)
	{

		// Calculate the direction of the hurt based on position difference
		Vector3 myPosition = transform.position; // Assuming this holds your object's position
		Vector3 hurtDirection = hurterPosition - player;

		// Determine rotation based on direction vector
		float angle = Mathf.Atan2(hurtDirection.y, hurtDirection.x) * Mathf.Rad2Deg;

		// Set rotation based on quadrants
		if (hurtDirection.x > 0 && hurtDirection.z > 0)
		{
			angle = -45; // Right and Up (hurt from front right)
		}
		else if (hurtDirection.x < 0 && hurtDirection.z > 0)
		{
			angle = 45; // Left and Up (hurt from front left)
		}
		else if (hurtDirection.x > 0 && hurtDirection.z < 0)
		{
			angle = -135; // Right and Down (hurt from behind right)
		}
		else if (hurtDirection.x < 0 && hurtDirection.z < 0)
		{
			angle = 135; // Left and Down (hurt from behind left)
		}
		else if (hurtDirection.x == 0 && hurtDirection.z > 0)
		{
			angle = 0; // Up only (hurt from front)
		}
		else if (hurtDirection.x == 0 && hurtDirection.z < 0)
		{
			angle = 180; // Down only (hurt from behind)
		}
		else if (hurtDirection.x > 0)
		{
			angle = -90; // Right only (hurt from directly right)
		}
		else
		{
			angle = 90; // Left only (hurt from directly left)
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
