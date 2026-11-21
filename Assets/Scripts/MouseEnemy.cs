using UnityEngine;

public class MouseEnemy : MonoBehaviour
{
    [Tooltip("Центр планеты.")]
    public Transform planet;

    [Tooltip("Радиус планеты SnowballPlanet, м.")]
    public float planetRadius = 30f;

    [Tooltip("Высота мыши над поверхностью, м.")]
    public float surfaceLift = 0.5f;

    [Tooltip("Ось патрулирования: мышь бегает по большому кругу вокруг неё.")]
    public Vector3 orbitAxis = Vector3.right;

    [Tooltip("Скорость патрулирования, градусов в секунду.")]
    public float orbitSpeed = 20f;

    [Tooltip("Стартовый угол патрулирования, градусов.")]
    public float startAngle;

    [Tooltip("Радиус обнаружения, градусов по поверхности шара. Внутри него мышь начинает погоню.")]
    public float detectAngle = 25f;

    [Tooltip("Если игрок отъехал дальше этого угла, погоня прекращается.")]
    public float loseAngle = 45f;

    [Tooltip("Скорость погони, градусов в секунду. Чуть выше базовой скорости игрока.")]
    public float chaseSpeed = 21f;

    [Tooltip("Сколько единиц шкалы Ёлки-2 крадёт мышь (одна игрушка).")]
    public float stealAmount = 5f;

    private Vector3 surfaceDir;
    private bool chasing;

    private void Start()
    {
        Vector3 axis = orbitAxis.normalized;
        Vector3 a = Vector3.Cross(axis, Vector3.up);

        if (a.sqrMagnitude < 0.0001f)
        {
            a = Vector3.Cross(axis, Vector3.forward);
        }

        a.Normalize();
        Vector3 b = Vector3.Cross(axis, a);

        float rad = startAngle * Mathf.Deg2Rad;
        surfaceDir = (a * Mathf.Cos(rad) + b * Mathf.Sin(rad)).normalized;
    }

    private void Update()
    {
        bool running = GameManager.Instance == null || GameManager.Instance.State == GameState.Running;
        float dt = running ? Time.deltaTime : 0f;
        Vector3 center = planet.position;

        Vector3 motion;

        if (running && GameManager.Instance.player != null)
        {
            Vector3 playerDir = (GameManager.Instance.player.transform.position - center).normalized;
            float toPlayer = Vector3.Angle(surfaceDir, playerDir);

            if (!chasing && toPlayer <= detectAngle)
            {
                chasing = true;
            }
            else if (chasing && toPlayer > loseAngle)
            {
                chasing = false;
            }
        }
        else
        {
            chasing = false;
        }

        if (chasing)
        {
            Vector3 playerDir = (GameManager.Instance.player.transform.position - center).normalized;
            Vector3 rotAxis = Vector3.Cross(surfaceDir, playerDir);
            float angle = Vector3.Angle(surfaceDir, playerDir);

            if (rotAxis.sqrMagnitude < 0.000001f || angle < 0.01f)
            {
                motion = Vector3.zero;
            }
            else
            {
                rotAxis.Normalize();
                float step = Mathf.Min(chaseSpeed * dt, angle);
                surfaceDir = Quaternion.AngleAxis(step, rotAxis) * surfaceDir;
                motion = Vector3.Cross(rotAxis, surfaceDir);
            }
        }
        else
        {
            Vector3 axis = orbitAxis.normalized;
            surfaceDir = Quaternion.AngleAxis(orbitSpeed * dt, axis) * surfaceDir;
            motion = Vector3.Cross(axis, surfaceDir);
        }

        surfaceDir.Normalize();
        transform.position = center + surfaceDir * (planetRadius + surfaceLift);

        if (motion.sqrMagnitude > 0.000001f)
        {
            transform.rotation = Quaternion.LookRotation(motion.normalized, surfaceDir);
        }
    }

    public void Vanish()
    {
        gameObject.SetActive(false);
        MouseRespawner.Instance.Schedule(this);
    }

    public void Respawn(Vector3 playerDirection)
    {
        Vector3 axis = orbitAxis.normalized;
        Vector3 onOrbit = Vector3.ProjectOnPlane(-playerDirection, axis);

        if (onOrbit.sqrMagnitude < 0.01f)
        {
            onOrbit = Vector3.ProjectOnPlane(Vector3.forward, axis);
        }

        surfaceDir = onOrbit.normalized;
        chasing = false;
        transform.position = planet.position + surfaceDir * (planetRadius + surfaceLift);
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        SnowballProjectile projectile = other.GetComponent<SnowballProjectile>();

        if (projectile != null)
        {
            Destroy(projectile.gameObject);
            AudioManager.Play(AudioManager.Sfx.Splat);
            Vanish();
            return;
        }

        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

        if (player == null)
        {
            return;
        }

        HealthManager health = player.GetComponent<HealthManager>();

        if (health == null || !health.Steal(stealAmount))
        {
            return;
        }

        AudioManager.Play(AudioManager.Sfx.Bite);
        Vanish();
    }
}
