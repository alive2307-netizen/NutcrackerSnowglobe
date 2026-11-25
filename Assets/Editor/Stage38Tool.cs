using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Stage38Tool
{
    private static readonly Color Gold = new Color(1f, 0.85f, 0.4f);
    private static readonly Color Body = new Color(0.92f, 0.95f, 1f);

    private const float CalibrateBottomMargin = 60f;
    private const float ButtonGap = 20f;

    private static readonly Color[] Palette =
    {
        new Color(1f, 0.25f, 0.25f),
        new Color(1f, 0.82f, 0.25f),
        new Color(0.3f, 0.6f, 1f),
        new Color(0.4f, 1f, 0.5f),
        new Color(0.62f, 0.25f, 0.95f),
        new Color(1f, 1f, 1f)
    };

    private const string Story =
        "Ночью на подоконнике треснул ёлочный снежный шар. Из трещины выкатились все игрушки и разлетелись по белой планете. " +
        "Теперь они лежат на её склонах, а вокруг шныряют мыши: каждая мечтает утащить игрушку в свою нору.\n\n" +
        "Щелкунчик проснулся от звона стекла. Праздник в опасности, и справиться может только он. " +
        "Беги по кругу планеты, собирай игрушки и возвращай их на ёлку, пока мыши не унесли больше, чем ты успел спасти.\n\n" +
        "Собери буквы фразы «С НОВЫМ ГОДОМ», разбросанные по планете, и укрась ёлку до конца. Тогда праздник вернётся домой.";

    private const string Controls =
        "Компьютер: W / S или стрелки — вперёд и назад. A / D или стрелки — поворот. Пробел — прыжок. Левая кнопка мыши — бросок снежка.\n" +
        "Телефон: джойстик слева, кнопка «ПРЫЖОК» и кнопка броска справа.";

    private const string Items =
        "<color=#FFD966>●</color> Игрушка — наполняет шкалу ёлки.\n" +
        "<color=#FFD966>●</color> Мишура — очки удваиваются на 10 секунд.\n" +
        "<color=#FFD966>●</color> Гирлянда — три части дают неуязвимость на время.\n" +
        "<color=#FFD966>●</color> Сугроб — замедляет до половины скорости на 5 секунд.\n" +
        "<color=#FFD966>●</color> Каток — разгоняет до 16 м/с на 10 секунд.\n" +
        "<color=#FFD966>●</color> Буквы — собери все 11 букв фразы. Победа: буквы собраны и ёлка полностью украшена.\n" +
        "<color=#FFD966>●</color> Мышь — крадёт игрушки. Если украла больше, чем собрано, — поражение.";

    [MenuItem("Nutcracker/Apply Stage 38 (about screen)")]
    public static void Apply()
    {
        RoundUI ui = Object.FindFirstObjectByType<RoundUI>();

        if (ui == null || ui.menuPanel == null || ui.optionsButton == null || ui.backButton == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Откройте сцену SnowballLevel с RoundUI. Сначала примените этапы 5, 21 и 34.", "Ок");
            return;
        }

        if (Object.FindFirstObjectByType<AboutScreen>() != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Этап 38 уже применён к этой сцене.", "Ок");
            return;
        }

        Canvas canvas = ui.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Не найден Canvas над RoundUI.", "Ок");
            return;
        }

        RectTransform calibrateRect = canvas.transform.Find("CalibrateButton") as RectTransform;
        if (calibrateRect == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Не найдена кнопка КАЛИБРОВКА. Сначала примените этап 12.", "Ок");
            return;
        }

        RectTransform optionsRect = (RectTransform)ui.optionsButton.transform;
        calibrateRect.anchoredPosition = new Vector2(0f, CalibrateBottomMargin);

        float scale = canvas.transform.lossyScale.y;
        float needed = (2f * optionsRect.sizeDelta.y + 2f * ButtonGap) * scale;
        float actual = optionsRect.position.y - calibrateRect.position.y;
        if (actual < needed)
        {
            optionsRect.position += new Vector3(0f, needed - actual, 0f);
        }

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        Button aboutButton = CloneButton(ui.optionsButton, ui.menuPanel.transform, "AboutButton", "ОБ ИГРЕ");
        RectTransform aboutRect = (RectTransform)aboutButton.transform;
        aboutRect.position = new Vector3(
            optionsRect.position.x,
            (optionsRect.position.y + calibrateRect.position.y) * 0.5f,
            optionsRect.position.z);
        aboutButton.transform.SetSiblingIndex(ui.optionsButton.transform.GetSiblingIndex() + 1);

        GameObject root = new GameObject("AboutScreen", typeof(RectTransform));
        root.transform.SetParent(canvas.transform, false);
        Stretch((RectTransform)root.transform);
        root.transform.SetAsLastSibling();

        GameObject panel = new GameObject("AboutPanel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(root.transform, false);
        Stretch((RectTransform)panel.transform);
        panel.GetComponent<Image>().color = new Color(0.04f, 0.08f, 0.18f, 0.98f);

        AddImage(panel.transform, "Planet", HudSprites.Disc, new Color(0.9f, 0.95f, 1f, 0.06f), new Vector2(-440f, -180f), new Vector2(1000f, 1000f));
        AddImage(panel.transform, "ToyRed", HudSprites.Disc, Palette[0], new Vector2(-840f, 440f), new Vector2(36f, 36f));
        AddImage(panel.transform, "ToyGold", HudSprites.Disc, Palette[1], new Vector2(860f, 430f), new Vector2(30f, 30f));
        AddImage(panel.transform, "ToyBlue", HudSprites.Disc, Palette[2], new Vector2(-860f, -420f), new Vector2(40f, 40f));
        AddImage(panel.transform, "ToyGreen", HudSprites.Disc, Palette[3], new Vector2(880f, -400f), new Vector2(34f, 34f));
        AddImage(panel.transform, "ToyPurple", HudSprites.Disc, Palette[4], new Vector2(760f, 150f), new Vector2(26f, 26f));

        Text title = AddTopText(panel.transform, "Title", "ЩЕЛКУНЧИК И СНЕЖНЫЙ ШАР", 72, Gold, font, TextAnchor.UpperCenter, -60f);
        title.gameObject.AddComponent<Outline>().effectColor = new Color(0.1f, 0.05f, 0.2f, 0.9f);

        GameObject scroll = new GameObject("AboutScroll", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
        scroll.transform.SetParent(panel.transform, false);
        RectTransform scrollRect = (RectTransform)scroll.transform;
        scrollRect.anchorMin = Vector2.zero;
        scrollRect.anchorMax = Vector2.one;
        scrollRect.offsetMin = new Vector2(60f, 200f);
        scrollRect.offsetMax = new Vector2(-60f, -420f);
        scroll.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);

        GameObject viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(RectMask2D));
        viewport.transform.SetParent(scroll.transform, false);
        Stretch((RectTransform)viewport.transform);
        viewport.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);

        GameObject content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        content.transform.SetParent(viewport.transform, false);
        RectTransform contentRect = (RectTransform)content.transform;
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = Vector2.zero;

        VerticalLayoutGroup layout = content.GetComponent<VerticalLayoutGroup>();
        layout.spacing = 24f;
        layout.padding = new RectOffset(20, 20, 20, 20);
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        content.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        AddSection(content.transform, "Story", Story, 34, Body, font, TextAnchor.UpperLeft);
        AddSection(content.transform, "ControlsTitle", "УПРАВЛЕНИЕ", 44, Gold, font, TextAnchor.UpperLeft);
        AddSection(content.transform, "Controls", Controls, 30, Body, font, TextAnchor.UpperLeft);
        AddSection(content.transform, "ItemsTitle", "ПРЕДМЕТЫ И ПРАВИЛА", 44, Gold, font, TextAnchor.UpperLeft);
        AddSection(content.transform, "Items", Items, 28, Body, font, TextAnchor.UpperLeft);

        ScrollRect scrollView = scroll.GetComponent<ScrollRect>();
        scrollView.viewport = (RectTransform)viewport.transform;
        scrollView.content = contentRect;
        scrollView.horizontal = false;
        scrollView.movementType = ScrollRect.MovementType.Clamped;

        Button backButton = CloneButton(ui.backButton, panel.transform, "AboutBackButton", "НАЗАД");
        RectTransform backRect = (RectTransform)backButton.transform;
        backRect.anchorMin = new Vector2(0.5f, 0f);
        backRect.anchorMax = new Vector2(0.5f, 0f);
        backRect.pivot = new Vector2(0.5f, 0f);
        backRect.anchoredPosition = new Vector2(0f, 80f);
        backRect.sizeDelta = new Vector2(340f, 80f);

        root.transform.SetAsLastSibling();
        AboutScreen about = root.AddComponent<AboutScreen>();
        about.panel = panel;
        about.openButton = aboutButton;
        about.backButton = backButton;
        panel.SetActive(false);

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 38 применён: кнопка «Об игре» и экран истории с управлением.");
    }

    private static Button CloneButton(Button source, Transform parent, string name, string caption)
    {
        GameObject go = Object.Instantiate(source.gameObject, parent, false);
        go.name = name;
        Button button = go.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        go.GetComponentInChildren<Text>().text = caption;
        return button;
    }

    private static void AddImage(Transform parent, string name, Sprite sprite, Color color, Vector2 position, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        image.raycastTarget = false;
    }

    private static Text AddTopText(Transform parent, string name, string value, int size, Color color, Font font, TextAnchor alignment, float topOffset)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text), typeof(ContentSizeFitter));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, topOffset);
        rect.sizeDelta = new Vector2(960f, 0f);

        Text text = ConfigureText(go, value, size, color, font, alignment);
        go.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        return text;
    }

    private static void AddSection(Transform parent, string name, string value, int size, Color color, Font font, TextAnchor alignment)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text), typeof(ContentSizeFitter));
        go.transform.SetParent(parent, false);

        ConfigureText(go, value, size, color, font, alignment);
        go.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private static Text ConfigureText(GameObject go, string value, int size, Color color, Font font, TextAnchor alignment)
    {
        Text text = go.GetComponent<Text>();
        text.font = font;
        text.fontSize = size;
        text.color = color;
        text.alignment = alignment;
        text.text = value;
        text.supportRichText = true;
        text.raycastTarget = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
