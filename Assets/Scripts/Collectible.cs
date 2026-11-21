using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
    protected virtual void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

        if (player == null || GameManager.Instance == null || GameManager.Instance.State != GameState.Running)
        {
            return;
        }

        OnCollect(player);
        Destroy(gameObject);
    }

    protected abstract void OnCollect(PlayerMovement player);
}
