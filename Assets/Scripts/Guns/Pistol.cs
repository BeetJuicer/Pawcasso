using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;
using UnityEngine.UI;

public class Pistol : PaintGun
{
	[Header("Dash Ability")]
	[SerializeField] private ExampleCharacterController characterController;
	[SerializeField] private DashParticles dashParticles;

	// Charging
	private float chargeTimer;
	private bool isCharging;
	[SerializeField] private Slider chargeSlider;

	protected override void Start()
    {
		base.Start();
		chargeSlider.gameObject.SetActive(false);
	}

    // Update is called once per frame
    protected override void Update()
    {
		base.Update();
	}

	protected override void CheckInputs()
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

}
