using UnityEngine;

public class TestNewScore : MonoBehaviour
{
    public IncrementScore scoreManager; //ano ddrag ko dun naur wer u

    void Update()
    {
        // Example: Add a random score when the space key is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            scoreManager.AddScore(Random.Range(10, 100));
        }
    }
}