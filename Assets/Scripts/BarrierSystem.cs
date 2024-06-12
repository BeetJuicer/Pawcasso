using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BarrierSystem : MonoBehaviour
{
    public GameObject[] Enemies;
    public GameObject barrier;

    // Start is called before the first frame update
    void Update()
    {
        if (AllEnemiesCleared())
        {
            Destroy(barrier);
        }
    }

    bool AllEnemiesCleared()
    {
        foreach (GameObject enemy in Enemies)
        {
            if (enemy != null)
            {
                return false;
            }
        }
        return true;
    }
}
