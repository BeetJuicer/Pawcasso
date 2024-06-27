using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDeactivate : MonoBehaviour
{
    [SerializeField] private float delay;

    private void OnEnable()
    {
        Invoke(nameof(Deactivate), delay);

    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
