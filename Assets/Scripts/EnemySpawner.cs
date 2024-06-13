using System.Collections;
using UnityEngine;

[System.Serializable]
public class EnemyType
{
    public GameObject enemyPrefab;
    public int count;
}

[System.Serializable]
public class EnemyWave
{
    public EnemyType[] enemyTypes;
}

public class EnemySpawner : MonoBehaviour
{
    public EnemyWave[] waveConfigurations;  // Array of wave configurations
    public Transform[] spawnPoints;
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
                int spawnIndex = Random.Range(0, spawnPoints.Length);
                Instantiate(enemyType.enemyPrefab, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
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
