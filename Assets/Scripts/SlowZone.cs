using UnityEngine;

/// <summary>Сугроб: тригер, при входе игрока замедляет его и играет звук удара о снег.</summary>
[RequireComponent(typeof(Collider))]
public class SlowZone : MonoBehaviour
{
    [Tooltip("Во сколько раз падает скорость.")]
    public float slowMultiplier = 0.5f;

    [Tooltip("Длительность замедления, секунд.")]
    public float duration = 5f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

        if (player != null)
        {
            player.ApplySlow(slowMultiplier, duration);
            AudioManager.Play(AudioManager.Sfx.SnowHit);
        }
    }
}
