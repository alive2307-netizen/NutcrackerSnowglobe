using UnityEngine;

public class SnowballProjectile : MonoBehaviour
{
    [Tooltip("Время жизни снежка, секунд.")]
    public float lifetime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<PlayerMovement>() != null)
        {
            return;
        }

        Destroy(gameObject);
    }
}
