using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorial : MonoBehaviour
{
    [SerializeField] private GameObject[] windows;
    [SerializeField] private GameObject backBtn;
    [SerializeField] private GameObject nextBtn;
    private int activeWindow;
    private void OnEnable()
    {
        backBtn.SetActive(false);
        if (windows.Length >= 1)
        {
            activeWindow = 0;
            nextBtn.SetActive(true);
        }
        else
            print("No panels");
    }

    public void NextWindow()
    {
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
}
