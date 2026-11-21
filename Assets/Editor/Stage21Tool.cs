using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Этап 21: нарядные ёлки вокруг шара, ёлка-индикатор вместо текста и кнопки звука в меню.
/// Запускается через меню Nutcracker, повторный запуск ничего не меняет.
/// </summary>
public static class Stage21Tool
{
    private const string TreePath = "Assets/Prefabs/Tree.prefab";
    private const string DecoratedPath = "Assets/Prefabs/TreeDecorated.prefab";

    [MenuItem("Nutcracker/Apply Stage 21 (forest backdrop, tree indicator, sound settings)")]
    public static void Apply()
    {
        RoundUI ui = Object.FindFirstObjectByType<RoundUI>();
        HealthManager health = Object.FindFirstObjectByType<HealthManager>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (ui == null || health == null || canvas == null || AssetDatabase.LoadAssetAtPath<GameObject>(TreePath) == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик",
                "Откройте сцену SnowballLevel. Сначала примените этапы 5 и 17.", "Ок");
            return;
        }

        if (GameObject.Find("BackgroundForest") != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Этап 21 уже применён к этой сцене.", "Ок");
            return;
        }

        EnsureFolder("Assets", "Materials");

        GameObject decorated = MakeDecoratedTree();
        BuildForest(decorated);
        BuildTreeIndicator(canvas, health);
        BuildSoundButtons(ui);
        ui.healthText.gameObject.SetActive(false);

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 21 применён: фон из ёлок, ёлка-индикатор, настройки звука.");
    }

    private static GameObject MakeDecoratedTree()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(TreePath);

        Object.DestroyImmediate(root.GetComponent<TreeCollision>());
        Object.DestroyImmediate(root.GetComponent<CapsuleCollider>());

        Transform visual = root.transform.Find("Visual");
        Transform parent = visual != null ? visual : root.transform;

        Material[] baubles =
        {
            Mat("PropBaubleRed", new Color(0.85f, 0.08f, 0.1f)),
            Mat("PropBaubleGold", new Color(1f, 0.78f, 0.25f)),
            Mat("PropBaubleBlue", new Color(0.2f, 0.5f, 1f))
        };

        float[,] tiers = { { 0.9f, 1.5f, 1.1f }, { 1.8f, 1.3f, 0.85f }, { 2.6f, 1.1f, 0.55f } };

        for (int t = 0; t < 3; t++)
        {
            float baseY = tiers[t, 0];
            float height = tiers[t, 1];
            float radius = tiers[t, 2];

            for (int k = 0; k < 6; k++)
            {
                float angle = (k * 60f + t * 25f) * Mathf.Deg2Rad;
                float y = baseY + 0.45f * height;
                float r = radius * 0.55f * 0.85f;

                GameObject bauble = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                bauble.name = "Bauble";
                bauble.transform.SetParent(parent, false);
                bauble.transform.localPosition = new Vector3(Mathf.Cos(angle) * r, y, Mathf.Sin(angle) * r);
                bauble.transform.localScale = Vector3.one * 0.22f;
                bauble.GetComponent<Renderer>().sharedMaterial = baubles[(t + k) % baubles.Length];
                Object.DestroyImmediate(bauble.GetComponent<Collider>());
            }
        }

        GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, DecoratedPath);
        PrefabUtility.UnloadPrefabContents(root);
        return saved;
    }

    private static void BuildForest(GameObject decorated)
    {
        GameObject planet = GameObject.Find("SnowballPlanet");
        Vector3 center = planet != null ? planet.transform.position : Vector3.zero;

        GameObject forest = new GameObject("BackgroundForest");
        AddRing(forest.transform, decorated, center, 75f, 24, -28f, 2.5f, 0f);
        AddRing(forest.transform, decorated, center, 92f, 36, -24f, 1.8f, 7f);
    }

    private static void AddRing(Transform parent, GameObject prefab, Vector3 center, float radius, int count, float y, float scale, float offsetDeg)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = (offsetDeg + i * 360f / count) * Mathf.Deg2Rad;
            Vector3 position = center + new Vector3(Mathf.Cos(angle) * radius, y, Mathf.Sin(angle) * radius);

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.name = "BackgroundTree";
            instance.transform.position = position;
            instance.transform.rotation = Quaternion.Euler(0f, (i * 37f) % 360f, 0f);
            instance.transform.localScale = Vector3.one * scale;
        }
    }

    private static void BuildTreeIndicator(Canvas canvas, HealthManager health)
    {
        Sprite silhouette = MakeTreeSprite();

        GameObject frame = new GameObject("TreeIndicator", typeof(RectTransform));
        frame.transform.SetParent(canvas.transform, false);

        RectTransform rect = (RectTransform)frame.transform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(40f, -60f);
        rect.sizeDelta = new Vector2(120f, 180f);

        Image outline = CreateChildImage(frame.transform, "Outline", silhouette, new Color(0.1f, 0.3f, 0.15f, 0.5f));

        Image fill = CreateChildImage(frame.transform, "Fill", silhouette, new Color(0.2f, 0.75f, 0.3f, 1f));
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Vertical;
        fill.fillOrigin = (int)Image.OriginVertical.Bottom;

        TreeIndicator indicator = frame.AddComponent<TreeIndicator>();
        indicator.health = health;
        indicator.fill = fill;
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

    private static void BuildSoundButtons(RoundUI ui)
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        RectTransform play = (RectTransform)ui.playButton.transform;

        MakeToggle(ui.menuPanel.transform, play, 1, SoundToggle.Kind.Music, font);
        MakeToggle(ui.menuPanel.transform, play, 2, SoundToggle.Kind.Sfx, font);
    }

    private static void MakeToggle(Transform parent, RectTransform reference, int index, SoundToggle.Kind kind, Font font)
    {
        GameObject go = new GameObject(kind + "Toggle", typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = reference.anchorMin;
        rect.anchorMax = reference.anchorMax;
        rect.pivot = reference.pivot;
        rect.anchoredPosition = reference.anchoredPosition + new Vector2(0f, -120f * index);
        rect.sizeDelta = new Vector2(420f, 90f);

        go.GetComponent<Image>().color = new Color(0.2f, 0.35f, 0.6f, 0.9f);

        GameObject label = new GameObject("Text", typeof(RectTransform), typeof(Text));
        label.transform.SetParent(go.transform, false);

        RectTransform labelRect = (RectTransform)label.transform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        Text text = label.GetComponent<Text>();
        text.font = font;
        text.fontSize = 34;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;

        SoundToggle toggle = go.AddComponent<SoundToggle>();
        toggle.kind = kind;
        toggle.label = text;

        go.GetComponent<Button>().onClick.AddListener(toggle.Toggle);
    }

    private static Sprite MakeTreeSprite()
    {
        const int w = 128;
        const int h = 192;
        float[,] tiers = { { 40f, 120f, 60f }, { 70f, 150f, 48f }, { 100f, 186f, 36f } };

        Color[] pixels = new Color[w * h];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                bool on = x >= 56 && x < 72 && y < 42;

                for (int t = 0; t < 3 && !on; t++)
                {
                    float bottom = tiers[t, 0];
                    float top = tiers[t, 1];
                    float half = tiers[t, 2];

                    if (y >= bottom && y <= top)
                    {
                        float width = half * (top - y) / (top - bottom);
                        on = Mathf.Abs(x - 63.5f) <= width;
                    }
                }

                pixels[y * w + x] = on ? Color.white : Color.clear;
            }
        }

        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0f, 0f, w, h), new Vector2(0.5f, 0.5f), 100f);
    }

    private static Material Mat(string name, Color color)
    {
        string path = "Assets/Materials/" + name + ".mat";
        Material m = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (m == null)
        {
            m = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(m, path);
        }

        m.color = color;
        m.SetFloat("_Glossiness", 0.9f);
        EditorUtility.SetDirty(m);
        return m;
    }

    private static void EnsureFolder(string parent, string name)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + name))
        {
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
