using UnityEngine;
using UnityEngine.UI;

public class RoundUI : MonoBehaviour
{
    private const float AdStubSeconds = 3f;

    public GameManager game;
    public HealthManager health;

    public GameObject menuPanel;
    public GameObject optionsPanel;
    public GameObject hudPanel;
    public GameObject resultPanel;
    public GameObject revivePanel;
    public GameObject adPanel;

    public Button playButton;
    public Button optionsButton;
    public Button backButton;
    public Button restartButton;
    public Button menuButton;
    public Button reviveCoinButton;
    public Button reviveAdButton;
    public Button giveUpButton;

    public Text resultTitle;
    public Text resultReason;
    public Text timerText;
    public Text healthText;
    public Text countdownText;
    public Text coinsText;
    public Text adText;

    private bool optionsOpen;
    private bool watchingAd;
    private float adEndsAt;

    private void Awake()
    {
        playButton.onClick.AddListener(game.StartRound);
        optionsButton.onClick.AddListener(() => optionsOpen = true);
        backButton.onClick.AddListener(() => optionsOpen = false);
        restartButton.onClick.AddListener(game.Restart);
        menuButton.onClick.AddListener(game.ReturnToMenu);
        reviveCoinButton.onClick.AddListener(() => game.TryReviveWithCoins());
        reviveAdButton.onClick.AddListener(StartAdStub);
        giveUpButton.onClick.AddListener(game.GiveUp);
        timerText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (watchingAd)
        {
            float left = adEndsAt - Time.unscaledTime;
            adText.text = "Реклама (заглушка)\n" + Mathf.CeilToInt(left);

            if (left <= 0f)
            {
                watchingAd = false;
                game.Revive();
            }
        }

        GameState state = game.State;
        bool menu = state == GameState.Menu;
        bool counting = state == GameState.Countdown;
        bool reviving = state == GameState.Revive;
        bool finished = state == GameState.Win || state == GameState.Lose;

        if (!menu)
        {
            optionsOpen = false;
        }

        menuPanel.SetActive(menu && !optionsOpen);
        optionsPanel.SetActive(menu && optionsOpen);
        hudPanel.SetActive(state == GameState.Running || counting || state == GameState.Defeat);
        resultPanel.SetActive(finished);
        revivePanel.SetActive(reviving && !watchingAd);
        adPanel.SetActive(reviving && watchingAd);
        countdownText.gameObject.SetActive(counting);
        coinsText.gameObject.SetActive(menu || reviving);

        if (counting)
        {
            countdownText.text = game.CountdownValue.ToString();
        }

        if (finished)
        {
            resultTitle.text = state == GameState.Win ? "ПОБЕДА!" : "ПОРАЖЕНИЕ";
            resultReason.text = game.ResultReason + (state == GameState.Win ? "\n+" + GameManager.WinReward + " монет" : "");
        }

        coinsText.text = "Монеты: " + CoinWallet.Coins;
        reviveCoinButton.interactable = CoinWallet.Coins >= GameManager.ReviveCost;
        healthText.text = "Ёлка: " + Mathf.CeilToInt(health.CurrentHealth) + " / " + Mathf.CeilToInt(health.maxHealth);
    }

    private void StartAdStub()
    {
        watchingAd = true;
        adEndsAt = Time.unscaledTime + AdStubSeconds;
    }
}
