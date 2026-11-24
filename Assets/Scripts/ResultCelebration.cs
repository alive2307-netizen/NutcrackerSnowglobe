using UnityEngine;
using UnityEngine.UI;

public class ResultCelebration : MonoBehaviour
{
    private const int ConfettiCount = 70;
    private const int DotsPerBurst = 18;
    private const int BulbCount = 14;
    private const float FireworkInterval = 0.9f;
    private const float BurstLife = 1.3f;

    private static readonly Color[] Palette =
    {
        new Color(1f, 0.25f, 0.25f),
        new Color(1f, 0.82f, 0.25f),
        new Color(0.3f, 0.6f, 1f),
        new Color(0.4f, 1f, 0.5f),
        new Color(0.62f, 0.25f, 0.95f),
        new Color(1f, 1f, 1f)
    };

    private static readonly Color Gold = new Color(1f, 0.85f, 0.4f);
    private static Sprite triangle;

    private class Dot
    {
        public Image Image;
        public Vector2 Origin;
        public Vector2 Unit;
        public float Distance;
        public float Age;
    }

    private RectTransform content;
    private float width;
    private float height;
    private RectTransform[] confetti;
    private float[] confettiSpeed;
    private float[] confettiSpin;
    private float[] confettiPhase;
    private Image[] bulbs;
    private Color[] bulbColors;
    private readonly System.Collections.Generic.List<Dot> dots = new System.Collections.Generic.List<Dot>();
    private float nextFirework;

    private void Update()
    {
        bool win = GameManager.Instance != null && GameManager.Instance.State == GameState.Win;

        if (!win)
        {
            if (content != null)
            {
                content.gameObject.SetActive(false);
            }

            return;
        }

        if (content == null)
        {
            Build();
        }

        content.gameObject.SetActive(true);

        float dt = Time.unscaledDeltaTime;
        AnimateConfetti(dt);
        AnimateBulbs();
        AnimateFireworks(dt);
    }

    private void Build()
    {
        RectTransform area = (RectTransform)transform;
        width = area.rect.width;
        height = area.rect.height;

        content = NewRect("Content", transform, Vector2.zero, new Vector2(width, height));

        BuildTree();

        Text phrase = AddText(content, "Phrase", "С НОВЫМ ГОДОМ!", 110, Gold, new Vector2(0f, height * 0.3f), new Vector2(width * 0.95f, 170f));
        phrase.gameObject.AddComponent<Outline>().effectColor = new Color(0.1f, 0.05f, 0.2f, 0.9f);

        confetti = new RectTransform[ConfettiCount];
        confettiSpeed = new float[ConfettiCount];
        confettiSpin = new float[ConfettiCount];
        confettiPhase = new float[ConfettiCount];

        for (int i = 0; i < ConfettiCount; i++)
        {
            Vector2 size = new Vector2(Random.Range(12f, 22f), Random.Range(20f, 34f));
            Vector2 position = new Vector2(Random.Range(-width * 0.5f, width * 0.5f), Random.Range(-height * 0.5f, height * 0.5f));
            confetti[i] = AddImage(content, "Confetti", null, Palette[Random.Range(0, Palette.Length)], position, size).rectTransform;
            confettiSpeed[i] = Random.Range(180f, 320f);
            confettiSpin[i] = Random.Range(-240f, 240f);
            confettiPhase[i] = Random.Range(0f, Mathf.PI * 2f);
        }

        nextFirework = 0.2f;
    }

    private void BuildTree()
    {
        float h = Mathf.Min(height * 0.42f, 760f);
        float baseY = -height * 0.22f;

        AddImage(content, "Trunk", null, new Color(0.45f, 0.25f, 0.1f), new Vector2(0f, baseY + h * 0.03f), new Vector2(h * 0.1f, h * 0.14f));

        Sprite tri = Triangle();
        AddImage(content, "TierBottom", tri, new Color(0.1f, 0.5f, 0.2f), new Vector2(0f, baseY + h * 0.1f + h * 0.18f), new Vector2(h * 0.56f, h * 0.36f));
        AddImage(content, "TierMiddle", tri, new Color(0.1f, 0.5f, 0.2f), new Vector2(0f, baseY + h * 0.3f + h * 0.15f), new Vector2(h * 0.44f, h * 0.30f));
        AddImage(content, "TierTop", tri, new Color(0.1f, 0.5f, 0.2f), new Vector2(0f, baseY + h * 0.48f + h * 0.13f), new Vector2(h * 0.32f, h * 0.26f));
        AddImage(content, "Star", HudSprites.Disc, Gold, new Vector2(0f, baseY + h * 0.78f), new Vector2(h * 0.1f, h * 0.1f));

        bulbs = new Image[BulbCount];
        bulbColors = new Color[BulbCount];

        for (int i = 0; i < BulbCount; i++)
        {
            float t = (i + 0.5f) / BulbCount;
            float rel = 0.1f + t * 0.62f;
            float halfWidth = h * Mathf.Max(0.05f, 0.28f * (1f - (rel - 0.1f) / 0.68f));
            float x = Mathf.Sin(i * 2.4f) * halfWidth * 0.8f;
            Vector2 position = new Vector2(x, baseY + h * rel);
            bulbColors[i] = Palette[i % Palette.Length];
            bulbs[i] = AddImage(content, "TreeBulb" + i, HudSprites.Disc, bulbColors[i], position, new Vector2(h * 0.045f, h * 0.045f));
        }
    }

