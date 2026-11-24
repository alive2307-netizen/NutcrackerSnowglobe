using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Stage32Tool
{
    private const int SnowSlots = 5;
    private const float SlotSize = 36f;
    private const float SlotGap = 12f;
    private const float HitAngle = 12f;

    private static readonly Color SnowColor = new Color(0.75f, 0.92f, 1f, 1f);
    private static readonly Color GarlandAmber = new Color(1f, 0.85f, 0.4f, 1f);

    [MenuItem("Nutcracker/Apply Stage 32 (snowball throw radius, ring ammo, bulb garland)")]
    public static void Apply()
    {
        SnowballShooter shooter = Object.FindFirstObjectByType<SnowballShooter>();
        RoundUI ui = Object.FindFirstObjectByType<RoundUI>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (shooter == null || ui == null || canvas == null)
        {
            EditorUtility.DisplayDialog("Снежки", "Откройте сцену SnowballLevel с RoundUI. Сначала примените этапы 5, 10 и 21.", "Ок");
            return;
        }

        EnsureFolder("Assets", "Sprites");
        EnsureFolder("Assets/Sprites", "Stage32");

        Sprite ringSprite = SaveSprite("SnowRing", MakeCircle(false));
        Sprite discSprite = SaveSprite("SnowDisc", MakeCircle(true));
        Sprite bulbSprite = SaveSprite("GarlandBulb", MakeBulb());

        shooter.maxAmmo = SnowSlots;
        shooter.hitAngle = HitAngle;
        EditorUtility.SetDirty(shooter);

        AmmoWidget oldLabel = Object.FindFirstObjectByType<AmmoWidget>();
        if (oldLabel != null)
        {
            Object.DestroyImmediate(oldLabel.gameObject);
        }

        BuildRings(ui.hudPanel != null ? ui.hudPanel.transform : canvas.transform, shooter, ringSprite, discSprite);

        TreeIndicator tree = Object.FindFirstObjectByType<TreeIndicator>();
        if (tree != null)
        {
            foreach (Image bulb in tree.bulbs)
            {
                bulb.sprite = bulbSprite;
                bulb.preserveAspect = true;
                ((RectTransform)bulb.transform).sizeDelta = new Vector2(14f, 22f);
            }

            tree.bulbColors = new[] { GarlandAmber };
            EditorUtility.SetDirty(tree);
        }

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Снежочек] Этап 32 применён: 5 снежков, кольца в углу, лампочки гирлянды, радиус броска.");
    }

    private static void BuildRings(Transform parent, SnowballShooter shooter, Sprite ringSprite, Sprite discSprite)
    {
        GameObject container = new GameObject("SnowballRings", typeof(RectTransform), typeof(AmmoWidget));
        container.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)container.transform;
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.sizeDelta = new Vector2(SnowSlots * SlotSize + (SnowSlots - 1) * SlotGap, SlotSize);
        rect.anchoredPosition = new Vector2(-40f, -40f);

        Image[] slots = new Image[SnowSlots];

        for (int i = 0; i < SnowSlots; i++)
        {
            GameObject go = new GameObject("Snowball" + i, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(container.transform, false);

            RectTransform slotRect = (RectTransform)go.transform;
            slotRect.anchorMin = new Vector2(0f, 0.5f);
            slotRect.anchorMax = new Vector2(0f, 0.5f);
            slotRect.pivot = new Vector2(0f, 0.5f);
            slotRect.sizeDelta = new Vector2(SlotSize, SlotSize);
            slotRect.anchoredPosition = new Vector2(i * (SlotSize + SlotGap), 0f);

            Image image = go.GetComponent<Image>();
            image.sprite = ringSprite;
            image.color = SnowColor;
            image.raycastTarget = false;
            slots[i] = image;
        }

        AmmoWidget widget = container.GetComponent<AmmoWidget>();
        widget.shooter = shooter;
        widget.slots = slots;
        widget.emptySprite = ringSprite;
        widget.filledSprite = discSprite;
    }

    private static Texture2D MakeCircle(bool filled)
    {
        const int size = 64;
        const float thickness = 7f;
        float radius = size * 0.5f - 2f;
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                float alpha = Mathf.Clamp01(radius - d + 0.5f);

                if (!filled)
                {
                    alpha *= Mathf.Clamp01(d - (radius - thickness) + 0.5f);
                }

                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        return BuildTexture(size, size, pixels);
    }

    private static Texture2D MakeBulb()
    {
        const int width = 64;
        const int height = 96;
        Vector2 center = new Vector2(32f, 60f);
        const float rx = 24f;
        const float ry = 28f;
        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float px = x + 0.5f;
                float py = y + 0.5f;
                float nx = (px - center.x) / rx;
                float ny = (py - center.y) / ry;
                float d = Mathf.Sqrt(nx * nx + ny * ny);
                float body = Mathf.Clamp01((1f - d) * rx + 0.5f);
                float neck = Mathf.Abs(px - 32f) <= 7f && py >= 14f && py <= 34f ? 1f : 0f;
                pixels[y * width + x] = new Color(1f, 1f, 1f, Mathf.Max(body, neck));
            }
        }

        return BuildTexture(width, height, pixels);
    }

    private static Texture2D BuildTexture(int width, int height, Color[] pixels)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    private static Sprite SaveSprite(string name, Texture2D texture)
    {
        string path = "Assets/Sprites/Stage32/" + name + ".png";
        File.WriteAllBytes(Path.Combine(Application.dataPath, "Sprites/Stage32", name + ".png"), texture.EncodeToPNG());
        Object.DestroyImmediate(texture);

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit = 100f;
        importer.alphaIsTransparency = true;
        importer.filterMode = FilterMode.Bilinear;
        importer.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static void EnsureFolder(string parent, string name)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + name))
        {
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
