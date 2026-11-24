using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Stage33Tool
{
    private const int SnowSlots = 5;
    private const int GarlandBulbs = 3;
    private const float SlotSize = 36f;
    private const float SlotGap = 12f;
    private const float BulbGap = 8f;

    private static readonly Color SnowColor = new Color(0.75f, 0.92f, 1f, 1f);
    private static readonly Color PurpleLit = new Color(0.62f, 0.25f, 0.95f, 1f);
    private static readonly Color PurpleDim = new Color(0.3f, 0.22f, 0.4f, 0.45f);

    [MenuItem("Nutcracker/Apply Stage 33 (ring ammo, vertical purple garland bulbs, attack button)")]
    public static void Apply()
    {
        SnowballShooter shooter = Object.FindFirstObjectByType<SnowballShooter>();
        RoundUI ui = Object.FindFirstObjectByType<RoundUI>();
        HealthManager health = Object.FindFirstObjectByType<HealthManager>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (shooter == null || ui == null || health == null || canvas == null)
        {
            EditorUtility.DisplayDialog("Снежочек", "Откройте сцену SnowballLevel с RoundUI. Сначала примените этапы 5, 10, 21 и 32.", "Ок");
            return;
        }

        Transform hud = ui.hudPanel != null ? ui.hudPanel.transform : canvas.transform;

        DestroyNamed("SnowballRings");
        DestroyNamed("GarlandColumn");
        DestroyNamed("AttackButton");
        DestroyNamed("Bulb");

        TreeIndicator tree = Object.FindFirstObjectByType<TreeIndicator>();
        if (tree != null)
        {
            tree.bulbs = new Image[0];
            tree.bulbLevels = new float[0];
            EditorUtility.SetDirty(tree);
        }

        shooter.maxAmmo = SnowSlots;
        EditorUtility.SetDirty(shooter);

        BuildRings(hud, shooter);
        BuildGarlandColumn(hud, health);
        BuildAttackButton(hud, shooter);
        int painted = PaintGarlandMaterials();

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Снежочек] Этап 33 применён: кольца снежков, фиолетовая гирлянда в столбик, кнопка броска. Перекрашено материалов гирлянды: " + painted);

        if (painted == 0)
        {
            Debug.LogWarning("[Снежочек] Материалы PickupLantern не найдены: гирлянда на карте не перекрашена.");
        }
    }

    private static void BuildRings(Transform hud, SnowballShooter shooter)
    {
        GameObject container = new GameObject("SnowballRings", typeof(RectTransform), typeof(AmmoWidget));
        container.transform.SetParent(hud, false);

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
            image.color = SnowColor;
            image.raycastTarget = false;
            slots[i] = image;
        }

        AmmoWidget widget = container.GetComponent<AmmoWidget>();
        widget.shooter = shooter;
        widget.slots = slots;
    }

    private static void BuildGarlandColumn(Transform hud, HealthManager health)
    {
        float height = GarlandBulbs * SlotSize + (GarlandBulbs - 1) * BulbGap;

        GameObject column = new GameObject("GarlandColumn", typeof(RectTransform), typeof(GarlandIndicator));
        column.transform.SetParent(hud, false);

        RectTransform rect = (RectTransform)column.transform;
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.sizeDelta = new Vector2(SlotSize, height);
        rect.anchoredPosition = new Vector2(-40f, -130f);

        Image[] bulbs = new Image[GarlandBulbs];

        for (int i = 0; i < GarlandBulbs; i++)
        {
            GameObject go = new GameObject("GarlandBulb" + i, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(column.transform, false);

            RectTransform bulbRect = (RectTransform)go.transform;
            bulbRect.anchorMin = new Vector2(0.5f, 1f);
            bulbRect.anchorMax = new Vector2(0.5f, 1f);
            bulbRect.pivot = new Vector2(0.5f, 1f);
            bulbRect.sizeDelta = new Vector2(SlotSize, SlotSize);
            bulbRect.anchoredPosition = new Vector2(0f, -i * (SlotSize + BulbGap));

            Image image = go.GetComponent<Image>();
            image.color = PurpleDim;
            image.raycastTarget = false;
            bulbs[i] = image;
        }

        GarlandIndicator indicator = column.GetComponent<GarlandIndicator>();
        indicator.health = health;
        indicator.bulbs = bulbs;
        indicator.litColor = PurpleLit;
        indicator.dimColor = PurpleDim;
    }

    private static void BuildAttackButton(Transform hud, SnowballShooter shooter)
    {
        Sprite knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject go = new GameObject("AttackButton", typeof(RectTransform), typeof(Image), typeof(Button), typeof(AttackButton));
        go.transform.SetParent(hud, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = new Vector2(1f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(180f, 180f);
        rect.anchoredPosition = new Vector2(-170f, 500f);

        Image image = go.GetComponent<Image>();
        image.sprite = knob;
        image.color = SnowColor;

        AttackButton attack = go.GetComponent<AttackButton>();
        attack.shooter = shooter;

        GameObject label = new GameObject("Label", typeof(RectTransform), typeof(Text));
        label.transform.SetParent(go.transform, false);

        RectTransform labelRect = (RectTransform)label.transform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        Text text = label.GetComponent<Text>();
        text.font = font;
        text.text = "СНЕЖОК";
        text.fontSize = 34;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(0.05f, 0.1f, 0.2f);
        text.raycastTarget = false;
    }

    private static int PaintGarlandMaterials()
    {
        string[] guids = AssetDatabase.FindAssets("PickupLantern t:Material");

        foreach (string guid in guids)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
            Color current = material.color;
            material.color = new Color(PurpleLit.r, PurpleLit.g, PurpleLit.b, current.a);

            if (material.HasProperty("_EmissionColor"))
            {
                material.SetColor("_EmissionColor", PurpleLit * 0.6f);
            }

            EditorUtility.SetDirty(material);
        }

        AssetDatabase.SaveAssets();
        return guids.Length;
    }

    private static void DestroyNamed(string name)
    {
        List<GameObject> found = new List<GameObject>();

        foreach (Transform t in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (t.name == name)
            {
                found.Add(t.gameObject);
            }
        }

        foreach (GameObject go in found)
        {
            if (go != null)
            {
                Object.DestroyImmediate(go);
            }
        }
    }
}
