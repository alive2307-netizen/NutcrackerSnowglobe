using UnityEngine;

/// <summary>
/// Радиальная гравитация к центру планеты и выравнивание тела по нормали поверхности.
/// Единственный скрипт, который пишет вращение Rigidbody: поворот от управления
/// приходит через AddYaw, иначе два скрипта перезаписывали бы вращение друг друга.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlanetGravity : MonoBehaviour
{
    [Header("References")]
    public Transform planet;

    [Header("Gravity")]
    [Tooltip("Ускорение к центру планеты, м/с².")]
    public float gravity = 30f;

    [Tooltip("Скорость выравнивания по нормали, 1/с. Больше значение - жёстче.")]
    public float alignSpeed = 12f;

    public Vector3 Up { get; private set; } = Vector3.up;

    private Rigidbody body;
    private float pendingYaw;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        body.constraints = RigidbodyConstraints.FreezeRotation;
        body.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void AddYaw(float degrees)
    {
        pendingYaw += degrees;
    }

    private void FixedUpdate()
    {
        Up = (body.position - planet.position).normalized;
        body.AddForce(-Up * gravity, ForceMode.Acceleration);

        Quaternion current = body.rotation;
        Quaternion aligned = Quaternion.FromToRotation(current * Vector3.up, Up) * current;
        float blend = 1f - Mathf.Exp(-alignSpeed * Time.fixedDeltaTime);

        // Поворот применяется после сглаживания, иначе выравнивание гасило бы его.
        Quaternion yaw = Quaternion.AngleAxis(pendingYaw, Up);
        pendingYaw = 0f;

        body.MoveRotation(yaw * Quaternion.Slerp(current, aligned, blend));
    }
}
