using UnityEngine;

public class ToyPickup : Collectible
{
    [Tooltip("Сколько единиц шкалы Ёлки-2 добавляет игрушка.")]
    public float progressAmount = 10f;

    [Tooltip("Базовые очки за игрушку, до множителя мишуры.")]
    public int points = 10;

    [Tooltip("Через сколько секунд игры игрушка появится на своём месте.")]
    public float respawnDelay = 90f;

    protected override AudioManager.Sfx PickupSound => AudioManager.Sfx.ToyPickup;

    protected override void Consume()
    {
        gameObject.SetActive(false);
        RespawnTimer.Schedule(gameObject, respawnDelay);
    }

    protected override void OnCollect(PlayerMovement player)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(points);
        }

        HealthManager health = player.GetComponent<HealthManager>();

        if (health != null)
        {
            health.Collect(progressAmount);
        }
    }
}
