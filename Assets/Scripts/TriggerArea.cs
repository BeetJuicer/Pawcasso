using UnityEngine;


public class TriggerArea : MonoBehaviour
{
    [SerializeField]
    private EnemySpawner spawner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (spawner != null)
            {
                spawner.TriggerSpawner();
            }
        }
    }
}