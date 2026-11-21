using UnityEngine;

/// <summary>Сугроб: триггер, при входе игрока замедляет его на несколько секунд.</summary>
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
        }
    }
}
