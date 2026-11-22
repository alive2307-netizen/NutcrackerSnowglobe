using UnityEngine;

public class SnowFall : MonoBehaviour
{
    [Tooltip("Снежинок в секунду.")]
    public float rate = 45f;

    [Tooltip("Скорость падения, м/с.")]
    public float fallSpeed = 1.1f;

    [Tooltip("Размер снежинки, м.")]
    public float minSize = 0.05f;

    public float maxSize = 0.12f;

    private ParticleSystem flakes;

    private void Awake()
    {
        GameObject go = new GameObject("SnowFlakes");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = new Vector3(0f, 7f, 14f);

        flakes = go.AddComponent<ParticleSystem>();

        ParticleSystem.MainModule main = flakes.main;
        main.playOnAwake = false;
        main.loop = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(6f, 8f);
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(minSize, maxSize);
        main.startColor = Color.white;
        main.gravityModifier = 0f;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.maxParticles = 400;

        ParticleSystem.EmissionModule emission = flakes.emission;
        emission.rateOverTime = rate;

        ParticleSystem.ShapeModule shape = flakes.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(26f, 1f, 26f);

        ParticleSystem.VelocityOverLifetimeModule velocity = flakes.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.y = new ParticleSystem.MinMaxCurve(-fallSpeed);

        ParticleSystem.NoiseModule noise = flakes.noise;
        noise.enabled = true;
        noise.strength = 0.35f;
        noise.frequency = 0.25f;
        noise.scrollSpeed = 0.2f;
        noise.damping = true;

        ParticleSystem.ColorOverLifetimeModule color = flakes.colorOverLifetime;
        color.enabled = true;

        Gradient fade = new Gradient();
        fade.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.9f, 0.15f), new GradientAlphaKey(0.9f, 0.8f), new GradientAlphaKey(0f, 1f) });
        color.color = new ParticleSystem.MinMaxGradient(fade);

        ParticleSystemRenderer renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default")) { mainTexture = MakeFlakeTexture() };
    }

    private void Update()
    {
        GameState state = GameManager.Instance != null ? GameManager.Instance.State : GameState.Menu;
        bool snowing = state == GameState.Countdown || state == GameState.Running || state == GameState.Defeat;

        if (snowing && !flakes.isEmitting)
        {
            flakes.Play();
        }
        else if (!snowing && flakes.isEmitting)
        {
            flakes.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private static Texture2D MakeFlakeTexture()
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
                float r = Mathf.Sqrt(dx * dx + dy * dy);
                float alpha = Mathf.SmoothStep(1f, 0f, r);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        tex.Apply();
        return tex;
    }
}
