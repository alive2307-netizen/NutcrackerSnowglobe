using UnityEngine;
using UnityEngine.UI;

public class ResultCelebration : MonoBehaviour
{
    private const int ConfettiCount = 70;
    private const int DotsPerBurst = 18;
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
        AnimateFireworks(dt);
    }

    private void Build()
    {
        RectTransform area = (RectTransform)transform;
        width = area.rect.width;
        height = area.rect.height;

        content = NewRect("Content", transform, Vector2.zero, new Vector2(width, height));

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
