using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlanetGravity))]
public class SnowballShooter : MonoBehaviour
{
    [Tooltip("Пауза между бросками, секунд.")]
    public float cooldown = 0.4f;

    [Tooltip("Максимум снежков в запасе. В начале раунда запас пуст.")]
    public int maxAmmo = 5;

    [Tooltip("Радиус поражения, градусов по поверхности планеты. Мыши дальше этого угла не задеваются.")]
    public float hitAngle = 12f;

    public int Ammo { get; private set; }

    private float nextShotTime;
    private PlanetGravity planetGravity;

    private void Awake()
    {
        planetGravity = GetComponent<PlanetGravity>();
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

        if (Time.time < nextShotTime || Ammo <= 0)
        {
            return;
        }

        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Running)
        {
            return;
        }

        nextShotTime = Time.time + cooldown;
        Ammo--;
        AudioManager.Play(AudioManager.Sfx.Shot);

        Vector3 center = planetGravity.planet.position;
        Vector3 playerDir = (transform.position - center).normalized;

        foreach (MouseEnemy mouse in FindObjectsByType<MouseEnemy>(FindObjectsSortMode.None))
        {
            Vector3 mouseDir = (mouse.transform.position - center).normalized;

            if (Vector3.Angle(playerDir, mouseDir) <= hitAngle)
            {
                mouse.Burst();
            }
        }
    }
}
