using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Этап 22: переносит переключатели звука в экран «Опции», добавляет кнопки «ОПЦИИ» в меню
/// и «В МЕНЮ» на экран результата. Требует этап 21. Запускается через меню Nutcracker.
/// </summary>
public static class Stage22Tool
{
    [MenuItem("Nutcracker/Apply Stage 22 (options screen, return to menu)")]
    public static void Apply()
    {
        RoundUI ui = Object.FindFirstObjectByType<RoundUI>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (ui == null || canvas == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Откройте сцену SnowballLevel с RoundUI. Сначала примените этапы 5 и 21.", "Ок");
            return;
        }

        if (ui.optionsPanel != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Этап 22 уже применён к этой сцене.", "Ок");
            return;
        }

        Transform musicToggle = ui.menuPanel.transform.Find("MusicToggle");
        Transform sfxToggle = ui.menuPanel.transform.Find("SfxToggle");

        if (musicToggle == null || sfxToggle == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Не найдены MusicToggle и SfxToggle в MenuPanel. Сначала примените этап 21.", "Ок");
            return;
        }

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        Sprite knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

        GameObject options = new GameObject("OptionsPanel", typeof(RectTransform), typeof(Image));
        options.transform.SetParent(canvas.transform, false);
        Stretch((RectTransform)options.transform);
        options.GetComponent<Image>().color = new Color(0.05f, 0.1f, 0.18f, 0.97f);

        Text title = MakeText(options.transform, font, "ОПЦИИ ЗВУКА", 64, new Color(1f, 0.85f, 0.5f),
            new Vector2(0.5f, 0.72f), new Vector2(960f, 120f));

        musicToggle.SetParent(options.transform, false);
        Place((RectTransform)musicToggle, new Vector2(0.5f, 0.55f), new Vector2(0f, 0f));

        sfxToggle.SetParent(options.transform, false);
        Place((RectTransform)sfxToggle, new Vector2(0.5f, 0.4f), new Vector2(0f, 0f));

        Button back = MakeButton(options.transform, font, knob, "НАЗАД",
            new Vector2(0.5f, 0.22f), new Vector2(420f, 110f), new Color(1f, 0.82f, 0.35f));

        options.SetActive(false);

        Button optionsButton = MakeButton(ui.menuPanel.transform, font, knob, "ОПЦИИ",
            new Vector2(0.5f, 0.2f), new Vector2(420f, 110f), new Color(0.6f, 0.8f, 1f));

        Button menuButton = MakeButton(ui.resultPanel.transform, font, knob, "В МЕНЮ",
            new Vector2(0.5f, 0.22f), new Vector2(520f, 120f), new Color(1f, 0.82f, 0.35f));

        ui.optionsPanel = options;
        ui.optionsButton = optionsButton;
        ui.backButton = back;
        ui.menuButton = menuButton;

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 22 применён: экран опций и кнопка «В МЕНЮ».");
    }

    private static void Place(RectTransform rect, Vector2 anchor, Vector2 position)
    {
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static Button MakeButton(Transform parent, Font font, Sprite sprite, string label,
        Vector2 anchor, Vector2 size, Color color)
    {
        GameObject go = new GameObject("Button_" + label, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;
        Place(rect, anchor, Vector2.zero);
        rect.sizeDelta = size;

        Image image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.color = color;

        Text text = MakeText(go.transform, font, label, 38, new Color(0.1f, 0.1f, 0.15f), new Vector2(0.5f, 0.5f), size);
        Stretch((RectTransform)text.transform);

        return go.GetComponent<Button>();
    }

    private static Text MakeText(Transform parent, Font font, string value, int size, Color color, Vector2 anchor, Vector2 sizeDelta)
    {
        GameObject go = new GameObject("Text", typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;
        Place(rect, anchor, Vector2.zero);
        rect.sizeDelta = sizeDelta;

        Text text = go.GetComponent<Text>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;
        text.raycastTarget = false;
        return text;
    }
}
