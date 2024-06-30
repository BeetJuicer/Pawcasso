using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorial : MonoBehaviour
{
    [SerializeField] private GameObject[] windows;
    [SerializeField] private GameObject backBtn;
    [SerializeField] private GameObject nextBtn;
    [SerializeField] private GameObject hud;
    private int activeWindow;
    private void OnEnable()
    {
        GameManager.Instance().PauseActions();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        hud.SetActive(false);

        backBtn.SetActive(false);
        if (windows.Length >= 1)
        {
            activeWindow = 0;
            nextBtn.SetActive(true);
        }
        else
        {
            print("No panels"); 
            return;
        }

        windows[0].SetActive(true);
    }

    public void NextWindow()
    {
        print("click");
        windows[activeWindow].SetActive(false);
        backBtn.SetActive(true);

        activeWindow++;
        windows[activeWindow].SetActive(true);

        if(activeWindow + 1 >= windows.Length)
        {
            nextBtn.SetActive(false);
        }
    }

    public void PreviousWindow()
    {
        windows[activeWindow].SetActive(false);
        nextBtn.SetActive(true);

        activeWindow--;
        windows[activeWindow].SetActive(true);

        if (activeWindow - 1 < 0)
        {
            backBtn.SetActive(false);
        }
    }

    public void Exit()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        hud.SetActive(true);
        print("tite");
        gameObject.SetActive(false);
        GameManager.Instance().ResumeActions();
    }
}
