using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillHud : MonoBehaviour
{
    [SerializeField] private Skill skill;
    [SerializeField] private WeaponSystem weaponSystem;

    [Space(2)]
    [SerializeField] private Slider colorOneSlider;
    [SerializeField] private Slider colorTwoSlider;
    [SerializeField] private TextMeshProUGUI amount;

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
        int primaryOneCurrent = weaponSystem.gauges[skill.PrimaryOne];
        int primaryTwoCurrent = weaponSystem.gauges[skill.PrimaryTwo];
        int primaryOneRequired = skill.requiredColors[skill.PrimaryOne];
        int primaryTwoRequired = skill.requiredColors[skill.PrimaryTwo];

        if (primaryOneCurrent < primaryOneRequired ||
            primaryTwoCurrent < primaryTwoRequired)
        {
            DisableContainer();
        }
        else
        {
            EnableContainer();
        }

        colorOneSlider.value = primaryOneCurrent;
        colorTwoSlider.value = primaryTwoCurrent;

        int uses = Mathf.Min(Mathf.FloorToInt(primaryOneCurrent / primaryOneRequired), 
                             Mathf.FloorToInt(primaryTwoCurrent / primaryTwoRequired));
        amount.text = uses.ToString();

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
