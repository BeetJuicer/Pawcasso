using UnityEngine;
using System.Collections;
using BreadcrumbAi;
using System.Collections.Generic;

[System.Serializable]
public class DemoEnemySounds{
	public AudioClip audio_hit_1, audio_hit_2, audio_dead_1, audio_dead_2, audio_melee_attack_1, audio_melee_attack_2;
}

public class DemoEnemyControls : MonoBehaviour {

	[Space]
	public DemoEnemySounds audioClips;

	public int enemyPointWorth;
	public enum EnemyType {Melee, Ranged, Special};
	//public GameObject healthPickUpPrefab;
	//public bool _canDropPickUp;
	public EnemyType enemyType;
	public Rigidbody rangedProjectilePrefab;

	[Header("Melee Stats")]
	[SerializeField] private float meleeDamage;

	[Header("Shield")]
	[SerializeField] private GunColor shieldColor;
	[SerializeField] private bool isShieldActive;
	[Range(0, 1)]
	[SerializeField] private float shieldDamageReductionMultiplier;
	[SerializeField] private int shieldPointWorth;
	private struct SecondaryColorCombo
    {
		public GunColor first;
		public GunColor second;
    }
	private SecondaryColorCombo lastTwoHits;
	private SecondaryColorCombo neededHits;

	private bool isPrimary = true;

	[Header("Effects")]
	public GameObject bloodPrefab;
	public GameObject specialPrefab;
	public GameObject explosionPrefab;

	[Header("Healing")]
	[SerializeField] private LayerMask whatIsPlayer;
	[SerializeField] private float healRadius;

	private Transform player;
	private Ai ai;
	
	private bool _removeBody, _isHit, _animAttack;
	private AudioSource audioSource;
	
	private float rangedAttackNext = 0.0f;
	private float rangedAttackRate = 2.0f;
	private float meleeAttackNext = 0.0f;
	private float meleeAttackRate = 1.0f;
	
	private Animator anim;
	private string animRun = "Run";
	private string animDeath1 = "Death1";
	private string animAttack = "Attack";
	
	private bool _pointScored;

	private float difficultyMultiplier;

    private void Awake()
    {
		// 0 = easy, 1 = hard
		difficultyMultiplier = PlayerPrefs.GetInt("Difficulty") == 1 ? 1.5f : 1f;

		// Setup for shield and combo.
		switch(shieldColor)
        {
			case GunColor.Green:
				neededHits.first = GunColor.Yellow;
				neededHits.second = GunColor.Blue;
				isPrimary = false;
				break;
			case GunColor.Orange:
				neededHits.first = GunColor.Yellow;
				neededHits.second = GunColor.Red;
				isPrimary = false;
				break;
			case GunColor.Violet:
				neededHits.first = GunColor.Red;
				neededHits.second = GunColor.Blue;
				isPrimary = false;
				break;
        }
		//combo
		lastTwoHits.first = GunColor.None;
		lastTwoHits.second = GunColor.None;
    }

    void Start(){
		ai = GetComponent<Ai>();
		anim = GetComponent<Animator>();
		audioSource = gameObject.AddComponent<AudioSource>();
		//score = Camera.main.GetComponent<DemoScore>();
		GameObject go = GameObject.FindGameObjectWithTag("Player");
		if(go){
			player = go.transform;
		}

		//apply multiplier to health
		ai.Health *= difficultyMultiplier;
	}
	
	void Update () {
		CheckHealth();
		//CheckDeathZone();
	}
	
	void FixedUpdate(){
		Animation();
		Attack();
	}
	
	private void CheckDeathZone(){
		if(transform.position.y < -10 || transform.position.y > 10){
			UpdateEnemyCount();
		}
	}
	
	private void Animation(){
		if(ai.lifeState == Ai.LIFE_STATE.IsAlive){
			if(ai.moveState != Ai.MOVEMENT_STATE.IsIdle){
				anim.SetBool(animRun, true);
			} else {
				anim.SetBool(animRun, false);
			}
			if(_animAttack){
				anim.SetBool(animAttack, true);
			} else {
				anim.SetBool(animAttack, false);
			}
        } else if(ai.lifeState == Ai.LIFE_STATE.IsDead){
            anim.SetBool(animDeath1, true);
        }
    }
    
