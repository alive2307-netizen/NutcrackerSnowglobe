using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [Tooltip("Стартовое и максимальное здоровье.")]
    public float maxHealth = 100f;

    public float CurrentHealth { get; private set; }

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f || CurrentHealth <= 0f)
        {
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.State != GameState.Running)
        {
            return;
        }

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        Debug.Log("[Щелкунчик] Получен урон " + amount + ", здоровье: " + CurrentHealth);

        if (CurrentHealth <= 0f && GameManager.Instance != null)
        {
            GameManager.Instance.GameOver("Здоровье закончилось");
        }
    }

    public void Heal(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
    }
}
