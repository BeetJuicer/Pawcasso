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

	protected override void Start()
    {
		base.Start();
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
			if (FireTimer >= actualROF && canFire)
				Fire();
		}
	}	

}
