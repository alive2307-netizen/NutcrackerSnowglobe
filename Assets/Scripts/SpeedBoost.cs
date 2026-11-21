using UnityEngine;

/// <summary>Каток: тригер, при входе игрока ускоряет его, пока он на льду, и играет звук скольжения.</summary>
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
            AudioManager.SetSkating(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() != null)
        {
            AudioManager.SetSkating(false);
        }
    }
}
