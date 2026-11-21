using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SnowballProjectile : MonoBehaviour
{
    [Tooltip("Время жизни снежка, секунд.")]
    public float lifetime = 3f;

    private Rigidbody body;
    private Transform planet;
    private float radius;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public void Launch(Transform planetCenter, Vector3 velocity)
    {
        planet = planetCenter;
        radius = Vector3.Distance(transform.position, planet.position);

        Vector3 up = (transform.position - planet.position).normalized;
        body.linearVelocity = Vector3.ProjectOnPlane(velocity, up);

        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        if (planet == null)
        {
            return;
        }

        Vector3 up = (body.position - planet.position).normalized;
        body.position = planet.position + up * radius;
        body.linearVelocity = Vector3.ProjectOnPlane(body.linearVelocity, up);
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
