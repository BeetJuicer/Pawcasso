using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DelayedBeaconTrigger : MonoBehaviour
{
    [SerializeField] string tagFilter;
    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;
    [SerializeField] float triggerExecutionDelay; // Delay before executing the delayed OnTriggerEnter function
    [SerializeField] Slider delaySlider; // Reference to the UI Slider

    private void Start()
    {
        // Ensure the slider's initial value reflects the triggerExecutionDelay
        if (delaySlider != null)
        {
            delaySlider.value = triggerExecutionDelay;
            delaySlider.onValueChanged.AddListener(UpdateTriggerExecutionDelay);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter)) return;
        StartCoroutine(ExecuteTriggerAfterDelay());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!string.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter)) return;
        onTriggerExit.Invoke();
    }

    private IEnumerator ExecuteTriggerAfterDelay()
    {
        yield return new WaitForSeconds(triggerExecutionDelay);
        onTriggerEnter.Invoke();
    }

    // Method to update triggerExecutionDelay from the slider
    public void UpdateTriggerExecutionDelay(float delay)
    {
        triggerExecutionDelay = delay;
    }

    // Method to update the slider based on triggerExecutionDelay
    public void UpdateSlider()
    {
        if (delaySlider != null)
        {
            delaySlider.value = triggerExecutionDelay;
        }
    }
}
