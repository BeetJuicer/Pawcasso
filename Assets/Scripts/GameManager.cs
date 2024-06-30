using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public bool isPaused { get; private set; } = false;

    private void Awake()
    {
        instance = this;
    }

    public static GameManager Instance()
    {
        return instance;
    }

    public void PauseActions()
    {
        Time.timeScale = 0;
        isPaused = true;
    }

    public void ResumeActions()
    {
        Time.timeScale = 1;
        isPaused = false;
    }


}
