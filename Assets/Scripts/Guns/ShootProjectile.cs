using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
	[Header("Projectile Launching")]
	[SerializeField] private GameObject projectile;
	[SerializeField] private Transform projectileSpawnSpot;
	[SerializeField] protected float launchCooldown;
	[SerializeField] private KeyCode key;
	private bool canShoot = true;

    private void Update()
    {
		if(Input.GetKeyDown(key) && canShoot)
        {
			Launch();
        }
    }

    public void Launch()
	{

		// Instantiate the projectile
		if (projectile != null)
		{
			GameObject proj = Instantiate(projectile, projectileSpawnSpot.position, projectileSpawnSpot.rotation) as GameObject;

			// Warmup heat
			proj.SendMessage("MultiplyInitialForce",  1, SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			Debug.Log("Projectile to be instantiated is null.  Make sure to set the Projectile field in the inspector.");
		}

		//launch muzzle effect
		//if (muzfx != null)
		//Instantiate(muzfx, muzzleEffectsPosition.position, muzzleEffectsPosition.rotation);

		// Play the launch fire sound
		//GetComponent<AudioSource>().PlayOneShot(fireSound);
	}
}
