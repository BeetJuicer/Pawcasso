using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;

public class ProjectileShooter : PaintGun
{
	[Header("Projectile Launching")]
	[SerializeField] private GameObject projectile;
	[SerializeField] private Transform projectileSpawnSpot;
	[SerializeField] protected float launchCooldown;
	protected float launchTimer;


	protected override void Start()
    {
		base.Start();
	}


    // Update is called once per frame
    protected override void Update()
    {
		base.Update();
		launchTimer += Time.deltaTime;
	}

	protected override void CheckInputs()
    {
		// Cancel the charge if there is any, and shoot if allowed.
		if (Input.GetButton("Fire1"))
		{
			// cancel the charge count. disable charging unless the user actually lets go of the right mouse button.
			isChargeAllowed = false;
			finalChargeTime = 0;

			if (FireTimer >= actualROF && canFire)
				Fire();
		}

		// start counting the charge if allowed.
		if (isChargeAllowed && Input.GetButtonDown("Fire2"))
		{
			startChargeTime = Time.time;
		}

		// count the final charge time.
		if (Input.GetButtonUp("Fire2") && launchTimer >= launchCooldown)
		{
			print("launchTimer: " + launchTimer + " / launchCooldown: " + launchCooldown);
			// charge not allowed means the user cancelled the charge using the left mouse button. No dash.
			if (!isChargeAllowed)
			{
				isChargeAllowed = true;
			}
			else
			{
				finalChargeTime = Time.time - startChargeTime;
				Launch(1);//TODO: implement an actual chargeTime
			}

		}
	}


	public void Launch(int chargeLevel)
	{
		// Reset the fire timer to 0 (for ROF)
		launchTimer = 0.0f;
		Recoil();

		// Instantiate the projectile
		if (projectile != null)
		{
			GameObject proj = Instantiate(projectile, projectileSpawnSpot.position, projectileSpawnSpot.rotation) as GameObject;

			// Warmup heat
			float initialForceMultiplier = (chargeLevel == 1) ? 1.0f : 1.5f;
			proj.SendMessage("MultiplyInitialForce", chargeLevel * initialForceMultiplier, SendMessageOptions.DontRequireReceiver);
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
