using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevel : MonoBehaviour
{
    [SerializeField] private GameObject[] levelsToLoad;
    [SerializeField] private GameObject[] levelsToDeload;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (levelsToLoad.Length > 0)
                foreach (GameObject level in levelsToLoad)
                    level.SetActive(true);

            if (levelsToDeload.Length > 0)
                foreach (GameObject level in levelsToDeload)
                    level.SetActive(false);
        }
    }
}
