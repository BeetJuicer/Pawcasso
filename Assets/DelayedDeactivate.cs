using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDeactivate : MonoBehaviour
{
    [SerializeField] private float delay;
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(Deactivate), delay);
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
