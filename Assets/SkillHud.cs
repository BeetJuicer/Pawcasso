using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillHud : MonoBehaviour
{
    [SerializeField] private Slider colorOneSlider;
    [SerializeField] private Slider colorTwoSlider;

    // Update is called once per frame
    void Update()
    {
        colorOneSlider.maxValue = 0;
    }
}
