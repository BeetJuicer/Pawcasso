using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class IncrementScore : MonoBehaviour
{      
    public TextMeshProUGUI totalScoreText;
    public GameObject scorePanel;
    public GameObject scoreTextPrefab;
    public int maxScoreTexts = 5;
    public float fadeDuration = 2.0f;

    private int totalScore = 0;
    private Queue<GameObject> scoreTexts = new Queue<GameObject>();

    void Start()
    {
        UpdateTotalScore();
    }

    public void AddScore(int score)
    {
        totalScore += score;
        UpdateTotalScore();

        GameObject newScoreText = Instantiate(scoreTextPrefab, scorePanel.transform);
        newScoreText.GetComponent<Text>().text = "+" + score.ToString();
        scoreTexts.Enqueue(newScoreText);

        if (scoreTexts.Count > maxScoreTexts)
        {
            GameObject oldScoreText = scoreTexts.Dequeue();
            StartCoroutine(FadeOutAndDestroy(oldScoreText));
        }
    }

    void UpdateTotalScore()
    {
        totalScoreText.text = "Total Score: " + totalScore.ToString();
    }

    IEnumerator FadeOutAndDestroy(GameObject scoreText)
    {
        print("fade is called");

        Text textComponent = scoreText.GetComponent<Text>();
        Color originalColor = textComponent.color;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            textComponent.color = Color.Lerp(originalColor, Color.clear, t / fadeDuration);
            yield return null;

        }
        Destroy(scoreText);
    }
}
