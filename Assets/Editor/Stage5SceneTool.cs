using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Добавляет в открытую сцену SnowballLevel меню, HUD раунда и экраны результата,
/// а также компонент HealthManager на Player. Запускается один раз через меню Nutcracker.
/// </summary>
public static class Stage5SceneTool
{
    [MenuItem("Nutcracker/Apply Stage 5 (menu, round, health)")]
    public static void Apply()
    {
        GameManager game = Object.FindFirstObjectByType<GameManager>();
        PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (game == null || player == null || canvas == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик",
                "Откройте сцену SnowballLevel: в ней должны быть GameManager, Player и Canvas.", "Ок");
            return;
        }

        if (Object.FindFirstObjectByType<RoundUI>() != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Этап 5 уже применён к этой сцене.", "Ок");
            return;
        }

        HealthManager health = player.GetComponent<HealthManager>();
        if (health == null)
        {
            health = player.gameObject.AddComponent<HealthManager>();
        }

        game.player = player;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        Sprite knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        RectTransform root = (RectTransform)canvas.transform;

        GameObject menuPanel = MakePanel("MenuPanel", root, new Color(0.05f, 0.1f, 0.18f, 0.95f));
        MakeText(menuPanel.transform, font, "ЩЕЛКУНЧИК: СНЕЖНЫЙ ШАР", 64,
            new Color(1f, 0.85f, 0.5f), new Vector2(0.5f, 0.75f), Vector2.zero, new Vector2(1000f, 120f));
        MakeText(menuPanel.transform, font,
            "Беги по шару, уклоняйся от ёлок и продержись весь раунд",
            34, new Color(0.8f, 0.9f, 1f), new Vector2(0.5f, 0.62f), Vector2.zero, new Vector2(960f, 100f));
        Button playButton = MakeButton(menuPanel.transform, font, knob, "ИГРАТЬ",
            new Vector2(0.5f, 0.4f), new Vector2(520f, 130f), new Color(1f, 0.82f, 0.35f));

        GameObject hudPanel = MakeContainer("HudPanel", root);
        Text timerText = MakeText(hudPanel.transform, font, "", 44, new Color(1f, 0.95f, 0.8f),
            new Vector2(0.5f, 1f), new Vector2(0f, -250f), new Vector2(1000f, 70f));
        Text healthText = MakeText(hudPanel.transform, font, "", 44, new Color(0.6f, 1f, 0.7f),
            new Vector2(0.5f, 1f), new Vector2(0f, -320f), new Vector2(1000f, 70f));

        GameObject resultPanel = MakePanel("ResultPanel", root, new Color(0.05f, 0.14f, 0.12f, 0.95f));
        Text resultTitle = MakeText(resultPanel.transform, font, "", 80, new Color(1f, 0.9f, 0.4f),
            new Vector2(0.5f, 0.7f), Vector2.zero, new Vector2(1000f, 120f));
        Text resultReason = MakeText(resultPanel.transform, font, "", 38, new Color(0.85f, 0.95f, 1f),
            new Vector2(0.5f, 0.56f), Vector2.zero, new Vector2(960f, 120f));
        Button restartButton = MakeButton(resultPanel.transform, font, knob, "ИГРАТЬ СНОВА",
            new Vector2(0.5f, 0.4f), new Vector2(620f, 130f), new Color(1f, 0.82f, 0.35f));

        RoundUI ui = canvas.gameObject.AddComponent<RoundUI>();
        ui.game = game;
        ui.health = health;
        ui.menuPanel = menuPanel;
        ui.hudPanel = hudPanel;
        ui.resultPanel = resultPanel;
        ui.playButton = playButton;
        ui.restartButton = restartButton;
        ui.resultTitle = resultTitle;
        ui.resultReason = resultReason;
        ui.timerText = timerText;
        ui.healthText = healthText;

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 5 применён: меню, раунд, здоровье, экраны результата.");
    }

    private static GameObject MakePanel(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        Stretch((RectTransform)go.transform);
        go.GetComponent<Image>().color = color;
        return go;
    }

    private static GameObject MakeContainer(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        Stretch((RectTransform)go.transform);
        return go;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static Text MakeText(Transform parent, Font font, string content, int fontSize,
        Color color, Vector2 anchor, Vector2 position, Vector2 size)
    {
        GameObject go = new GameObject("Text", typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Text text = go.GetComponent<Text>();
        text.font = font;
        text.text = content;
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;
        text.raycastTarget = false;
        return text;
    }

    private static Button MakeButton(Transform parent, Font font, Sprite sprite, string label,
        Vector2 anchor, Vector2 size, Color color)
    {
        GameObject go = new GameObject("Button_" + label, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;

        Image image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.color = color;

        MakeText(go.transform, font, label, 48, new Color(0.1f, 0.12f, 0.18f),
            new Vector2(0.5f, 0.5f), Vector2.zero, size);

        return go.GetComponent<Button>();
    }
}
