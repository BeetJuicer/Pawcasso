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
}

public class EnemySpawner : MonoBehaviour
{
    public EnemyWave[] waveConfigurations;  // Array of wave configurations
    private int currentWave = 0;
    private bool isTriggered = false;
    private BarrierSystem gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<BarrierSystem>();
    }

    void Update()
    {
        // Check if spawner is triggered, there are more waves to spawn, and no enemies are currently alive
        if (isTriggered && currentWave < waveConfigurations.Length && GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            // Spawn enemies for the current wave
            SpawnEnemies(waveConfigurations[currentWave]);
            currentWave++;
        }
    }

    void SpawnEnemies(EnemyWave wave)
    {
        int totalEnemies = 0;
        foreach (var enemyType in wave.enemyTypes)
        {
            for (int j = 0; j < enemyType.count; j++)
            {
                int spawnIndex = j % enemyType.spawnPoints.Length;
                GameObject enemy = Instantiate(enemyType.enemyPrefab, enemyType.spawnPoints[spawnIndex].position, enemyType.spawnPoints[spawnIndex].rotation);
                // Set the enemy's barrier system to be this gameobject.
                enemy.GetComponent<DemoEnemyControls>().SetBarrierSystem(gameManager);
            }
            totalEnemies += enemyType.count;
        }
        gameManager.UpdateTotalEnemiesToKill(totalEnemies);
    }

    public void TriggerSpawner()
    {
        if (!isTriggered)
        {
            isTriggered = true;
            currentWave = 0; // Reset currentWave index
            if (waveConfigurations.Length > 0)
            {
                SpawnEnemies(waveConfigurations[currentWave]);
                currentWave++;
            }
        }
    }
}
