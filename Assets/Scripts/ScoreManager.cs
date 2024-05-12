using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    public static int playerScore { get; private set; }
    public static int comboMultiplier { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        
    }

    public void AddToPlayerScore(int addScore)
    {
        // apply multipliers here
        playerScore += addScore;
        print("New Score: " + playerScore);
    }

    public void AddToComboMultiplier(float addMultiplier)
    {

    }
}
