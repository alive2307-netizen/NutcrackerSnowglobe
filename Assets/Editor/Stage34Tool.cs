using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Stage34Tool
{
    private static readonly Color Gold = new Color(1f, 0.85f, 0.4f);
    private static readonly Color White = Color.white;
    private static readonly Color Dark = new Color(0.1f, 0.1f, 0.15f);

    [MenuItem("Nutcracker/Apply Stage 34 (coins, revive, no score)")]
    public static void Apply()
    {
        RoundUI ui = Object.FindFirstObjectByType<RoundUI>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (ui == null || canvas == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Откройте сцену SnowballLevel с RoundUI. Сначала примените этапы 5, 21 и 33.", "Ок");
            return;
        }

        if (ui.revivePanel != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Этап 34 уже применён к этой сцене.", "Ок");
            return;
        }

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        Sprite knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        Transform root = canvas.transform;

        DestroyNamed("ScoreText");

        Text coins = MakeText(root, font, "", 40, Gold, new Vector2(0f, 1f), new Vector2(40f, -40f), new Vector2(700f, 60f), TextAnchor.MiddleLeft);
        coins.gameObject.name = "CoinsText";

        GameObject revive = NewPanel(root, "RevivePanel", new Color(0.05f, 0.1f, 0.18f, 0.95f));
        MakeText(revive.transform, font, "ВОСКРЕСНУТЬ?", 70, Gold, new Vector2(0.5f, 0.5f), new Vector2(0f, 320f), new Vector2(960f, 120f), TextAnchor.MiddleCenter);
        MakeText(revive.transform, font, "Сохраните весь прогресс и продолжите раунд", 36, White, new Vector2(0.5f, 0.5f), new Vector2(0f, 200f), new Vector2(960f, 80f), TextAnchor.MiddleCenter);
        Button coinButton = MakeButton(revive.transform, font, knob, "ЗА " + GameManager.ReviveCost + " МОНЕТ", new Vector2(0f, 40f));
        Button adButton = MakeButton(revive.transform, font, knob, "ПОСМОТРЕТЬ РЕКЛАМУ", new Vector2(0f, -110f));
        Button giveUp = MakeButton(revive.transform, font, knob, "СДАТЬСЯ", new Vector2(0f, -260f));

        GameObject ad = NewPanel(root, "AdPanel", new Color(0f, 0f, 0f, 0.95f));
        Text adText = MakeText(ad.transform, font, "Реклама (заглушка)", 56, White, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(960f, 200f), TextAnchor.MiddleCenter);

        revive.SetActive(false);
        ad.SetActive(false);
        revive.transform.SetAsLastSibling();
        ad.transform.SetAsLastSibling();

        ui.revivePanel = revive;
        ui.adPanel = ad;
        ui.reviveCoinButton = coinButton;
        ui.reviveAdButton = adButton;
        ui.giveUpButton = giveUp;
        ui.coinsText = coins;
        ui.adText = adText;
        EditorUtility.SetDirty(ui);

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 34 применён: монеты, воскрешение за монеты и рекламу (заглушка), очки убраны из HUD.");
    }

    private static GameObject NewPanel(Transform parent, string name, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        go.GetComponent<Image>().color = color;
        return go;
    }

    private static Button MakeButton(Transform parent, Font font, Sprite sprite, string label, Vector2 position)
    {
        Vector2 size = new Vector2(760f, 130f);
        GameObject go = new GameObject("Button_" + label, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.color = Gold;

        MakeText(go.transform, font, label, 40, Dark, new Vector2(0.5f, 0.5f), Vector2.zero, size, TextAnchor.MiddleCenter);
        return go.GetComponent<Button>();
    }

    private static Text MakeText(Transform parent, Font font, string value, int size, Color color, Vector2 anchor, Vector2 position, Vector2 sizeDelta, TextAnchor alignment)
    {
        GameObject go = new GameObject("Text", typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.anchoredPosition = position;
        rect.sizeDelta = sizeDelta;

        Text text = go.GetComponent<Text>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;
        return text;
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
