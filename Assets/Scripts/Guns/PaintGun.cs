using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PaintGun : MonoBehaviour
{
    public int CurrentAmmo { get; protected set; }
    public int MaxAmmo;

	[Header("General")]
	[SerializeField] protected Transform raycastStartSpot;
	[SerializeField] protected GameObject weaponModel;    // The actual mesh for this weapon
	[SerializeField] protected float damage;
	[SerializeField] protected LayerMask whatIsNoCollision;

	[Header("Ammo")]
	protected bool canFire = true;
	protected int ammoCapacity;
	public float reloadTime;

	[Header("Shoot Speed")]
	[SerializeField] protected float rateOfFire;
	[SerializeField] protected float shotsPerRound;
	protected float actualROF;
	public float FireTimer { get; private set; }

	[Header("Accuracy")]
	[SerializeField] protected float range;
	[SerializeField] protected float accuracy = 80f;
	[SerializeField] protected float accuracyRecoverRate = 0.1f;
	[SerializeField] protected float accuracyDropPerShot = 1f;
	protected float currentAccuracy;

	[Header("Charging")]
	[SerializeField] protected float maxChargeTime;
	protected float startChargeTime;
	protected float finalChargeTime;
	protected bool isChargeAllowed = true;
	protected float boostStartTime;

	[Header("Paint")]
	[SerializeField] protected GunColor gunColor = GunColor.Red;
	protected Brush brush;

	[Header("FX")]
	[SerializeField] protected Transform muzzleEffectsPosition;
	[SerializeField] protected AudioClip fireSound;    // Sound to play when the weapon is fired
	[SerializeField] protected AudioClip reloadSound;  // Sound to play when the weapon is reloading
	[SerializeField] protected AudioClip dryFireSound; // Sound to play when the user tries to fire but is out of ammo
	[Space(1)]
	[SerializeField] protected GameObject hitEffect;
	[SerializeField] protected GameObject bulletTrail;
	[SerializeField] protected GameObject[] muzzleEffects; // Particles for muzzleEffects to choose randomly.

	[Header("Recoil")]
	[SerializeField] protected bool recoil = true;                          // Whether or not this weapon should have recoil
	[SerializeField] protected float recoilKickBackMin = 0.1f;              // The minimum distance the weapon will kick backward when fired
	[SerializeField] protected float recoilKickBackMax = 0.3f;              // The maximum distance the weapon will kick backward when fired
	[SerializeField] protected float recoilRotationMin = 0.1f;              // The minimum rotation the weapon will kick when fired
	[SerializeField] protected float recoilRotationMax = 0.25f;             // The maximum rotation the weapon will kick when fired
	[SerializeField] protected float recoilRecoveryRate = 0.01f;            // The rate at which the weapon recovers from the recoil displacement

	[Header("Crosshair")]
	// Crosshairs
	[SerializeField] protected bool showCrosshair = true;                   // Whether or not the crosshair should be displayed
	[SerializeField] protected Texture2D crosshairTextureHorizontal;                  // The texture used to draw the crosshair
	[SerializeField] protected Texture2D crosshairTextureVertical;                  // The texture used to draw the crosshair
	[SerializeField] protected int crosshairLength = 10;                    // The length of each crosshair line
	[SerializeField] protected int crosshairWidth = 4;                      // The width of each crosshair line
	[SerializeField] protected float startingCrosshairSize = 10.0f;         // The gap of space (in pixels) between the crosshair lines (for weapon inaccuracy)
	protected float currentCrosshairSize;                 // The gap of space between crosshair lines that is updated based on weapon accuracy in realtime

    protected virtual void Start()
    {
		brush = GetComponent<BrushMono>().brush;

		if (rateOfFire != 0)
			actualROF = 1.0f / rateOfFire;
		else
			actualROF = 0.01f;

		ammoCapacity = MaxAmmo;
		CurrentAmmo = ammoCapacity;

		currentCrosshairSize = startingCrosshairSize;
	}

	protected virtual void Update()
    {
		// Calculate the current accuracy for this weapon
		currentAccuracy = Mathf.Lerp(currentAccuracy, accuracy, accuracyRecoverRate * Time.deltaTime);
		// Calculate the current crosshair size.  This is what causes the crosshairs to grow and shrink dynamically while shooting
		//currentCrosshairSize = startingCrosshairSize + (accuracy - currentAccuracy) * 0.1f;

		// Update the fireTimer
		FireTimer += Time.deltaTime;

		CheckInputs();

		// Reload if the weapon is out of ammo
		if (CurrentAmmo <= 0)
			Reload();

		// Recoil Recovery
		if (recoil)
		{
			weaponModel.transform.position = Vector3.Lerp(weaponModel.transform.position, transform.position, recoilRecoveryRate * Time.deltaTime);
			weaponModel.transform.rotation = Quaternion.Lerp(weaponModel.transform.rotation, transform.rotation, recoilRecoveryRate * Time.deltaTime);
		}
	}

	protected virtual void CheckInputs() {
	}

    protected void Fire()
	{
		print("firing");

		//Wish to add to comboTimer;
		ScoreManager.Instance.WishForCombo(gunColor);

		// Reset the fireTimer to 0 (for ROF)
		FireTimer = 0.0f;

		// First make sure there is ammo
		if (CurrentAmmo <= 0)
		{
			DryFire();
			return;
		}

		// Subtract 1 from the current ammo
		CurrentAmmo--;
		Recoil();

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

			GameObject trail = Instantiate(bulletTrail, muzzleEffectsPosition.position, muzzleEffectsPosition.rotation);
			if (Physics.Raycast(ray, out hit, range, ~whatIsNoCollision))
			{
				//note: duplicated the lookat for the trails because I only want to consider wall and enemies as hits.

				//paint the paintable.
				if (hit.collider.gameObject.TryGetComponent<PaintTarget>(out PaintTarget paintTarget))
				{
					PaintTarget.PaintObject(paintTarget, hit.point, hit.normal, brush);
					if (hitEffect != null)
						Instantiate(hitEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));

					//if we hit something, make the trail move towards that hit. Otherwise, it'll go wherever the muzzlePosition is pointed
					trail.GetComponent<Transform>().LookAt(hit.point);

				}

				//damage the enemy
				if (hit.collider.gameObject.TryGetComponent<DemoEnemyControls>(out DemoEnemyControls enemy))
				{
					enemy.TakeDamage(damage, hit.point, Quaternion.identity, gunColor);

					//if we hit something, make the trail move towards that hit. Otherwise, it'll go wherever the muzzlePosition is pointed
					trail.GetComponent<Transform>().LookAt(hit.point);
				}
			}

		}

		// Muzzle flash effects
		GameObject muzfx = muzzleEffects[Random.Range(0, muzzleEffects.Length)];
		if (muzfx != null)
		{
			Instantiate(muzfx, muzzleEffectsPosition.position, muzzleEffectsPosition.rotation, muzzleEffectsPosition);
		}

		// Play the gunshot sound
		GetComponent<AudioSource>().PlayOneShot(fireSound);
	}

	protected void DryFire()
    { 
		GetComponent<AudioSource>().PlayOneShot(dryFireSound);
	}

	protected void Reload()
    {
		//activate reload indicator UI
		CurrentAmmo = ammoCapacity;
		FireTimer = -reloadTime;
		GetComponent<AudioSource>().PlayOneShot(reloadSound);

		// Send a messsage so that users can do other actions whenever this happens
		SendMessageUpwards("OnEasyWeaponsReload", SendMessageOptions.DontRequireReceiver);
	}

	// Recoil FX.  This is the "kick" that you see when the weapon moves back while firing
	protected void Recoil()
	{
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
}
