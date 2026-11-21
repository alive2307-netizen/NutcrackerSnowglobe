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

    [Header("References")]
    public PlayerMovement player;

    [Header("Round")]
    [Tooltip("Сколько секунд нужно продержаться до победы. Временное правило до этапа 10 (звезда на ёлке).")]
    public float roundDuration = 60f;

    public GameState State { get; private set; } = GameState.Menu;
    public float TimeLeft { get; private set; }
    public string ResultReason { get; private set; } = "";

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        SetState(GameState.Menu);
    }

    private void Update()
    {
        if (State != GameState.Running)
        {
            return;
        }

        TimeLeft -= Time.deltaTime;

        if (TimeLeft <= 0f)
        {
            Finish(GameState.Win, "Вы продержались весь раунд");
        }
    }

    public void StartRound()
    {
        if (State == GameState.Running)
        {
            return;
        }

        TimeLeft = roundDuration;
        ResultReason = "";
        SetState(GameState.Running);
    }

    public void GameOver(string reason)
    {
        Finish(GameState.Lose, reason);
    }

    public void Restart()
    {
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
