using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public static int playerScore { get; private set; }


    private void Awake()
    {
        instance = this;
    }

    public static void AddToPlayerScore(int addScore)
    {
        // apply multipliers here
        playerScore += addScore;
        print("New Score: " + playerScore);
    }


}
