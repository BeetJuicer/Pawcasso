using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
	private Brush brush;

    #region Gun Variables
    // shoot speed
    [SerializeField] private float rateOfFire;
	private float actualROF;
	private float fireTimer;

	// Charging
	private float heat;
	[SerializeField] private float maxWarmup;

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
	}

    // Update is called once per frame
    void Update()
    {
		/*flow:
			getbuttonup Fire1 - shoot.

			getbuttondown Fire2 - start counting until:
					-getbuttonup Fire2 || getbuttonup Fire1

			if get button up fire2
				Do Boost. Depending on amount of charge.
		 
		 */

		// Calculate the current accuracy for this weapon
		currentAccuracy = Mathf.Lerp(currentAccuracy, accuracy, accuracyRecoverRate * Time.deltaTime);

		// Update the fireTimer
		fireTimer += Time.deltaTime;

		if (fireTimer >= actualROF && canFire)
		{
			if (Input.GetButtonDown("Fire1"))
			{
				Fire();
			}
		}

		// Reload if the weapon is out of ammo
		if (currentAmmo <= 0)
			Reload();
	}

	void Fire()
	{
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
			// Warmup heat
			//float damage = power;
			//if (warmup)
			//{
			//	damage *= heat * powerMultiplier;
			//	heat = 0.0f;
			//}

			//paint the paintable.
			if (hit.collider.gameObject.TryGetComponent<PaintTarget>(out PaintTarget paintTarget))
			{
				PaintTarget.PaintObject(paintTarget, hit.point, hit.normal, brush);
			}

			// Damage
			hit.collider.gameObject.SendMessageUpwards("ChangeHealth", -damage, SendMessageOptions.DontRequireReceiver);

			// Hit Effects -TODO: place a paint impact particle here
			if (hitEffect != null)
				Instantiate(hitEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
			else
				print("no hit effect gameObject!");        

			// Add force to the object that was hit
			//if (hit.rigidbody)
			//{
			//	hit.rigidbody.AddForce(ray.direction * power * forceMultiplier);
			//}
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
