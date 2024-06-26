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
    [SerializeField] private ExampleCharacterController characterController;
    private bool isDashing = false;
    private float timeLeft;

    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {
        //if we have enough in the gauge and the user presses a key
        if (Input.GetKeyDown(key) &&
            requiredColors[PrimaryOne] <= ws.gauges[PrimaryOne] &&
            requiredColors[PrimaryTwo] <=    ws.gauges[PrimaryTwo])
        {
            EnterDash();

            ws.SubtractFromGauge(PrimaryOne, requiredColors[PrimaryOne]);
            ws.SubtractFromGauge(PrimaryTwo, requiredColors[PrimaryTwo]);
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
        timeLeft = characterController.EnterChargeState(1);
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
