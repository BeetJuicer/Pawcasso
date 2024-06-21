using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;

public class ProjectileShooter : PaintGun
{
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
		if (Input.GetButton("Fire1"))
		{
			if (FireTimer >= actualROF && canFire)
				Fire();
		}
	}
	

}
