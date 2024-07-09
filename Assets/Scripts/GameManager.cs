using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;
using SmartPoint;
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public bool IsPaused { get; private set; } = false;

    [SerializeField] private ExampleCharacterController playerCharacter;
    [SerializeField] private CheckPointController cp1;
    [SerializeField] private CheckPointController cpGino;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        int checkpointIndex = PlayerPrefs.GetInt("PlayerCheckpoint");
        CheckPointController cp = cp1;

        if(checkpointIndex > cp1.checkPoints.Count)
        {
            cp = cpGino;
            //past the tutorial.
            //activate gino world, add all guns and skills.
        }

        for(int i = 0; i < checkpointIndex; i++)
            cp.SetCheckpointState(i, true);

        cp.TeleportToCheckpoint(checkpointIndex, playerCharacter.gameObject);
    }

    public static GameManager Instance()
    {
        return instance;
    }

    public void PauseActions()
    {
        Time.timeScale = 0;
        IsPaused = true;
    }

    public void ResumeActions()
    {
        Time.timeScale = 1;
        IsPaused = false;
    }


}
