﻿/// <summary>
/// Health.cs
/// Author: MutantGopher
/// This is a sample health script.  If you use a different script for health,
/// make sure that it is called "Health".  If it is not, you may need to edit code
/// referencing the Health component from other scripts
/// </summary>

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
	private bool canDie = true;                 // Whether or not this health can die
	public bool isStatue = false;
	
	public float startingHealth = 100.0f;		// The amount of health to start with
	public float maxHealth = 100.0f;			// The maximum amount of health
	private float currentHealth;				// The current ammount of health

	public bool replaceWhenDead = false;		// Whether or not a dead replacement should be instantiated.  (Useful for breaking/shattering/exploding effects)
	public GameObject deadReplacement;			// The prefab to instantiate when this GameObject dies

	public GameObject deathCam;                 // The camera to activate when the player dies
	[SerializeField] private string gameOverScene;
	[SerializeField] private HealthBar healthBar;
	[SerializeField] private GameObject gameOverMenu;
	[SerializeField] private GameObject playerObjectToDestroy;
	[SerializeField] private HurtIndicator hurtIndicator;


	private bool dead = false;					// Used to make sure the Die() function isn't called twice

	// Use this for initialization
	void Start()
	{
		// Initialize the currentHealth variable to the value specified by the user in startingHealth
		currentHealth = startingHealth;

		//healthBar.SetMaxHealth(startingHealth);
	}

	public void ChangeHealth(float amount, Vector3 position)
    {
		// Change the health by the amount specified in the amount variable
		currentHealth += amount;

		healthBar.SetHealth(currentHealth);

		//Hurt
		if (amount < 0 && !isStatue)
			hurtIndicator.Hurt(transform.position, position);

		// If the health runs out, then Die.
		if (currentHealth <= 0 && !dead && canDie)
		{
			dead = true;
			if (isStatue)
				DieStatue();
			else
				Die();
		}
		// Make sure that the health never exceeds the maximum health
		else if (currentHealth > maxHealth)
			currentHealth = maxHealth;
	}

	public void ChangeHealth(float amount)
	{
		// Change the health by the amount specified in the amount variable
		currentHealth += amount;

		healthBar.SetHealth(currentHealth);

		//Hurt
		print("Hurt!");
		if (amount < 0 && !isStatue)
			hurtIndicator.Hurt();

		// If the health runs out, then Die.
		if (currentHealth <= 0 && !dead && canDie)
        {
			dead = true;
			if (isStatue)
				DieStatue();
			else
				Die();
        }
		// Make sure that the health never exceeds the maximum health
		else if (currentHealth > maxHealth)
			currentHealth = maxHealth;
	}

	public void Die()
	{
		// Make death effects
		if (replaceWhenDead)
			Instantiate(deadReplacement, transform.position, transform.rotation);

		GameOver();
	}

	private void DieStatue()
    {
		GameOver();
	}

	private void GameOver()
    {
		deathCam.transform.position = new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z);
		deathCam.SetActive(true);
		gameOverMenu.SetActive(true);
		Destroy(playerObjectToDestroy);
	}
}
