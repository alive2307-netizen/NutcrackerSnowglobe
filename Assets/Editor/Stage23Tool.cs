using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Этап 23: убирает ёлочный фон (возвращает дефолтную сцену) и пересобирает индикатор ёлки
/// крупнее, с гирляндой, которая загорается по мере заполнения. Запускается через меню Nutcracker.
/// </summary>
public static class Stage23Tool
{
    private const int BulbCount = 12;
    private const float Width = 260f;
    private const float Height = 380f;
    private const int TreeW = 128;
    private const int TreeH = 192;

    private static readonly float[,] Tiers = { { 40f, 120f, 60f }, { 70f, 150f, 48f }, { 100f, 186f, 36f } };
    private static readonly float[] BulbOffsets = { -0.6f, 0.5f, -0.2f, 0.7f, -0.7f, 0.2f, -0.5f, 0.6f, 0f, -0.3f, 0.4f, 0f };

    private static readonly Color[] BulbColors =
    {
        new Color(1f, 0.2f, 0.2f),
        new Color(1f, 0.9f, 0.3f),
        new Color(0.3f, 0.6f, 1f),
        new Color(1f, 1f, 1f)
    };

    [MenuItem("Nutcracker/Apply Stage 23 (default background, bigger tree indicator with garland)")]
    public static void Apply()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        HealthManager health = Object.FindFirstObjectByType<HealthManager>();

        if (canvas == null || health == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик",
                "Откройте сцену SnowballLevel. Сначала примените этапы 5 и 21.", "Ок");
            return;
        }

        GameObject forest = GameObject.Find("BackgroundForest");

        if (forest != null)
        {
            Object.DestroyImmediate(forest);
        }

        Transform old = canvas.transform.Find("TreeIndicator");

        if (old != null)
        {
            Object.DestroyImmediate(old.gameObject);
        }

        BuildIndicator(canvas, health);

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 23 применён: фон по умолчанию, крупный индикатор с гирляндой.");
    }

    private static void BuildIndicator(Canvas canvas, HealthManager health)
    {
        Sprite silhouette = MakeTreeSprite();
        Sprite dot = MakeDotSprite();

        GameObject frame = new GameObject("TreeIndicator", typeof(RectTransform));
        frame.transform.SetParent(canvas.transform, false);

        RectTransform rect = (RectTransform)frame.transform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(40f, -60f);
        rect.sizeDelta = new Vector2(Width, Height);

        CreateChildImage(frame.transform, "Outline", silhouette, new Color(0.1f, 0.3f, 0.15f, 0.5f));

        Image fill = CreateChildImage(frame.transform, "Fill", silhouette, new Color(0.2f, 0.75f, 0.3f, 1f));
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Vertical;
        fill.fillOrigin = (int)Image.OriginVertical.Bottom;

        Image[] bulbs = new Image[BulbCount];
        float[] levels = new float[BulbCount];

        for (int i = 0; i < BulbCount; i++)
        {
            float level = 0.12f + i * (0.8f / (BulbCount - 1));
            float py = level * TreeH;
            float half = HalfWidthAt(py) * 0.8f;
            float px = TreeW * 0.5f + BulbOffsets[i] * half;

            GameObject go = new GameObject("Bulb", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(frame.transform, false);

            RectTransform bulbRect = (RectTransform)go.transform;
            bulbRect.anchorMin = Vector2.zero;
            bulbRect.anchorMax = Vector2.zero;
            bulbRect.pivot = new Vector2(0.5f, 0.5f);
            bulbRect.sizeDelta = new Vector2(20f, 20f);
            bulbRect.anchoredPosition = new Vector2(px / TreeW * Width, level * Height);

            Image image = go.GetComponent<Image>();
            image.sprite = dot;
            image.color = new Color(0.3f, 0.3f, 0.3f, 0.35f);
            image.raycastTarget = false;

            bulbs[i] = image;
            levels[i] = level;
        }

        TreeIndicator indicator = frame.AddComponent<TreeIndicator>();
        indicator.health = health;
        indicator.fill = fill;
        indicator.bulbs = bulbs;
        indicator.bulbLevels = levels;
        indicator.bulbColors = BulbColors;
    }

    private static float HalfWidthAt(float py)
    {
        float best = 0f;

        for (int t = 0; t < 3; t++)
        {
            float bottom = Tiers[t, 0];
            float top = Tiers[t, 1];
            float half = Tiers[t, 2];

            if (py >= bottom && py <= top)
            {
                best = Mathf.Max(best, half * (top - py) / (top - bottom));
            }
        }

        return best;
    }

    private static Image CreateChildImage(Transform parent, string name, Sprite sprite, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static Sprite MakeTreeSprite()
    {
        Color[] pixels = new Color[TreeW * TreeH];

        for (int y = 0; y < TreeH; y++)
        {
            for (int x = 0; x < TreeW; x++)
            {
                bool on = (x >= 56 && x < 72 && y < 42) || HalfWidthAt(y) >= Mathf.Abs(x - 63.5f) && HalfWidthAt(y) > 0f;
                pixels[y * TreeW + x] = on ? Color.white : Color.clear;
            }
        }

        return MakeSprite(TreeW, TreeH, pixels);
    }

    private static Sprite MakeDotSprite()
    {
        const int size = 20;
        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - 9.5f;
                float dy = y - 9.5f;
                pixels[y * size + x] = dx * dx + dy * dy <= 90f ? Color.white : Color.clear;
            }
        }

        return MakeSprite(size, size, pixels);
    }

    private static Sprite MakeSprite(int w, int h, Color[] pixels)
    {
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0f, 0f, w, h), new Vector2(0.5f, 0.5f), 100f);
    }
}
