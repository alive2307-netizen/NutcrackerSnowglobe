using UnityEngine;

public class TinselPickup : Collectible
{
    [Tooltip("Во сколько раз умножаются очки.")]
    public float scoreMultiplier = 2f;

    [Tooltip("Длительность удвоения очков, секунд.")]
    public float duration = 10f;

    protected override void OnCollect(PlayerMovement player)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ApplyScoreMultiplier(scoreMultiplier, duration);
        }
    }
}
