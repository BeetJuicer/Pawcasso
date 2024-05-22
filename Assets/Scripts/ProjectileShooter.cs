using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;

public class ProjectileShooter : MonoBehaviour
{
	private Brush brush;
	[SerializeField] private GameObject projectile;
	[SerializeField] private Transform projectileSpawnSpot;

	[SerializeField] private GunColor gunColor = GunColor.Red;

    #region Gun Variables
    // shoot speed
    [SerializeField] private float rateOfFire;
    [SerializeField] private float shotsPerRound;
	private float actualROF;
	private float fireTimer;
	private float launchTimer;
	[SerializeField] private float launchCooldown;

	// Charging
	private float startChargeTime;
	private float finalChargeTime;
	[SerializeField] private float maxChargeTime;
	private bool isChargeAllowed = true;
	private float boostStartTime;

	// Ammo
	private bool canFire = true;
	[SerializeField] private int ammoCapacity;
	[SerializeField] private float reloadTime;
	private int currentAmmo;

	// Accuracy
	[SerializeField] private float range;
	[SerializeField] private float accuracy = 80f;
	[SerializeField] private float accuracyRecoverRate = 0.1f;
	[SerializeField] private float accuracyDropPerShot = 1f;
	private float currentAccuracy;

	// Damage
	[SerializeField] private float damage;

	// RayCast
	[SerializeField] private Transform raycastStartSpot;
	
	// FX
	[SerializeField] private Transform muzzleEffectsPosition;
	[SerializeField] private GameObject hitEffect;
	[SerializeField] private AudioClip fireSound;    // Sound to play when the weapon is fired
	[SerializeField] private AudioClip reloadSound;  // Sound to play when the weapon is reloading
	[SerializeField] private AudioClip dryFireSound; // Sound to play when the user tries to fire but is out of ammo
	[SerializeField] private GameObject[] muzzleEffects; // Particles for muzzleEffects to choose randomly.
    #endregion

    private void Start()
    {
		brush = GetComponent<BrushMono>().brush;

		if (rateOfFire != 0)
			actualROF = 1.0f / rateOfFire;
		else
			actualROF = 0.01f;

		currentAmmo = ammoCapacity;
	}

    // Update is called once per frame
    void Update()
    {
		// Calculate the current accuracy for this weapon
		currentAccuracy = Mathf.Lerp(currentAccuracy, accuracy, accuracyRecoverRate * Time.deltaTime);

		// Update the fireTimer
		fireTimer += Time.deltaTime;
		launchTimer += Time.deltaTime;

		CheckInputs();

		// Reload if the weapon is out of ammo
		if (currentAmmo <= 0)
			Reload();
	}