    private void Attack(){
    	if(player){
	    	if(ai.lifeState == Ai.LIFE_STATE.IsAlive){
		    	if(enemyType != EnemyType.Ranged){
					if(ai.attackState == Ai.ATTACK_STATE.CanAttackPlayer && Time.time > meleeAttackNext){
						meleeAttackNext = Time.time + meleeAttackRate;
						float rand = Random.value;
						if(rand <= 0.4f){
							audioSource.clip = audioClips.audio_melee_attack_1;
						} else {
							audioSource.clip = audioClips.audio_melee_attack_2;
						}
						audioSource.PlayOneShot(audioSource.clip);
						player.GetComponentInChildren<Health>().ChangeHealth(-meleeDamage);
						//player.GetComponent<DemoPlayerControls>()._isHit = true;
						//player.GetComponent<DemoPlayerControls>().Bleed(transform.rotation);
						_animAttack = true;
					} else {
						_animAttack = false;
					}
		    	} else {
					if(ai.attackState == Ai.ATTACK_STATE.CanAttackPlayer && Time.time > rangedAttackNext){
						rangedAttackNext = Time.time + rangedAttackRate;
						Rigidbody spit = Instantiate(rangedProjectilePrefab, transform.position + transform.forward + transform.up, transform.rotation) as Rigidbody;
						spit.AddForce(transform.forward * 500);
						_animAttack = true;
					} else {
						_animAttack = false;
					}
		    	}
	    	}
    	}
    }
    
    private void CheckHealth(){
        if(_isHit && this != null){
			float rand = Random.value;
			if(ai.Health > 0){
				if(rand > 0.5f){
					if(rand < 0.7f){
						audioSource.clip = audioClips.audio_hit_2;
					} else {
						audioSource.clip = audioClips.audio_hit_1;
					}
					audioSource.PlayOneShot(audioSource.clip);
				}
			}
			if(ai.Health <= 0){
				if(rand > 0.5f){
					audioSource.clip = audioClips.audio_dead_1;
				} else {
					audioSource.clip = audioClips.audio_dead_2;
				}
				audioSource.PlayOneShot(audioSource.clip);
			}
	        _isHit = false;
        }
        
		if(ai.lifeState == Ai.LIFE_STATE.IsDead){
			if(!_pointScored){
				if(enemyType == EnemyType.Special){
					ScoreManager.Instance.AddToPlayerScore(enemyPointWorth * 2);
				}
				else {
					ScoreManager.Instance.AddToPlayerScore(enemyPointWorth);
				}
				_pointScored = true;
			}
			//if(_canDropPickUp){
			//	float rand = Random.value;
			//	if(rand <= 0.3f){
			//		GameObject healthPickUp = Instantiate(healthPickUpPrefab,transform.position,Quaternion.identity) as GameObject;
			//		healthPickUp.transform.position = new Vector3(transform.position.x, 1, transform.position.z);
			//		Destroy(healthPickUp, 20);
			//	}
			//	_canDropPickUp = false;
			//}

			//TODO: Spawn a death paint particle that heals the player on collision with the player, limited to 1 heal per 0.2f or something.
			Instantiate(explosionPrefab, transform.position, Quaternion.identity);
			UpdateEnemyCount();
			Destroy(gameObject);
	    }
    }
		
	
	void UpdateEnemyCount(){
		if(enemyType == EnemyType.Special){
			GameObject.Find("Spawners").GetComponent<DemoSpawnerControl>().specialEnemyCount--;
		}
		//TODO: update enemy count in room.
		//GameObject.Find("Spawners").GetComponent<DemoSpawnerControl>().enemyCount--;
		Destroy(gameObject);
	}
	
	public void TakeDamage(float damage, Vector3 hitSpawnPoint, Quaternion rotation, GunColor color)
    {
		// Checking for shield
		if(isShieldActive)
        {
			CheckShield(color);
        }
		_isHit = true;
		print("Last two hits: " + lastTwoHits.first + lastTwoHits.second);

		ai.Health -= damage * shieldDamageReductionMultiplier;
		GameObject blood = Instantiate(bloodPrefab, hitSpawnPoint, rotation) as GameObject;
		Destroy(blood, 3);
	}

	private void CheckShield(GunColor color)
    {
		//primary color
		if (isPrimary && color == shieldColor)
		{
			PopShield();
		}
		//secondary color
		else if (!isPrimary)
		{
			//First is current hit. Second is the hit before this. 
			lastTwoHits.second = GunColor.None;

			//both empty
			if (lastTwoHits.first == GunColor.None && lastTwoHits.second == GunColor.None)
				lastTwoHits.first = color;
			// first is not empty
			else if (lastTwoHits.first != GunColor.None)
			{
				// like a queue, push back 'first' to the second spot and move the current color to the 'first' spot
				lastTwoHits.second = lastTwoHits.first;
				lastTwoHits.first = color;
			}

			if ((lastTwoHits.first == neededHits.first && lastTwoHits.second == neededHits.second) ||
			   (lastTwoHits.second == neededHits.first && lastTwoHits.first == neededHits.second))
			{
				PopShield();
			}
		}
	}

	private void PopShield()
    {
		print("shield popped!");
		isShieldActive = false;
		shieldDamageReductionMultiplier = 1;
		ScoreManager.Instance.AddToPlayerScore(shieldPointWorth);
		//play a pop animation here for the indicator and shit.
	}
}
