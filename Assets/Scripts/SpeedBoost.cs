using UnityEngine;

/// <summary>Ледяная дорожка: триггер, при входе игрока ускоряет его на несколько секунд.</summary>
[RequireComponent(typeof(Collider))]
public class SpeedBoost : MonoBehaviour
{
    [Tooltip("Во сколько раз растёт скорость.")]
    public float boostMultiplier = 1.6f;

    [Tooltip("Длительность ускорения, секунд.")]
    public float duration = 10f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

        if (player != null)
        {
            player.ApplySpeedBoost(boostMultiplier, duration);
        }
    }
}
