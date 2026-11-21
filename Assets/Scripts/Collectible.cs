using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
    protected virtual AudioManager.Sfx PickupSound => AudioManager.Sfx.Pickup;

    protected virtual void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

        if (player == null || GameManager.Instance == null || GameManager.Instance.State != GameState.Running)
        {
            return;
        }

        OnCollect(player);
        AudioManager.Play(PickupSound);
        Destroy(gameObject);
    }

    protected abstract void OnCollect(PlayerMovement player);
}
