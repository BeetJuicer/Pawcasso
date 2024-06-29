using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;
using KinematicCharacterController;

public class MoveLevel : MonoBehaviour
{
    [SerializeField] private Transform teleportTo;
    [SerializeField] private GameObject levelToDisable;
    [SerializeField] private GameObject levelTOEnable;


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (levelTOEnable != null)
                levelTOEnable.SetActive(true);
            else
                Debug.LogWarning("No level to enable. Are you sure?");

            other.GetComponent<KinematicCharacterMotor>().SetPosition(teleportTo.position);

            if (levelToDisable != null)
                levelToDisable.SetActive(false);
            else
                Debug.LogWarning("No level disabled. Are you sure?");
        }
    }
}
