using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndUI : MonoBehaviour
{
    public GameObject[] children;

    public void Activate()
    {
        foreach(GameObject child in children)
        {
            child.SetActive(true);
        }
    }

    public void Delayed()
    {
        Invoke(nameof(Activate), 1f);
    }
}
