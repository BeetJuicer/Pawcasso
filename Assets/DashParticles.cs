using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem dashEffect;

    [SerializeField] private float lowDashTime;
    [SerializeField] private float mediumDashTime;
    [SerializeField] private float highDashTime;
    private float dashStartTime;
    private float dashTime;

    private void Start()
    {
        dashEffect.Stop();
    }

    public void PlayDash(int chargeLevel)
    {
        switch(chargeLevel)
        {
            case 1:
                dashTime = lowDashTime;
                break;
            case 2:
                dashTime = mediumDashTime;
                break;
            case 3:
                dashTime = highDashTime;
                break;
        }

        dashStartTime = Time.time;
        dashEffect.Play();
    }

    private void Update()
    {
        if(!dashEffect.isStopped)
        {
            if (Time.time > dashStartTime + dashTime)
            {
                dashEffect.Stop();
            }
        }
    }
}
