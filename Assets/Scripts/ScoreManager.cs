using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    public static int PlayerScore { get; private set; }
    public static float PointComboMultiplier { get; private set; }

    private GunColor previousColor = GunColor.None;
    private const float COLOR_SWITCH_MULTIPLIER = 0.2f;
    [SerializeField] private float maxColorComboTimer = 2f;
    private float colorComboDuration;
    [SerializeField] private float maxPointComboMultiplier = 2f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        colorComboDuration = maxColorComboTimer;
    }

    private void Update()
    {
        if(colorComboDuration > 0)
        {
            colorComboDuration -= Time.deltaTime;
            if (colorComboDuration < 0)
            {
                //multiplier back to normal.
                PointComboMultiplier = 1;
            }
        }
    }

    public void AddToPlayerScore(int addScore)
    {
        // apply multipliers here
        PlayerScore += Mathf.FloorToInt(addScore * PointComboMultiplier);
        print("New Score: " + PlayerScore);
    }

    public void WishForCombo(GunColor color)
    {
        if(previousColor != color)
        {
            //reset combo timer
            colorComboDuration = maxColorComboTimer;
            AddToPointComboMultiplier(COLOR_SWITCH_MULTIPLIER);
        }

        previousColor = color;
    }

    private void AddToPointComboMultiplier(float addMultiplier)
    {
        float sum = PointComboMultiplier + addMultiplier;
        if (sum <= maxPointComboMultiplier)
            PointComboMultiplier += addMultiplier;
    }
}
