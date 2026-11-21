using UnityEngine;

/// <summary>
/// Синглтон состояния игры. На этапе 4 умеет только одно: обработать гибель игрока
/// (остановить бег, написать в консоль, перезагрузить сцену). Здоровье и экраны
/// победы и поражения добавляются на следующих этапах.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public PlayerMovement player;

    [Header("Game Over")]
    [Tooltip("Пауза перед перезагрузкой сцены, секунд.")]
    public float restartDelay = 1.5f;

    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    public void GameOver(string reason)
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;
        Debug.Log("Игра окончена: " + reason + ". Перезапуск через " + restartDelay + " с");

        if (player != null)
        {
            player.canMove = false;
        }

        Invoke(nameof(RestartScene), restartDelay);
    }

    private void RestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