    private void AnimateConfetti(float dt)
    {
        float top = height * 0.5f + 40f;
        float bottom = -height * 0.5f - 40f;

        for (int i = 0; i < confetti.Length; i++)
        {
            Vector2 p = confetti[i].anchoredPosition;
            p.y -= confettiSpeed[i] * dt;
            p.x += Mathf.Sin(Time.unscaledTime * 2f + confettiPhase[i]) * 40f * dt;

            if (p.y < bottom)
            {
                p.y = top;
                p.x = Random.Range(-width * 0.5f, width * 0.5f);
            }

            confetti[i].anchoredPosition = p;
            confetti[i].Rotate(0f, 0f, confettiSpin[i] * dt);
        }
    }

    private void AnimateBulbs()
    {
        for (int i = 0; i < bulbs.Length; i++)
        {
            float pulse = 0.6f + 0.4f * Mathf.Sin(Time.unscaledTime * 6f + i);
            Color c = bulbColors[i];
            bulbs[i].color = new Color(c.r * pulse, c.g * pulse, c.b * pulse, 1f);
        }
    }

    private void AnimateFireworks(float dt)
    {
        nextFirework -= dt;

        if (nextFirework <= 0f)
        {
            nextFirework = FireworkInterval * Random.Range(0.7f, 1.3f);
            SpawnBurst();
        }

        for (int i = dots.Count - 1; i >= 0; i--)
        {
            Dot d = dots[i];
            d.Age += dt;
            float t = d.Age / BurstLife;

            if (t >= 1f)
            {
                Destroy(d.Image.gameObject);
                dots.RemoveAt(i);
                continue;
            }

            float ease = 1f - (1f - t) * (1f - t);
            d.Image.rectTransform.anchoredPosition = d.Origin + d.Unit * d.Distance * ease;
            d.Image.rectTransform.localScale = Vector3.one * (1f - t * 0.5f);
            Color c = d.Image.color;
            d.Image.color = new Color(c.r, c.g, c.b, 1f - t);
        }
    }

    private void SpawnBurst()
    {
        Vector2 origin = new Vector2(Random.Range(-width * 0.4f, width * 0.4f), Random.Range(height * 0.05f, height * 0.35f));
        Color color = Palette[Random.Range(0, Palette.Length)];

        for (int i = 0; i < DotsPerBurst; i++)
        {
            float angle = i * Mathf.PI * 2f / DotsPerBurst;
            Image image = AddImage(content, "Spark", HudSprites.Disc, color, origin, new Vector2(16f, 16f));

            dots.Add(new Dot
            {
                Image = image,
                Origin = origin,
                Unit = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)),
                Distance = Random.Range(height * 0.08f, height * 0.14f),
                Age = 0f
            });
        }
    }

    private static Sprite Triangle()
    {
        if (triangle != null)
        {
            return triangle;
        }

        const int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };
        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            float half = (size - 1) * 0.5f * (1f - y / (float)(size - 1));

            for (int x = 0; x < size; x++)
            {
                bool inside = Mathf.Abs(x - (size - 1) * 0.5f) <= half;
                pixels[y * size + x] = inside ? Color.white : Color.clear;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        triangle = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
        return triangle;
    }

    private static RectTransform NewRect(string name, Transform parent, Vector2 position, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return rect;
    }

    private static Image AddImage(Transform parent, string name, Sprite sprite, Color color, Vector2 position, Vector2 size)
    {
        RectTransform rect = NewRect(name, parent, position, size);
        Image image = rect.gameObject.AddComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static Text AddText(Transform parent, string name, string value, int size, Color color, Vector2 position, Vector2 sizeDelta)
    {
        RectTransform rect = NewRect(name, parent, position, sizeDelta);
        Text text = rect.gameObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = size;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;
        text.raycastTarget = false;
        return text;
    }
}
