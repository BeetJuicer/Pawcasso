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

    [Space(2)]
    [SerializeField] private GameObject containerActive;
    [SerializeField] private GameObject containerDisabled;

    private void Start()
    {
        colorOneSlider.maxValue = weaponSystem.gaugesMax[skill.PrimaryOne];
        colorTwoSlider.maxValue = weaponSystem.gaugesMax[skill.PrimaryTwo];
    }

    // Update is called once per frame
    void Update()
    {
        if (weaponSystem.gauges[skill.PrimaryOne] < skill.requiredColors[skill.PrimaryOne] ||
            weaponSystem.gauges[skill.PrimaryTwo] < skill.requiredColors[skill.PrimaryTwo])
        {
            DisableContainer();
        }
        else
        {
            EnableContainer();
        }

        colorOneSlider.value = weaponSystem.gauges[skill.PrimaryOne];
        colorTwoSlider.value = weaponSystem.gauges[skill.PrimaryTwo];

        //if(max amount of skill stacks, value is full)

        //print(skill.gameObject.name + ": " + weaponSystem.gauges[skill.primaryOne] + " / " + skill.requiredColors[skill.primaryOne] + ", "
        //                                   + weaponSystem.gauges[skill.primaryTwo] + " / " + skill.requiredColors[skill.primaryTwo]);
    }

    private void DisableContainer()
    {
        containerActive.SetActive(false);
        containerDisabled.SetActive(true);
    }

    private void EnableContainer()
    {
        containerActive.SetActive(true);
        containerDisabled.SetActive(false);
    }
}
