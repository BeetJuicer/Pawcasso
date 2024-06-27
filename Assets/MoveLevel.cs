using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;
using KinematicCharacterController;

public class MoveLevel : MonoBehaviour
{
    [SerializeField] private Transform teleportTo;
    [SerializeField] private GameObject levelToDisable;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<KinematicCharacterMotor>().SetPosition(teleportTo.position);
            levelToDisable.SetActive(false);
        }
    }
}
