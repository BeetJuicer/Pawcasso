using System.Collections;
using UnityEngine;

[System.Serializable]
public class EnemyType
{
    public GameObject enemyPrefab;
    public int count;
    public Transform[] spawnPoints;  // Spawn points for this enemy type
}

[System.Serializable]
public class EnemyWave
{
    public EnemyType[] enemyTypes;
    [HideInInspector] public int enemyCountInWave;
}

public class EnemySpawner : MonoBehaviour
{
    public EnemyWave[] waveConfigurations;  // Array of wave configurations
    private int currentWave = 0;
    private bool isTriggered = false;

    [Tooltip("An array of barriers to enable once the player enters.")]
    [SerializeField] private GameObject[] barriersToEnableOnEntry;

    [Tooltip("An array of barriers to disable once all waves are finished.")]
    [SerializeField] private GameObject[] barriersToDisableOnExit;

    private int enemiesKilledThisWave;
    public void TriggerSpawner()
    {
        if (!isTriggered)
        {
            isTriggered = true;
            EnableBarriers();
            currentWave = 0; // Reset currentWave index
            if (currentWave < waveConfigurations.Length)
            {
                SpawnEnemies(waveConfigurations[currentWave]);
            }
        }
    }

    void SpawnEnemies(EnemyWave wave)
    {
        foreach (var enemyType in wave.enemyTypes)
        {
            for (int j = 0; j < enemyType.count; j++)
            {
                int spawnIndex = j % enemyType.spawnPoints.Length;
                GameObject enemy = Instantiate(enemyType.enemyPrefab, enemyType.spawnPoints[spawnIndex].position, enemyType.spawnPoints[spawnIndex].rotation);

                // Set the enemy's barrier system to be this gameobject.
                enemy.GetComponent<DemoEnemyControls>().SetSpawner(this);
            }
            wave.enemyCountInWave += enemyType.count;
        }
    }

    public void OnEnemyKilled()
    {
        enemiesKilledThisWave++;

        if (enemiesKilledThisWave >= waveConfigurations[currentWave].enemyCountInWave)
        {
            //reset enemy counter
            enemiesKilledThisWave = 0;
            FinishWave();
        }
    }

    private void FinishWave()
    {
        currentWave++;
        if (currentWave >= waveConfigurations.Length)
        {
            // all waves done.
            DisableBarriers();
        }
        else
        {
            SpawnEnemies(waveConfigurations[currentWave]);
        }
    }

    private void EnableBarriers()
    {
        foreach (GameObject barrier in barriersToEnableOnEntry)
        {
            barrier.SetActive(true);
        }
    }

    private void DisableBarriers()
    {
        foreach (GameObject barrier in barriersToDisableOnExit)
        {
            barrier.SetActive(false);
        }
    }
}
