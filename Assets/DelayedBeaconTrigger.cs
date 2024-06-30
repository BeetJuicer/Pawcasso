using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DelayedBeaconTrigger : MonoBehaviour
{
    [SerializeField] private string tagFilter;
    [SerializeField] private UnityEvent onTriggerEnter;
    [SerializeField] private UnityEvent onTriggerExit;
    [SerializeField] private float triggerExecutionDelay; // Delay before executing the delayed OnTriggerEnter function
    [SerializeField] private Slider delaySlider; // Reference to the UI Slider
    private float currentDelay;

    private void Start()
    {
        // Ensure the slider's initial value reflects the triggerExecutionDelay
        if (delaySlider != null)
        {
            delaySlider.maxValue = triggerExecutionDelay; // Set max value to the delay
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
        currentDelay = triggerExecutionDelay;
        while (currentDelay > 0)
        {
            yield return null;
            currentDelay -= Time.deltaTime;
            if (delaySlider != null)
            {
                delaySlider.value = currentDelay;
            }
        }
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
