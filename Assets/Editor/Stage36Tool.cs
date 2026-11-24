using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Stage36Tool
{
    [MenuItem("Nutcracker/Apply Stage 36 (speed HUD, shop, victory)")]
    public static void Apply()
    {
        RoundUI ui = Object.FindFirstObjectByType<RoundUI>();
        PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();

        if (ui == null || ui.hudPanel == null || player == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Откройте сцену SnowballLevel с RoundUI и игроком. Сначала примените этапы 5, 21 и 34.", "Ок");
            return;
        }

        Canvas canvas = ui.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Не найден Canvas над RoundUI.", "Ок");
            return;
        }

        RemoveOldHud(canvas.transform);
        RemovePhrase();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        CreateSpeedHud(ui, player, font);

        if (Object.FindFirstObjectByType<ShopMenu>() == null)
        {
            CreateShop(canvas, font);
        }

        if (Object.FindFirstObjectByType<ResultCelebration>() == null)
        {
            GameObject celebration = new GameObject("ResultCelebration", typeof(RectTransform));
            celebration.transform.SetParent(canvas.transform, false);
            Stretch((RectTransform)celebration.transform);
            celebration.AddComponent<ResultCelebration>();
        }

        Stage35Tool.Apply();

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 36 применён: старая подсказка и скорость убраны, фраза вверху, магазин, экран победы.");
    }

    private static void RemoveOldHud(Transform canvasRoot)
    {
        foreach (Transform t in canvasRoot.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == "Hint" || t.name == "SpeedHud" || t.name == "SpeedLabel")
            {
                Object.DestroyImmediate(t.gameObject);
            }
        }

        foreach (SpeedHud old in Object.FindObjectsByType<SpeedHud>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            Object.DestroyImmediate(old);
        }
    }

    private static void RemovePhrase()
    {
        foreach (LetterSpawner spawner in Object.FindObjectsByType<LetterSpawner>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            Object.DestroyImmediate(spawner.gameObject);
        }

        foreach (LetterPickup pickup in Object.FindObjectsByType<LetterPickup>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            Object.DestroyImmediate(pickup.gameObject);
        }

        foreach (PhraseProgress progress in Object.FindObjectsByType<PhraseProgress>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            Object.DestroyImmediate(progress.gameObject);
        }
    }

    private static void CreateSpeedHud(RoundUI ui, PlayerMovement player, Font font)
    {
        Text speedLabel = CreateText("SpeedLabel", ui.hudPanel.transform, font, 36, TextAnchor.UpperLeft,
            new Vector2(0f, 1f), new Vector2(20f, -90f), new Vector2(300f, 50f));
        SpeedHud speedHud = speedLabel.gameObject.AddComponent<SpeedHud>();
        speedHud.player = player;
        speedHud.label = speedLabel;
    }

    private static void CreateShop(Canvas canvas, Font font)
    {
        GameObject shopRoot = new GameObject("ShopMenu", typeof(RectTransform));
        shopRoot.transform.SetParent(canvas.transform, false);
        Stretch(shopRoot.GetComponent<RectTransform>());

        Button openButton = CreateButton("ShopButton", shopRoot.transform, font, "МАГАЗИН",
            new Vector2(0f, 1f), new Vector2(20f, -160f), new Vector2(240f, 70f));

        GameObject panel = new GameObject("ShopPanel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(shopRoot.transform, false);
        Stretch(panel.GetComponent<RectTransform>());
        panel.GetComponent<Image>().color = new Color(0.05f, 0.1f, 0.2f, 0.92f);

        Text coinsText = CreateText("CoinsText", panel.transform, font, 48, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f), new Vector2(0f, 80f), new Vector2(800f, 80f));

        Button closeButton = CreateButton("CloseButton", panel.transform, font, "ЗАКРЫТЬ",
            new Vector2(0.5f, 0.5f), new Vector2(0f, -80f), new Vector2(300f, 80f));

        ShopMenu shop = shopRoot.AddComponent<ShopMenu>();
        shop.openButton = openButton;
        shop.closeButton = closeButton;
        shop.panel = panel;
        shop.coinsText = coinsText;
        panel.SetActive(false);
    }

    private static Text CreateText(string name, Transform parent, Font font, int size, TextAnchor alignment,
        Vector2 anchor, Vector2 position, Vector2 dimensions)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.anchoredPosition = position;
        rect.sizeDelta = dimensions;

        Text text = go.GetComponent<Text>();
        text.font = font;
        text.fontSize = size;
        text.alignment = alignment;
        text.raycastTarget = false;
        return text;
    }

    private static Button CreateButton(string name, Transform parent, Font font, string caption,
        Vector2 anchor, Vector2 position, Vector2 dimensions)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.anchoredPosition = position;
        rect.sizeDelta = dimensions;
        go.GetComponent<Image>().color = new Color(1f, 0.82f, 0.25f);

        Text label = CreateText("Label", go.transform, font, 36, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f), Vector2.zero, dimensions);
        label.text = caption;
        label.color = new Color(0.1f, 0.1f, 0.1f);

        return go.GetComponent<Button>();
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
