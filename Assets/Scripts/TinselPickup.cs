using UnityEngine;

public class TinselPickup : Collectible
{
    [Tooltip("Во сколько раз ускоряется игрок.")]
    public float speedMultiplier = 2f;

    [Tooltip("Длительность ускорения, секунд.")]
    public float duration = 10f;

    protected override void OnCollect(PlayerMovement player)
    {
        player.ApplySpeedBoost(speedMultiplier, duration);
    }
}
