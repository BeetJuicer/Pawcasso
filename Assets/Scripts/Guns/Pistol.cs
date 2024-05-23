using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;
using UnityEngine.UI;

public class Pistol : PaintGun
{
	private Brush brush;
	[SerializeField] private ExampleCharacterController characterController;
	[SerializeField] private DashParticles dashParticles;

	[SerializeField] private GunColor gunColor = GunColor.Red;

    #region Gun Variables
    // shoot speed
    [SerializeField] private float rateOfFire;
	private float actualROF;
	private float fireTimer;

	// Charging
	[SerializeField] private float maxChargeTime;
	private float chargeTimer;
	private bool isChargeAllowed;
	private bool isCharging;
	[SerializeField] private Slider chargeSlider;


	// Ammo
	private bool canFire = true;
	[SerializeField] private int ammoCapacity;
	[SerializeField] private float reloadTime;

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

	// Crosshairs
	public bool showCrosshair = true;                   // Whether or not the crosshair should be displayed
	public Texture2D crosshairTextureHorizontal;                  // The texture used to draw the crosshair
	public Texture2D crosshairTextureVertical;                  // The texture used to draw the crosshair
	public int crosshairLength = 10;                    // The length of each crosshair line
	public int crosshairWidth = 4;                      // The width of each crosshair line
	public float startingCrosshairSize = 10.0f;         // The gap of space (in pixels) between the crosshair lines (for weapon inaccuracy)
	private float currentCrosshairSize;                 // The gap of space between crosshair lines that is updated based on weapon accuracy in realtime

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

		CurrentAmmo = ammoCapacity;

		chargeSlider.gameObject.SetActive(false);
		currentCrosshairSize = startingCrosshairSize;
	}

    // Update is called once per frame
    void Update()
    {
		// Calculate the current accuracy for this weapon
		currentAccuracy = Mathf.Lerp(currentAccuracy, accuracy, accuracyRecoverRate * Time.deltaTime);
		// Calculate the current crosshair size.  This is what causes the crosshairs to grow and shrink dynamically while shooting
		//currentCrosshairSize = startingCrosshairSize + (accuracy - currentAccuracy) * 0.1f;

		// Update the fireTimer
		fireTimer += Time.deltaTime;

		CheckInputs();

		if(isCharging)
        {
			chargeSlider.gameObject.SetActive(true);
			chargeTimer += Time.deltaTime;
			chargeSlider.value = (chargeTimer / maxChargeTime);
			print(chargeSlider.value);
        }
        else
        {
			chargeSlider.gameObject.SetActive(false);
		}


		// Reload if the weapon is out of ammo
		if (CurrentAmmo <= 0)
			Reload();
	}

	void CheckInputs()
    {
		// Cancel the charge if there is any, and shoot if allowed.
		if (Input.GetButtonDown("Fire1"))
		{
			// cancel the charge count. disable charging unless the user actually lets go of the right mouse button.
			isCharging = false;
			chargeTimer = 0;

			if (fireTimer >= actualROF && canFire)
				Fire();
		}

		// start counting the charge if allowed.
		if (Input.GetButtonDown("Fire2"))
		{
			isCharging = true;
		}

		// count the final charge time.
		if (Input.GetButtonUp("Fire2"))
		{
			//calculate the charge level depending on the amount of time charged.
			if(chargeTimer >= maxChargeTime)
            {
				characterController.EnterChargeState(1);
				dashParticles.PlayDash(1);
            }

			isCharging = false;
			chargeTimer = 0;
		}
	}

	void Fire()
	{
		//Wish to add to comboTimer;
		ScoreManager.Instance.WishForCombo(gunColor);

		// Reset the fireTimer to 0 (for ROF)
		fireTimer = 0.0f;

		// First make sure there is ammo
		if (CurrentAmmo <= 0)
		{
			DryFire();
			return;
		}

		// Subtract 1 from the current ammo
		CurrentAmmo--;

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

			//damage the enemy
			if (hit.collider.gameObject.TryGetComponent<DemoEnemyControls>(out DemoEnemyControls enemy))
            {
				enemy.TakeDamage(damage, hit.point, Quaternion.identity);
            }

			// Damage
			hit.collider.gameObject.SendMessageUpwards("ChangeHealth", -damage, SendMessageOptions.DontRequireReceiver);

			// Hit Effects -TODO: place a paint impact particle here
			if (hitEffect != null)
				Instantiate(hitEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
			//else
				//print("no hit effect gameObject!");        

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
		CurrentAmmo = ammoCapacity;
		fireTimer = -reloadTime;
		GetComponent<AudioSource>().PlayOneShot(reloadSound);

		// Send a messsage so that users can do other actions whenever this happens
		SendMessageUpwards("OnEasyWeaponsReload", SendMessageOptions.DontRequireReceiver);
	}

	void OnGUI()
	{
		if (showCrosshair)
		{
			// Hold the location of the center of the screen in a variable
			Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);

			// Draw the crosshairs based on the weapon's inaccuracy
			// Left
			Rect leftRect = new Rect(center.x - crosshairLength - currentCrosshairSize, center.y - (crosshairWidth / 2), crosshairLength, crosshairWidth);
			GUI.DrawTexture(leftRect, crosshairTextureHorizontal, ScaleMode.StretchToFill);
			// Right
			Rect rightRect = new Rect(center.x + currentCrosshairSize, center.y - (crosshairWidth / 2), crosshairLength, crosshairWidth);
			GUI.DrawTexture(rightRect, crosshairTextureHorizontal, ScaleMode.StretchToFill);
			// Top
			Rect topRect = new Rect(center.x - (crosshairWidth / 2), center.y - crosshairLength - currentCrosshairSize, crosshairWidth, crosshairLength);
			GUI.DrawTexture(topRect, crosshairTextureVertical, ScaleMode.StretchToFill);
			// Bottom
			Rect bottomRect = new Rect(center.x - (crosshairWidth / 2), center.y + currentCrosshairSize, crosshairWidth, crosshairLength);
			GUI.DrawTexture(bottomRect, crosshairTextureVertical, ScaleMode.StretchToFill);
		}
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
