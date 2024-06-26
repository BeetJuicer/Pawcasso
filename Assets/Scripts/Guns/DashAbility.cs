using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;
public class DashAbility : Skill
{
    [Header("Dashing")]
    [SerializeField] private GameObject dashCollider;
    [SerializeField] private float damage;
    [SerializeField] private GameObject dashVisuals;
    [SerializeField] private GameObject dashParticles;
    private bool isDashing = false;
    private float timeLeft;

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        //if we have enough in the gauge and the user presses a key
        if (Input.GetKeyDown(key) &&
            requiredColors[primaryOne] <= ws.gauges[primaryOne] &&
            requiredColors[primaryTwo] <=    ws.gauges[primaryTwo])
        {
            EnterDash();

            ws.SubtractFromGauge(primaryOne, requiredColors[primaryOne]);
            ws.SubtractFromGauge(primaryTwo, requiredColors[primaryTwo]);
        }

        // timer for dash
        if(isDashing)
        {
            timeLeft -= Time.deltaTime;
            if(timeLeft <= 0f)
            {
                ExitDash();
            }
        }
    }

    private void EnterDash()
    {
        timeLeft = GetComponent<ExampleCharacterController>().EnterChargeState(1);
        dashCollider.SetActive(true);

        dashVisuals.SetActive(true);
        dashParticles.GetComponent<DashParticles>().PlayDash(1);

        dashCollider.GetComponent<DamageEnemiesCollider>().SetDamage(damage);
        isDashing = true;
    }
    private void ExitDash()
    {
        dashCollider.SetActive(false);
        dashParticles.GetComponent<DashParticles>().PlayDash(1);
        dashVisuals.SetActive(false);
        isDashing = false;
    }
}
