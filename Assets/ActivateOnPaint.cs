using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnPaint : MonoBehaviour
{
    ParticleSystem pSystem;
    // Start is called before the first frame update
    void Start()
    {
        pSystem = GetComponent<ParticleSystem>();
        pSystem.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        if (PaintSurfaceChecker.IsOnColoredGround)
        {
            pSystem.Play();
        }
        else
        {
            pSystem.Pause();
            pSystem.Clear();
        }
    }
}
