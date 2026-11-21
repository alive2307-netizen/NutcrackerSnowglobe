using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [Tooltip("Сколько единиц нужно собрать, чтобы Ёлка-2 заполнилась и раунд был выигран.")]
    public float maxHealth = 100f;

    [Tooltip("Сколько частей гирлянды нужно собрать для неуязвимости.")]
    public int garlandPartsNeeded = 3;

    public float CurrentHealth { get; private set; }
    public int GarlandParts { get; private set; }
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

    public bool Steal(float amount)
    {
        if (amount <= 0f || IsInvincible || !IsRunning())
        {
            return false;
        }

        float left = CurrentHealth - amount;

        if (left < 0f)
        {
            CurrentHealth = 0f;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver("Мыши украли больше игрушек, чем вы собрали");
            }

            return true;
        }

        CurrentHealth = left;
        return true;
    }

    public void CollectGarlandPart(float duration)
    {
        if (!IsRunning())
        {
            return;
        }

        GarlandParts++;

        if (GarlandParts >= garlandPartsNeeded)
        {
            GarlandParts = 0;
            MakeInvincible(duration);
        }
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
