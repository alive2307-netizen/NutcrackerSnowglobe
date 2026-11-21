using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Показывает нужный экран по состоянию GameManager: меню, HUD раунда или результат.
/// Кнопки подключаются в Awake, ссылки на элементы выставляет Stage5SceneTool.
/// </summary>
public class RoundUI : MonoBehaviour
{
    public GameManager game;
    public HealthManager health;

    public GameObject menuPanel;
    public GameObject hudPanel;
    public GameObject resultPanel;

    public Button playButton;
    public Button restartButton;

    public Text resultTitle;
    public Text resultReason;
    public Text timerText;
    public Text healthText;
    public Text scoreText;

    private void Awake()
    {
        playButton.onClick.AddListener(game.StartRound);
        restartButton.onClick.AddListener(game.Restart);
    }

    private void Update()
    {
        GameState state = game.State;
        bool finished = state == GameState.Win || state == GameState.Lose;

        menuPanel.SetActive(state == GameState.Menu);
        hudPanel.SetActive(state == GameState.Running);
        resultPanel.SetActive(finished);

        if (finished)
        {
            resultTitle.text = state == GameState.Win ? "ПОБЕДА!" : "ПОРАЖЕНИЕ";
            resultReason.text = game.ResultReason;
        }

        timerText.text = "Время: " + Mathf.CeilToInt(game.TimeLeft) + " с";
        healthText.text = "Ёлка: " + Mathf.CeilToInt(health.CurrentHealth) + " / " + Mathf.CeilToInt(health.maxHealth);

        if (scoreText != null)
        {
            scoreText.text = "Очки: " + game.Score + (game.IsScoreMultiplied ? " (x2)" : "");
        }
    }
}
