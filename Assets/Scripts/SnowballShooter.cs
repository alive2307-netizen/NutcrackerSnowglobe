using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlanetGravity))]
public class SnowballShooter : MonoBehaviour
{
    public GameObject snowballPrefab;

    [Tooltip("Скорость снежка, м/с.")]
    public float speed = 25f;

    [Tooltip("Пауза между выстрелами, секунд.")]
    public float cooldown = 0.4f;

    [Tooltip("Снежков в начале раунда.")]
    public int startAmmo = 5;

    [Tooltip("Максимум снежков в запасе.")]
    public int maxAmmo = 10;

    public int Ammo { get; private set; }

    private float nextShotTime;
    private PlanetGravity planetGravity;

    private void Awake()
    {
        planetGravity = GetComponent<PlanetGravity>();
        Ammo = startAmmo;
    }

    public void AddAmmo(int amount)
    {
        Ammo = Mathf.Min(maxAmmo, Ammo + amount);
    }

    private void Update()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }

        if (Time.time < nextShotTime || snowballPrefab == null || Ammo <= 0)
        {
            return;
        }

        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Running)
        {
            return;
        }

        nextShotTime = Time.time + cooldown;
        Ammo--;

        Vector3 up = planetGravity.Up;
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, up).normalized;
        Vector3 spawn = transform.position + up * 1f + forward * 1.2f;

        GameObject snowball = Instantiate(snowballPrefab, spawn, Quaternion.LookRotation(forward, up));
        snowball.GetComponent<Rigidbody>().linearVelocity = forward * speed;
    }
}
