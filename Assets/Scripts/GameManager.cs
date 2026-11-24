using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Countdown,
    Running,
    Defeat,
    Win,
    Lose,
    Revive
}

public class GameManager : MonoBehaviour
{
    private const float CountdownSeconds = 3f;
    private const float DefeatPauseSeconds = 3f;
    private const float ReviveGraceSeconds = 2f;

    public const int ReviveCost = 50;
    public const int WinReward = 10;

    public static GameManager Instance { get; private set; }

    private static bool startAfterReload;

    [Header("References")]
    public PlayerMovement player;

    public GameState State { get; private set; } = GameState.Menu;
    public string ResultReason { get; private set; } = "";

    public int Score { get; private set; }
    public bool IsScoreMultiplied => Time.time < scoreMultiplierUntil;
    public int CountdownValue => Mathf.Max(1, Mathf.CeilToInt(stateEndsAt - Time.time));
    public bool CanBeDefeated => State == GameState.Running && Time.time >= reviveGraceUntil;

    private float scoreMultiplier = 1f;
    private float scoreMultiplierUntil;
    private float stateEndsAt;
    private float reviveGraceUntil;
    private bool defeatByTheft;
    private bool phraseCollected;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        SetState(GameState.Menu);

        if (startAfterReload)
        {
            startAfterReload = false;
            StartRound();
        }
    }

    private void Update()
    {
        if (State == GameState.Countdown && Time.time >= stateEndsAt)
        {
            SetState(GameState.Running);
        }
        else if (State == GameState.Defeat && Time.time >= stateEndsAt)
        {
            SetState(GameState.Revive);
        }

        AudioManager.SetGnawing(State == GameState.Defeat && defeatByTheft);
    }

    public void StartRound()
    {
        if (State != GameState.Menu)
        {
            return;
        }

        ResultReason = "";
        Score = 0;
        scoreMultiplierUntil = 0f;
        defeatByTheft = false;
        stateEndsAt = Time.time + CountdownSeconds;
        SetState(GameState.Countdown);
    }

    public void GameOver(string reason)
    {
        if (!CanBeDefeated)
        {
            return;
        }

        ResultReason = reason;
        stateEndsAt = Time.time + DefeatPauseSeconds;
        SetState(GameState.Defeat);
    }

    public void GameOverByTheft(string reason)
    {
        if (!CanBeDefeated)
        {
            return;
        }

        defeatByTheft = true;
        GameOver(reason);
    }

    public void Win(string reason)
    {
        if (State != GameState.Running)
        {
            return;
        }

        ResultReason = reason;
        CoinWallet.Add(WinReward);
        SetState(GameState.Win);
        Debug.Log("[Щелкунчик] Раунд окончен: Win. " + reason);
    }

    public void MarkPhraseCollected()
    {
        phraseCollected = true;
        TryWin();
    }

    public void TryWin()
    {
        if (!phraseCollected || player == null)
        {
            return;
        }

        HealthManager health = player.GetComponent<HealthManager>();

        if (health != null && health.CurrentHealth >= health.maxHealth)
        {
            Win("Фраза собрана и ёлка украшена");
        }
    }

    public bool TryReviveWithCoins()
    {
        if (State != GameState.Revive || !CoinWallet.TrySpend(ReviveCost))
        {
            return false;
        }

        Revive();
        return true;
    }

    public void Revive()
    {
        if (State != GameState.Revive)
        {
            return;
        }

        HealthManager health = player != null ? player.GetComponent<HealthManager>() : null;

        if (defeatByTheft && health != null)
        {
            health.RestoreAfterTheft();
        }

        defeatByTheft = false;
        reviveGraceUntil = Time.time + ReviveGraceSeconds;
        SetState(GameState.Running);
    }

    public void GiveUp()
    {
        if (State != GameState.Revive)
        {
            return;
        }

        SetState(GameState.Lose);
        Debug.Log("[Щелкунчик] Раунд окончен: Lose. " + ResultReason);
    }

    public void AddScore(int basePoints)
    {
        if (State != GameState.Running || basePoints <= 0)
        {
            return;
        }

        Score += Mathf.RoundToInt(basePoints * (IsScoreMultiplied ? scoreMultiplier : 1f));
    }

    public void ApplyScoreMultiplier(float multiplier, float duration)
    {
        scoreMultiplier = multiplier;
        scoreMultiplierUntil = Time.time + duration;
    }

    public void Restart()
    {
        startAfterReload = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        startAfterReload = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SetState(GameState state)
    {
        State = state;

        if (player != null)
        {
            player.canMove = state == GameState.Running;
        }
    }
}
