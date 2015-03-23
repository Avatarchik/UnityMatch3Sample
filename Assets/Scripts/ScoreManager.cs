using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

    public Renderer backgroundRenderer;
    public Text scoreText;

    private int score = 0;
    private int displayedScore = 0;
    private Tweener scoreChangeTween;

    private void OnScoreIncreased(int count)
    {
        score += count;
        //backgroundRenderer.material.SetFloat("_BlurLevel", Mathf.Max(0.3f, 1f - score / 10000f));

        if (scoreChangeTween != null)
        {
            scoreChangeTween.Kill();
        }
        scoreChangeTween = DOTween.To(() => displayedScore, DisplayScore, score, 0.5f);
    }

    void DisplayScore(int newScore)
    {
        displayedScore = newScore;
        scoreText.text = displayedScore.ToString();
    }

    void OnEnable()
    {
        BoardRenderer.OnScoreIncreased += OnScoreIncreased;
    }

    void OnDisable()
    {
        BoardRenderer.OnScoreIncreased -= OnScoreIncreased;
    }
}
