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

    [SerializedDictionary("Color", "Amount")]
    public SerializedDictionary<GunColor, int> requiredColors;
    private GunColor primaryOne;
    private GunColor primaryTwo;

    // Update is called once per frame
    void Update()
    {
        var e = requiredColors.GetEnumerator();
        e.MoveNext();
        primaryOne = e.Current.Key;
        e.MoveNext();
        primaryTwo = e.Current.Key;

        //if we have enough in the gauge and the user presses a key
        if (Input.GetKeyDown(key) &&
            requiredColors[primaryOne] < ws.gauges[primaryOne] &&
            requiredColors[primaryTwo] < ws.gauges[primaryTwo])
        {
            GetComponent<ExampleCharacterController>().EnterChargeState(1);
            ws.SubtractFromGauge(primaryOne, requiredColors[primaryOne]);
            ws.SubtractFromGauge(primaryTwo, requiredColors[primaryTwo]);
        }

        //print(primaryOne + ": " + 
        //	ws.gauges[primaryOne] + 
        //	" / " + requiredColors[primaryOne] + ". " + primaryTwo + ": " + ws.gauges[primaryTwo] + " / " + requiredColors[primaryTwo]);
    }
}
