using UnityEngine;

/// <summary>
/// Бег по планете: персонаж не может остановиться. Ось X ввода поворачивает курс,
/// ось Y меняет скорость в пределах minSpeed..maxSpeed, прыжок - импульс вдоль нормали.
/// Замедление (сугроб) и ускорение (лёд) накладываются множителем на целевую скорость.
/// Вращением управляет PlanetGravity, здесь только передаётся поворот через AddYaw.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(PlanetGravity))]
[DefaultExecutionOrder(-10)]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public InputReader input;

    [Header("Run")]
    [Tooltip("Скорость без наклона джойстика вперёд-назад, м/с.")]
    public float baseSpeed = 10f;

    public float minSpeed = 6f;
    public float maxSpeed = 16f;

    [Tooltip("Верхняя граница скорости с учётом ускорения от льда, м/с.")]
    public float maxBoostedSpeed = 28f;

    [Tooltip("Как быстро скорость догоняет целевую, м/с².")]
    public float acceleration = 12f;

    [Tooltip("Скорость поворота при полном отклонении, градусов в секунду.")]
    public float turnSpeed = 140f;

    [Header("Jump")]
    [Tooltip("Начальная скорость прыжка вдоль нормали, м/с.")]
    public float jumpImpulse = 12f;

    [Tooltip("Длина луча вниз от центра: половина высоты капсулы плюс запас.")]
    public float groundCheckDistance = 1.2f;

    public LayerMask groundMask;

    [Header("State")]
    public bool canMove = true;

    public bool IsGrounded { get; private set; }
    public float CurrentSpeed => speed;
    public bool IsSlowed => Time.time < slowUntil;
    public bool IsBoosted => Time.time < boostUntil;

    private Rigidbody body;
    private PlanetGravity planetGravity;
    private float speed;
    private bool jumpRequested;

    private float slowUntil;
    private float slowMultiplier = 1f;
    private float boostUntil;
    private float boostMultiplier = 1f;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        planetGravity = GetComponent<PlanetGravity>();
        speed = baseSpeed;
    }

    /// <summary>Замедление на duration секунд (сугроб). Повторное касание продлевает таймер.</summary>
    public void ApplySlow(float multiplier, float duration)
    {
        slowMultiplier = multiplier;
        slowUntil = Time.time + duration;
        Debug.Log("Замедление x" + multiplier + " на " + duration + " с");
    }

    /// <summary>Ускорение на duration секунд (лёд). Повторное касание продлевает таймер.</summary>
    public void ApplySpeedBoost(float multiplier, float duration)
    {
        boostMultiplier = multiplier;
        boostUntil = Time.time + duration;
        Debug.Log("Ускорение x" + multiplier + " на " + duration + " с");
    }

    private void Update()
    {
        if (canMove && input.JumpPressed)
        {
            jumpRequested = true;
        }
    }

    private void FixedUpdate()
    {
        Vector3 up = planetGravity.Up;
        Vector3 vertical = Vector3.Project(body.linearVelocity, up);

        IsGrounded = Physics.Raycast(
            body.position, -up, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore);

        if (!canMove)
        {
            speed = baseSpeed;
            jumpRequested = false;
            body.linearVelocity = vertical;
            return;
        }

        Vector2 move = input.Move;

        planetGravity.AddYaw(move.x * turnSpeed * Time.fixedDeltaTime);

        float targetSpeed = move.y >= 0f
            ? baseSpeed + move.y * (maxSpeed - baseSpeed)
            : baseSpeed + move.y * (baseSpeed - minSpeed);

        if (IsSlowed)
        {
            targetSpeed = baseSpeed * slowMultiplier;
        }
        else if (IsBoosted)
        {
            targetSpeed = baseSpeed * boostMultiplier;
        }

        targetSpeed = Mathf.Clamp(targetSpeed, minSpeed * 0.5f, maxBoostedSpeed);
        speed = Mathf.MoveTowards(speed, targetSpeed, acceleration * Time.fixedDeltaTime);

        Vector3 forward = Vector3.ProjectOnPlane(body.rotation * Vector3.forward, up).normalized;
        body.linearVelocity = forward * speed + vertical;

        if (jumpRequested && IsGrounded)
        {
            body.AddForce(up * jumpImpulse, ForceMode.VelocityChange);
        }

        jumpRequested = false;
    }
}
