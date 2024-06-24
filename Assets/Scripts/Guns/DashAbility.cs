using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;
using AYellowpaper.SerializedCollections;


public class DashAbility : MonoBehaviour
{
    [SerializeField] private WeaponSystem ws;

    [Header("Dashing")]
    [SerializeField] private KeyCode key;
    [SerializeField] private GameObject dashCollider;
    [SerializeField] private float damage;
    [SerializeField] private GameObject dashVisuals;
    [SerializeField] private GameObject dashParticles;
    private bool isDashing = false;
    private float timeLeft;

    [SerializedDictionary("Color", "Amount")]
    public SerializedDictionary<GunColor, int> requiredColors;
    private GunColor primaryOne;
    private GunColor primaryTwo;

    // Update is called once per frame
    void Update()
    {
        // grab the required colors
        var e = requiredColors.GetEnumerator();
        e.MoveNext();
        primaryOne = e.Current.Key;
        e.MoveNext();
        primaryTwo = e.Current.Key;

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
