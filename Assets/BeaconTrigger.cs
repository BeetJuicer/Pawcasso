using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeaconTrigger : MonoBehaviour
{
    [SerializeField] bool destroyOnTriggerEnter;
    [SerializeField] string tagFilter;
    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;
    [SerializeField] List<GameObject> gameObjectsToDestroy; // List of GameObjects to destroy immediately
    [SerializeField] List<GameObject> delayedGameObjectsToDestroy; // List of GameObjects to destroy with delay
    [SerializeField] float destructionDelay; // Single delay time for all delayed GameObjects

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter)) return;
        onTriggerEnter.Invoke();

        // Destroy immediate GameObjects
        if (gameObjectsToDestroy != null)
        {
            foreach (var obj in gameObjectsToDestroy)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }

        // Destroy delayed GameObjects
        if (delayedGameObjectsToDestroy != null)
        {
            foreach (var obj in delayedGameObjectsToDestroy)
            {
                if (obj != null)
                {
                    StartCoroutine(DestroyAfterDelay(obj, destructionDelay));
                }
            }
        }

        if (destroyOnTriggerEnter)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!string.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter)) return;
        onTriggerExit.Invoke();
    }

    private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }
}
