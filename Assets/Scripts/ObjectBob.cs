using UnityEngine;

public class ObjectBob : MonoBehaviour
{
    public Transform visual;
    public float bobHeight = 0.05f;
    public float bobSpeed = 1f;
    public float spinSpeed;
    public float wobbleAngle;
    public bool pulseGlow;

    private static readonly int EmissionId = Shader.PropertyToID("_EmissionColor");

    private Vector3 restPosition;
    private Quaternion restRotation;
    private float phase;
    private float spin;
    private Renderer[] glowRenderers;
    private readonly MaterialPropertyBlock block = new MaterialPropertyBlock();

    private void Start()
    {
        restPosition = visual.localPosition;
        restRotation = visual.localRotation;
        glowRenderers = visual.GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        phase += dt * bobSpeed * 2f * Mathf.PI;
        spin += spinSpeed * dt;

        float lift = bobHeight * 0.5f * (1f - Mathf.Cos(phase));
        float tilt = Mathf.Sin(phase) * wobbleAngle;

        visual.localPosition = restPosition + Vector3.up * lift;
        visual.localRotation = restRotation * Quaternion.Euler(0f, spin, tilt);

        if (pulseGlow)
        {
            Pulse();
        }
    }

    private void Pulse()
    {
        float strength = 0.6f + 0.4f * Mathf.Sin(Time.time * 3f);

        foreach (Renderer r in glowRenderers)
        {
            Material m = r.sharedMaterial;

            if (m == null || !m.IsKeywordEnabled("_EMISSION"))
            {
                continue;
            }

            r.GetPropertyBlock(block);
            block.SetColor(EmissionId, m.GetColor(EmissionId) * strength);
            r.SetPropertyBlock(block);
        }
    }
}
