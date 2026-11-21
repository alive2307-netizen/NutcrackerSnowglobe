using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [Tooltip("Сколько единиц нужно собрать, чтобы Ёлка-2 заполнилась и раунд был выигран.")]
    public float maxHealth = 100f;

    public float CurrentHealth { get; private set; }
    public bool IsInvincible => Time.time < invincibleUntil;

    private float invincibleUntil;

    private void Awake()
    {
        CurrentHealth = 0f;
    }

    public void Collect(float amount)
    {
        if (amount <= 0f || !IsRunning())
        {
            return;
        }

        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);

        if (CurrentHealth >= maxHealth && GameManager.Instance != null)
        {
            GameManager.Instance.Win("Ёлка полностью украшена");
        }
    }

    // Возвращает true, если мышь забрала игрушки: тогда она исчезает.
    public bool LoseAllToys()
    {
        if (CurrentHealth <= 0f || IsInvincible || !IsRunning())
        {
            return false;
        }

        CurrentHealth = 0f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver("Мыши забрали все игрушки");
        }

        return true;
    }

    public void MakeInvincible(float duration)
    {
        invincibleUntil = Mathf.Max(invincibleUntil, Time.time + duration);
    }

    private static bool IsRunning()
    {
        return GameManager.Instance == null || GameManager.Instance.State == GameState.Running;
    }
}
