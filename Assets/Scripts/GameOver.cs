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
    [Space(2)]
    [SerializeField] private GameObject HUD;

    private void Awake()
    {
        HUD.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Restart()
    {
        print("restart clicked");
        SceneManager.LoadScene(RestartScene);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(MainMenuScene);
    }

}
