using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    public static int playerScore { get; private set; }


    private void Awake()
    {
        Instance = this;
    }

    public void AddToPlayerScore(int addScore)
    {
        // apply multipliers here
        playerScore += addScore;
        print("New Score: " + playerScore);
    }


}
