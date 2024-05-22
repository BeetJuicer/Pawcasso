using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuckForce : MonoBehaviour
{
	public float damage = 100.0f;                                       // The amount of damage to be applied (only for Direct damage type)
	public float speed = 10.0f;                                         // The speed at which this projectile will move
	public float initialForce = 1000.0f;                                // The force to be applied to the projectile initially
	public float lifetime = 30.0f;                                      // The maximum time (in seconds) before the projectile is destroyed

	private float lifeTimer = 0.0f;                                     // The timer to keep track of how long this projectile has been in existence

	[SerializeField] private float radius;
	[SerializeField] private float power;
	[SerializeField] private float suckTime;
	private bool suck;
	private float suckTimer;



	void Start()
	{
		// Add the initial force to rigidbody
		GetComponent<Rigidbody>().AddRelativeForce(0, 0, initialForce);
	}

	// Update is called once per frame
	void Update()
	{
		// Update the timer
		lifeTimer += Time.deltaTime;

		// Destroy the projectile if the time is up
		if (lifeTimer >= lifetime)
		{
			suck = true;
		}

		if(suck)
        {
			suckTimer += Time.deltaTime;
			if(suckTimer >= suckTime)
			{
				// Destroy this projectile
				Destroy(gameObject);
			}
        }

		// Make the projectile move
		if (initialForce == 0)      // Only if initial force is not being used to propel this projectile
			GetComponent<Rigidbody>().velocity = transform.forward * speed;
	}

	void FixedUpdate()
	{
		if (suck)
		{
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
			foreach (Collider hit in colliders)
			{
				if (hit && hit.transform != transform && hit.TryGetComponent<Rigidbody>(out Rigidbody rb))
				{
					print(hit.gameObject.name);
					Vector3 difference = hit.transform.position - transform.position;
					rb.AddForce(-difference.normalized * power, ForceMode.Force);
				}
			}
		}
	}

	void OnCollisionEnter(Collision col)
	{
		// If the projectile collides with something, call the Hit() function
		suck = true;
	}


	// Modify the damage that this projectile can cause
	public void MultiplyDamage(float amount)
	{
		damage *= amount;
	}

	// Modify the inital force
	public void MultiplyInitialForce(float amount)
	{
		initialForce *= amount;
	}
}

