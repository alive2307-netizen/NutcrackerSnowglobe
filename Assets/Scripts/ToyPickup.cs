using UnityEngine;

public class ToyPickup : Collectible
{
    [Tooltip("Сколько единиц шкалы Ёлки-2 добавляет игрушка.")]
    public float progressAmount = 10f;

    protected override void OnCollect(PlayerMovement player)
    {
        HealthManager health = player.GetComponent<HealthManager>();

        if (health != null)
        {
            health.Collect(progressAmount);
        }
    }
}
