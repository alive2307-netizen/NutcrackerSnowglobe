using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [Tooltip("Сколько единиц нужно набрать, чтобы шкала Ёлки-2 заполнилась и раунд был выигран.")]
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

    public void TakeDamage(float amount)
    {
        if (amount <= 0f || IsInvincible || !IsRunning())
        {
            return;
        }

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
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
