using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Running,
    Win,
    Lose
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private static bool startAfterReload;

    [Header("References")]
    public PlayerMovement player;

    public GameState State { get; private set; } = GameState.Menu;
    public string ResultReason { get; private set; } = "";

    public int Score { get; private set; }
    public bool IsScoreMultiplied => Time.time < scoreMultiplierUntil;

    private float scoreMultiplier = 1f;
    private float scoreMultiplierUntil;

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

    public void StartRound()
    {
        if (State == GameState.Running)
        {
            return;
        }

        ResultReason = "";
        Score = 0;
        scoreMultiplierUntil = 0f;
        SetState(GameState.Running);
    }

    public void GameOver(string reason)
    {
        Finish(GameState.Lose, reason);
    }

    public void Win(string reason)
    {
        Finish(GameState.Win, reason);
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

    private void Finish(GameState result, string reason)
    {
        if (State != GameState.Running)
        {
            return;
        }

        ResultReason = reason;
        SetState(result);
        AudioManager.Play(result == GameState.Win ? AudioManager.Sfx.Win : AudioManager.Sfx.Lose);
        Debug.Log("[Щелкунчик] Раунд окончен: " + result + ". " + reason);
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
