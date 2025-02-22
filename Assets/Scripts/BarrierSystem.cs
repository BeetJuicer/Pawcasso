using UnityEngine;

public class BarrierSystem : MonoBehaviour
{
    public GameObject barrier;
    private int enemiesKilled = 0;
    private int totalEnemiesToKill = 0;

    void Start()
    {
        // Instantiate the barrier at the start if not already set
        if (barrier == null)
        {
            Debug.LogError("Barrier GameObject is not assigned to BarrierSystem!");
            return;
        }
        barrier.SetActive(true); // Ensure the barrier is active at the start
    }

    public void UpdateTotalEnemiesToKill(int count)
    {
        totalEnemiesToKill += count;
    }

    public void OnEnemyKilled()
    {
        enemiesKilled++;

        if (enemiesKilled >= totalEnemiesToKill)
        {
            DestroyBarrier();
        }
    }

    private void DestroyBarrier()
    {
        if (barrier != null)
        {
            Destroy(barrier);
        }
    }
}
