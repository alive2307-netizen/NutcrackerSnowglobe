using UnityEngine;

public class SnowballPickup : Collectible
{
    [Tooltip("Сколько снежков добавляет подбор.")]
    public int amount = 3;

    protected override void OnCollect(PlayerMovement player)
    {
        SnowballShooter shooter = player.GetComponent<SnowballShooter>();

        if (shooter != null)
        {
            shooter.AddAmmo(amount);
        }
    }
}
