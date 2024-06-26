using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class ShootProjectile : Skill
{
	[Header("Projectile Launching")]
	[SerializeField] private GameObject projectile;
	[SerializeField] private Transform projectileSpawnSpot;
	[SerializeField] protected float launchCooldown;

    protected override void Awake()
    {
		base.Awake();
    }

    private void Update()
    {
		//if we have enough in the gauge and the user presses a key
		if (Input.GetKeyDown(key) &&
			requiredColors[primaryOne] <= ws.gauges[primaryOne] &&
			requiredColors[primaryTwo] <= ws.gauges[primaryTwo])
        {
			Launch();
			ws.SubtractFromGauge(primaryOne, requiredColors[primaryOne]);
			ws.SubtractFromGauge(primaryTwo, requiredColors[primaryTwo]);
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

		// Play the launch fire sound
		//GetComponent<AudioSource>().PlayOneShot(fireSound);
	}
}
