using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Следит за состоянием раунда: меняет музыку, играет звук выигрыша или поражения
/// и показывает финал. Победа - конфетти у камеры, поражение - мыши, бегущие по экрану.
/// </summary>
public class RoundDirector : MonoBehaviour
{
    private const int MouseCount = 12;
    private const float HalfWidth = 700f;
    private const float Bounce = 14f;

    public GameManager game;
    public RectTransform canvasRect;
    public Transform cameraTransform;

    private GameState lastState = (GameState)(-1);
    private ParticleSystem confetti;
    private RectTransform miceRoot;
    private RectTransform[] mice;
    private float[] speeds;
    private float[] baseY;
    private float[] phases;

    private void Awake()
    {
        CreateConfetti();
        CreateMice();
    }

    private void Update()
    {
        if (game.State != lastState)
        {
            lastState = game.State;
            OnStateChanged(lastState);
        }

        MoveMice();
    }

    private void OnStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                AudioManager.PlayMusic(AudioManager.MusicTrack.Menu);
                miceRoot.gameObject.SetActive(false);
                break;

            case GameState.Countdown:
                AudioManager.Play(AudioManager.Sfx.Click);
                AudioManager.PlayMusic(AudioManager.MusicTrack.Round);
                miceRoot.gameObject.SetActive(false);
                break;

            case GameState.Running:
                break;

            case GameState.Win:
                AudioManager.PlayMusic(AudioManager.MusicTrack.Win);
                AudioManager.Play(AudioManager.Sfx.Win);
                confetti.transform.position = cameraTransform.position + cameraTransform.forward * 7f;
                confetti.Play();
                confetti.Emit(180);
                break;

            case GameState.Lose:
                AudioManager.PlayMusic(AudioManager.MusicTrack.Lose);
                AudioManager.Play(AudioManager.Sfx.Lose);
                miceRoot.gameObject.SetActive(true);
                break;
        }
    }

    private void CreateConfetti()
    {
        GameObject go = new GameObject("Confetti");
        confetti = go.AddComponent<ParticleSystem>();
        go.GetComponent<ParticleSystemRenderer>().sharedMaterial = new Material(Shader.Find("Sprites/Default"));

        ParticleSystem.MainModule main = confetti.main;
        main.playOnAwake = false;
        main.loop = false;
        main.duration = 3f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = 3f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(4f, 9f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.25f, 0.45f);
        main.gravityModifier = 0.6f;
        main.maxParticles = 300;
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.2f, 0.2f), new Color(0.2f, 1f, 0.4f));

        ParticleSystem.EmissionModule emission = confetti.emission;
        emission.enabled = false;

        ParticleSystem.ShapeModule shape = confetti.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 1.5f;
    }

    private void CreateMice()
    {
        GameObject root = new GameObject("RunningMice", typeof(RectTransform));
        root.transform.SetParent(canvasRect, false);

        miceRoot = (RectTransform)root.transform;
        miceRoot.anchorMin = Vector2.zero;
        miceRoot.anchorMax = Vector2.one;
        miceRoot.offsetMin = Vector2.zero;
        miceRoot.offsetMax = Vector2.zero;

        Sprite sprite = MakeMouseSprite();
        mice = new RectTransform[MouseCount];
        speeds = new float[MouseCount];
        baseY = new float[MouseCount];
        phases = new float[MouseCount];

        for (int i = 0; i < MouseCount; i++)
        {
            GameObject m = new GameObject("Mouse", typeof(RectTransform), typeof(Image));
            m.transform.SetParent(miceRoot, false);

            RectTransform rect = (RectTransform)m.transform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(150f, 94f);

            Image image = m.GetComponent<Image>();
            image.sprite = sprite;
            image.raycastTarget = false;

            speeds[i] = (i % 2 == 0 ? 1f : -1f) * Random.Range(320f, 560f);
            baseY[i] = Random.Range(-420f, -120f);
            phases[i] = Random.Range(0f, 2f * Mathf.PI);
            rect.anchoredPosition = new Vector2(Random.Range(-HalfWidth, HalfWidth), baseY[i]);

            mice[i] = rect;
        }

        miceRoot.gameObject.SetActive(false);
    }

    private void MoveMice()
    {
        if (!miceRoot.gameObject.activeSelf)
        {
            return;
        }

        float dt = Time.deltaTime;
        float t = Time.time;

        for (int i = 0; i < mice.Length; i++)
        {
            Vector2 pos = mice[i].anchoredPosition;
            pos.x += speeds[i] * dt;

            if (pos.x > HalfWidth + 100f)
            {
                pos.x = -HalfWidth - 100f;
            }
            else if (pos.x < -HalfWidth - 100f)
            {
                pos.x = HalfWidth + 100f;
            }

            float wave = Mathf.Sin(t * 9f + phases[i]);
            pos.y = baseY[i] + wave * Bounce;

            mice[i].anchoredPosition = pos;
            mice[i].localScale = new Vector3(speeds[i] < 0f ? -1f : 1f, 1f, 1f);
            mice[i].localEulerAngles = new Vector3(0f, 0f, wave * 6f);
        }
    }

    private static Sprite MakeMouseSprite()
    {
        const int w = 128;
        const int h = 80;

        Color fur = new Color(0.62f, 0.62f, 0.68f);
        Color pink = new Color(0.95f, 0.6f, 0.68f);
        Color eye = new Color(0.05f, 0.05f, 0.05f);

        Color[] pixels = new Color[w * h];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                Color c = Color.clear;

                if (Inside(x, y, 104, 42, 3f, 3f))
                {
                    c = eye;
                }
                else if (Inside(x, y, 118, 36, 4f, 4f))
                {
                    c = pink;
                }
                else if (Inside(x, y, 52, 34, 38f, 20f) || Inside(x, y, 96, 38, 20f, 17f)
                         || Inside(x, y, 90, 56, 9f, 9f) || Inside(x, y, 106, 58, 9f, 9f)
                         || NearSegment(x, y, 16f, 30f, 2f, 12f, 2.5f)
                         || (x >= 36 && x <= 42 && y <= 16) || (x >= 60 && x <= 66 && y <= 16))
                {
                    c = fur;
                }

                pixels[y * w + x] = c;
            }
        }

        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0f, 0f, w, h), new Vector2(0.5f, 0.5f), 100f);
    }

    private static bool Inside(float x, float y, float cx, float cy, float rx, float ry)
    {
        float dx = (x - cx) / rx;
        float dy = (y - cy) / ry;
        return dx * dx + dy * dy <= 1f;
    }

    private static bool NearSegment(float x, float y, float x0, float y0, float x1, float y1, float r)
    {
        float vx = x1 - x0;
        float vy = y1 - y0;
        float t = Mathf.Clamp01(((x - x0) * vx + (y - y0) * vy) / (vx * vx + vy * vy));
        float dx = x0 + vx * t - x;
        float dy = y0 + vy * t - y;
        return dx * dx + dy * dy <= r * r;
    }
}
