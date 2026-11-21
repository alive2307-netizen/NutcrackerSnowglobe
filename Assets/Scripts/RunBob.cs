using UnityEngine;

public class RunBob : MonoBehaviour
{
    public Transform visual;
    public PlayerMovement player;

    [Tooltip("Высота подпрыгивания, м.")]
    public float bobHeight = 0.08f;

    [Tooltip("Наклон корпуса вперёд-назад, градусов.")]
    public float tiltAngle = 6f;

    [Tooltip("Шагов в секунду на базовой скорости.")]
    public float stepsPerSecond = 2.5f;

    [Tooltip("Скорость, с которой покачивание нарастает и затухает.")]
    public float blendSpeed = 4f;

    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly Color DeepRed = new Color(0.45f, 0f, 0.02f);
    private static readonly Color BrightRed = new Color(1f, 0.12f, 0.18f);

    private Vector3 restPosition;
    private Quaternion restRotation;
    private float phase;
    private float amount;
    private HealthManager health;
    private Renderer[] renderers;
    private Color[] baseColors;
    private MaterialPropertyBlock block;
    private bool shimmering;

    private void Start()
    {
        restPosition = visual.localPosition;
        restRotation = visual.localRotation;
        health = player.GetComponent<HealthManager>();
        renderers = visual.GetComponentsInChildren<Renderer>();
        baseColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            baseColors[i] = renderers[i].sharedMaterial.color;
        }

        block = new MaterialPropertyBlock();
    }

    private void Update()
    {
        bool running = player.canMove && player.IsGrounded;
        float speedRatio = running && player.baseSpeed > 0f ? player.CurrentSpeed / player.baseSpeed : 0f;

        amount = Mathf.MoveTowards(amount, Mathf.Clamp01(speedRatio), blendSpeed * Time.deltaTime);
        phase = Mathf.Repeat(phase + 2f * Mathf.PI * stepsPerSecond * speedRatio * Time.deltaTime, 2f * Mathf.PI);

        float wave = Mathf.Sin(phase);
        float lift = Mathf.Abs(wave) * bobHeight * amount;
        float tilt = wave * tiltAngle * amount;

        visual.localPosition = restPosition + Vector3.up * lift;
        visual.localRotation = restRotation * Quaternion.Euler(tilt, 0f, 0f);

        if (health != null && health.IsInvincible)
        {
            Shimmer();
        }
        else if (shimmering)
        {
            ClearShimmer();
        }
    }

    private void Shimmer()
    {
        shimmering = true;
        float k = 0.5f + 0.5f * Mathf.Sin(Time.time * 6f);
        Color tint = Color.Lerp(DeepRed, BrightRed, k);

        for (int i = 0; i < renderers.Length; i++)
        {
            block.SetColor(ColorId, Color.Lerp(baseColors[i], tint, 0.85f));
            renderers[i].SetPropertyBlock(block);
        }
    }

    private void ClearShimmer()
    {
        shimmering = false;

        foreach (Renderer r in renderers)
        {
            r.SetPropertyBlock(null);
        }
    }
}