	void CheckInputs()
    {
		// Cancel the charge if there is any, and shoot if allowed.
		if (Input.GetButtonDown("Fire1"))
		{
			// cancel the charge count. disable charging unless the user actually lets go of the right mouse button.
			isChargeAllowed = false;
			finalChargeTime = 0;

			if (fireTimer >= actualROF && canFire)
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

	void Fire()
	{
		//Wish to add to comboTimer;
		ScoreManager.Instance.WishForCombo(gunColor);

		// Reset the fireTimer to 0 (for ROF)
		fireTimer = 0.0f;

		// First make sure there is ammo
		if (currentAmmo <= 0)
		{
			DryFire();
			return;
		}

		// Subtract 1 from the current ammo
		currentAmmo--;

		// Fire 
		// Fire once for each shotPerRound value
		for (int i = 0; i < shotsPerRound; i++)
		{
			// Calculate accuracy for this shot
			float accuracyVary = (100 - currentAccuracy) / 1000;
			Vector3 direction = raycastStartSpot.forward;
			direction.x += UnityEngine.Random.Range(-accuracyVary, accuracyVary);
			direction.y += UnityEngine.Random.Range(-accuracyVary, accuracyVary);
			direction.z += UnityEngine.Random.Range(-accuracyVary, accuracyVary);
			currentAccuracy -= accuracyDropPerShot;
			if (currentAccuracy <= 0.0f)
				currentAccuracy = 0.0f;

			// The ray that will be used for this shot
			Ray ray = new Ray(raycastStartSpot.position, direction);
			RaycastHit hit;


			Debug.DrawRay(raycastStartSpot.position, direction, Color.red, 2f);

			if (Physics.Raycast(ray, out hit, range))
			{
				//paint the paintable.
				if (hit.collider.gameObject.TryGetComponent<PaintTarget>(out PaintTarget paintTarget))
				{
					PaintTarget.PaintObject(paintTarget, hit.point, hit.normal, brush);
				}

				// Damage
				hit.collider.gameObject.SendMessageUpwards("ChangeHealth", -damage, SendMessageOptions.DontRequireReceiver);


				// Hit Effects
				//if (makeHitEffects)
				//{
				//	foreach (GameObject hitEffect in hitEffects)
				//	{
				//		if (hitEffect != null)
				//			Instantiate(hitEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
				//	}
				//}
			}
		}


		/*
		// Muzzle flash effects
		GameObject muzfx = muzzleEffects[Random.Range(0, muzzleEffects.Length)];
		if (muzfx != null)
			Instantiate(muzfx, muzzleEffectsPosition.position, muzzleEffectsPosition.rotation);
		*/
		// Play the gunshot sound
		GetComponent<AudioSource>().PlayOneShot(fireSound);
	}
	public void Launch(int chargeLevel)
	{
		// Reset the fire timer to 0 (for ROF)
		launchTimer = 0.0f;

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
		

		//// Recoil
		//if (recoil)
		//	Recoil();

		// Muzzle flash effects
		//if (makeMuzzleEffects)
		//{
		//	GameObject muzfx = muzzleEffects[Random.Range(0, muzzleEffects.Length)];
		//	if (muzfx != null)
		//		Instantiate(muzfx, muzzleEffectsPosition.position, muzzleEffectsPosition.rotation);
		//}

		//// Instantiate shell props
		//if (spitShells)
		//{
		//	GameObject shellGO = Instantiate(shell, shellSpitPosition.position, shellSpitPosition.rotation) as GameObject;
		//	shellGO.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(shellSpitForce + Random.Range(0, shellForceRandom), 0, 0), ForceMode.Impulse);
		//	shellGO.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(shellSpitTorqueX + Random.Range(-shellTorqueRandom, shellTorqueRandom), shellSpitTorqueY + Random.Range(-shellTorqueRandom, shellTorqueRandom), 0), ForceMode.Impulse);
		//}

		// Play the gunshot sound
		//GetComponent<AudioSource>().PlayOneShot(fireSound);
	}
	void DryFire()
	{
		GetComponent<AudioSource>().PlayOneShot(dryFireSound);
	}

	void Reload()
	{
		currentAmmo = ammoCapacity;
		fireTimer = -reloadTime;
		GetComponent<AudioSource>().PlayOneShot(reloadSound);

		// Send a messsage so that users can do other actions whenever this happens
		SendMessageUpwards("OnEasyWeaponsReload", SendMessageOptions.DontRequireReceiver);
	}

	/* REcoil()
	// Recoil FX.  This is the "kick" that you see when the weapon moves back while firing
	void Recoil()
	{
		// No recoil for AIs
		if (!playerWeapon)
			return;

		// Make sure the user didn't leave the weapon model field blank
		if (weaponModel == null)
		{
			Debug.Log("Weapon Model is null.  Make sure to set the Weapon Model field in the inspector.");
			return;
		}

		// Calculate random values for the recoil position and rotation
		float kickBack = Random.Range(recoilKickBackMin, recoilKickBackMax);
		float kickRot = Random.Range(recoilRotationMin, recoilRotationMax);

		// Apply the random values to the weapon's postion and rotation
		weaponModel.transform.Translate(new Vector3(0, 0, -kickBack), Space.Self);
		weaponModel.transform.Rotate(new Vector3(-kickRot, 0, 0), Space.Self);
	}
	*/

}
