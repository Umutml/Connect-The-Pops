using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonGeneric<GameManager>
{
    private readonly int _targetFrameRate = 120;
    [SerializeField] private TextMeshProUGUI scoreText;
    private int _globalScore;
    private const int ScoreMultiplier = 10;

    public override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = _targetFrameRate;
        QualitySettings.vSyncCount = 0;
        SetScoreUI(0);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    private void SetScoreUI(int score)
    {
        scoreText.SetText(score.ToString());
    }

    public void AddScore(int score)
    {
        _globalScore += score;
        SetScoreUI(_globalScore * ScoreMultiplier);
    }
}
