using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;

public class DashAbility : MonoBehaviour
{
    [Header("Dashing")]
    [SerializeField] private KeyCode key;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(key))
            GetComponent<ExampleCharacterController>().EnterChargeState(1);
    }
}
