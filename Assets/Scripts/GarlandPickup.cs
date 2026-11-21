using UnityEngine;

public class GarlandPickup : Collectible
{
    [Tooltip("Длительность неуязвимости после сбора всех частей, секунд.")]
    public float duration = 10f;

    protected override void OnCollect(PlayerMovement player)
    {
        HealthManager health = player.GetComponent<HealthManager>();

        if (health != null)
        {
            health.CollectGarlandPart(duration);
        }
    }
}
