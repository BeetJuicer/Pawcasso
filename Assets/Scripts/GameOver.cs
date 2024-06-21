using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class GameOver : MonoBehaviour
{
    [SerializeField] private string RestartScene;
    [SerializeField] private string MainMenuScene;

    public void Restart()
    {
        SceneManager.LoadScene(RestartScene);
        print("??????");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(MainMenuScene);
        print("uhuhuhuh");
    }

}
