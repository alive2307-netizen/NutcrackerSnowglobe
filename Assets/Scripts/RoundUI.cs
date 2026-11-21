using UnityEngine;
using UnityEngine.UI;

public class RoundUI : MonoBehaviour
{
    public GameManager game;
    public HealthManager health;

    public GameObject menuPanel;
    public GameObject optionsPanel;
    public GameObject hudPanel;
    public GameObject resultPanel;

    public Button playButton;
    public Button optionsButton;
    public Button backButton;
    public Button restartButton;
    public Button menuButton;

    public Text resultTitle;
    public Text resultReason;
    public Text timerText;
    public Text healthText;
    public Text scoreText;
    public Text countdownText;

    private bool optionsOpen;

    private void Awake()
    {
        playButton.onClick.AddListener(game.StartRound);
        optionsButton.onClick.AddListener(() => optionsOpen = true);
        backButton.onClick.AddListener(() => optionsOpen = false);
        restartButton.onClick.AddListener(game.Restart);
        menuButton.onClick.AddListener(game.ReturnToMenu);
        timerText.gameObject.SetActive(false);
    }

    private void Update()
    {
        GameState state = game.State;
        bool menu = state == GameState.Menu;
        bool counting = state == GameState.Countdown;
        bool finished = state == GameState.Win || state == GameState.Lose;

        if (!menu)
        {
            optionsOpen = false;
        }

        menuPanel.SetActive(menu && !optionsOpen);
        optionsPanel.SetActive(menu && optionsOpen);
        hudPanel.SetActive(state == GameState.Running || counting || state == GameState.Defeat);
        resultPanel.SetActive(finished);

        countdownText.gameObject.SetActive(counting);

        if (counting)
        {
            countdownText.text = game.CountdownValue.ToString();
        }

        if (finished)
        {
            resultTitle.text = state == GameState.Win ? "ПОБЕДА!" : "ПОРАЖЕНИЕ";
            resultReason.text = game.ResultReason;
        }

        healthText.text = "Ёлка: " + Mathf.CeilToInt(health.CurrentHealth) + " / " + Mathf.CeilToInt(health.maxHealth);

        if (scoreText != null)
        {
            scoreText.text = "Очки: " + game.Score + (game.IsScoreMultiplied ? " (x2)" : "");
        }
    }
}
