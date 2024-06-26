using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillHud : MonoBehaviour
{
    [SerializeField] private Skill skill;
    [SerializeField] private WeaponSystem weaponSystem;

    [SerializeField] private Slider colorOneSlider;
    [SerializeField] private Slider colorTwoSlider;

    private void Start()
    {
        colorOneSlider.maxValue = skill.requiredColors[skill.primaryOne];
        colorTwoSlider.maxValue = skill.requiredColors[skill.primaryTwo];
    }

    // Update is called once per frame
    void Update()
    {
        colorOneSlider.value = weaponSystem.gauges[skill.primaryOne];
        colorTwoSlider.value = weaponSystem.gauges[skill.primaryTwo];

        print(skill.gameObject.name + ": " + weaponSystem.gauges[skill.primaryOne] + " / " + skill.requiredColors[skill.primaryOne] + ", "
                                           + weaponSystem.gauges[skill.primaryTwo] + " / " + skill.requiredColors[skill.primaryTwo]);
    }
}
