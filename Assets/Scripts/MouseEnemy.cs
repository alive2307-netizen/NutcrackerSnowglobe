using UnityEngine;

public class MouseEnemy : MonoBehaviour
{
    [Tooltip("Центр планеты.")]
    public Transform planet;

    [Tooltip("Радиус планеты SnowballPlanet, м.")]
    public float planetRadius = 30f;

    [Tooltip("Высота мыши над поверхностью, м.")]
    public float surfaceLift = 0.5f;

    [Tooltip("Ось большого круга, по которому бегает мышь. Должна совпадать с трассой игрока.")]
    public Vector3 orbitAxis = Vector3.right;

    [Tooltip("Скорость по кругу, градусов в секунду.")]
    public float orbitSpeed = 20f;

    [Tooltip("Стартовый угол на круге, градусов.")]
    public float startAngle;

    [Tooltip("Урон игроку при касании.")]
    public float damageToPlayer = 15f;

    private float angle;

    private void Start()
    {
        angle = startAngle;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.State != GameState.Running)
        {
            return;
        }

        angle += orbitSpeed * Time.deltaTime;

        Vector3 axis = orbitAxis.normalized;
        Vector3 a = Vector3.Cross(axis, Vector3.up);

        if (a.sqrMagnitude < 0.0001f)
        {
            a = Vector3.Cross(axis, Vector3.forward);
        }

        a.Normalize();
        Vector3 b = Vector3.Cross(axis, a);

        float rad = angle * Mathf.Deg2Rad;
        Vector3 normal = (a * Mathf.Cos(rad) + b * Mathf.Sin(rad)).normalized;
        Vector3 velocity = -a * Mathf.Sin(rad) + b * Mathf.Cos(rad);

        transform.position = planet.position + normal * (planetRadius + surfaceLift);
        transform.rotation = Quaternion.LookRotation(velocity, normal);
    }

    private void OnTriggerEnter(Collider other)
    {
        SnowballProjectile projectile = other.GetComponent<SnowballProjectile>();

        if (projectile != null)
        {
            Destroy(projectile.gameObject);
            Destroy(gameObject);
            return;
        }

        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

        if (player == null)
        {
            return;
        }

        HealthManager health = player.GetComponent<HealthManager>();

        if (health == null || health.IsInvincible)
        {
            return;
        }

        health.TakeDamage(damageToPlayer);
        Destroy(gameObject);
    }
}
