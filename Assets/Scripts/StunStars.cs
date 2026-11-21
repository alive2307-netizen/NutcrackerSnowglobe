using UnityEngine;

/// <summary>Звёздочки над головой щелкунчика, пока раунд стоит на паузе после столкновения.</summary>
public class StunStars : MonoBehaviour
{
    [Tooltip("Высота над центром Player, м. Подбирается по голове модели.")]
    public float height = 1.35f;

    [Tooltip("Радиус круга, по которому летают звёздочки, м.")]
    public float radius = 0.45f;

    private ParticleSystem stars;

    private void Awake()
    {
        GameObject go = new GameObject("StunStars");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = new Vector3(0f, height, 0f);

        stars = go.AddComponent<ParticleSystem>();

        ParticleSystem.MainModule main = stars.main;
        main.playOnAwake = false;
        main.loop = true;
        main.startLifetime = 1.2f;
        main.startSpeed = 0.25f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.22f);
        main.startColor = new Color(1f, 0.85f, 0.3f);
        main.gravityModifier = 0f;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.maxParticles = 40;

        ParticleSystem.EmissionModule emission = stars.emission;
        emission.rateOverTime = 10f;

        ParticleSystem.ShapeModule shape = stars.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = radius;
        shape.rotation = new Vector3(90f, 0f, 0f);

        ParticleSystemRenderer renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default")) { mainTexture = MakeStarTexture() };
    }

    private void Update()
    {
        bool defeat = GameManager.Instance != null && GameManager.Instance.State == GameState.Defeat;

        if (defeat && !stars.isPlaying)
        {
            stars.Play();
        }
        else if (!defeat && stars.isPlaying)
        {
            stars.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private static Texture2D MakeStarTexture()
    {
        const int size = 32;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x + 0.5f) / size * 2f - 1f;
                float dy = (y + 0.5f) / size * 2f - 1f;
                float radial = Mathf.Clamp01(1f - 2.5f * Mathf.Sqrt(dx * dx + dy * dy));
                float armX = Mathf.Clamp01(1f - 10f * Mathf.Abs(dx)) * Mathf.Clamp01(1f - 2.5f * Mathf.Abs(dy));
                float armY = Mathf.Clamp01(1f - 10f * Mathf.Abs(dy)) * Mathf.Clamp01(1f - 2.5f * Mathf.Abs(dx));
                float alpha = Mathf.Max(radial * radial, Mathf.Max(armX, armY));
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        tex.Apply();
        return tex;
    }
}
