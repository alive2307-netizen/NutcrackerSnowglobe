using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Этап 27: звук удара о ёлку с осыпающимся снегом и переключатели звука в виде ползунков в «Опциях».
/// Требует этапы 13 и 22. Запускается через меню Nutcracker.
/// </summary>
public static class Stage27Tool
{
    private const string TreeHitPath = "Assets/Audio/Sfx/tree_hit.wav";

    [MenuItem("Nutcracker/Apply Stage 27 (tree hit sound, sound sliders)")]
    public static void Apply()
    {
        AudioManager audio = Object.FindFirstObjectByType<AudioManager>();
        RoundUI ui = Object.FindFirstObjectByType<RoundUI>();

        if (audio == null || ui == null || ui.optionsPanel == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Откройте сцену SnowballLevel. Сначала примените этапы 13 и 22.", "Ок");
            return;
        }

        if (ui.optionsPanel.transform.Find("MusicSlider") != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Этап 27 уже применён к этой сцене.", "Ок");
            return;
        }

        Transform musicOld = ui.optionsPanel.transform.Find("MusicToggle");
        Transform sfxOld = ui.optionsPanel.transform.Find("SfxToggle");

        if (musicOld == null || sfxOld == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Не найдены MusicToggle и SfxToggle в OptionsPanel. Сначала примените этап 22.", "Ок");
            return;
        }

        AudioClip hit = AssetDatabase.LoadAssetAtPath<AudioClip>(TreeHitPath);

        if (hit == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Не найден tree_hit.wav в Assets/Audio/Sfx. Дождитесь импорта и повторите.", "Ок");
            return;
        }

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        Sprite track = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        Sprite knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

        Object.DestroyImmediate(musicOld.gameObject);
        Object.DestroyImmediate(sfxOld.gameObject);

        MakeSlider(ui.optionsPanel.transform, 0.55f, SoundToggle.Kind.Music, "MusicSlider", font, track, knob);
        MakeSlider(ui.optionsPanel.transform, 0.4f, SoundToggle.Kind.Sfx, "SfxSlider", font, track, knob);

        audio.treeHitSound = hit;
        EditorUtility.SetDirty(audio);

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 27 применён: звук удара о ёлку и ползунки звука.");
    }

    private static void MakeSlider(Transform parent, float anchorY, SoundToggle.Kind kind, string name,
        Font font, Sprite trackSprite, Sprite knobSprite)
    {
        GameObject row = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        row.transform.SetParent(parent, false);

        RectTransform rowRect = (RectTransform)row.transform;
        rowRect.anchorMin = new Vector2(0.5f, anchorY);
        rowRect.anchorMax = new Vector2(0.5f, anchorY);
        rowRect.pivot = new Vector2(0.5f, 0.5f);
        rowRect.anchoredPosition = Vector2.zero;
        rowRect.sizeDelta = new Vector2(900f, 130f);

        Image rowImage = row.GetComponent<Image>();
        rowImage.color = new Color(0f, 0f, 0f, 0f);

        Text label = MakeText(row.transform, font);

        GameObject trackGo = new GameObject("Track", typeof(RectTransform), typeof(Image));
        trackGo.transform.SetParent(row.transform, false);
        RectTransform trackRect = (RectTransform)trackGo.transform;
        trackRect.anchorMin = new Vector2(0.5f, 0.5f);
        trackRect.anchorMax = new Vector2(0.5f, 0.5f);
        trackRect.pivot = new Vector2(0.5f, 0.5f);
        trackRect.anchoredPosition = new Vector2(250f, 0f);
        trackRect.sizeDelta = new Vector2(230f, 88f);
        Image trackImage = trackGo.GetComponent<Image>();
        trackImage.sprite = trackSprite;
        trackImage.raycastTarget = false;

        GameObject knobGo = new GameObject("Knob", typeof(RectTransform), typeof(Image));
        knobGo.transform.SetParent(trackGo.transform, false);
        RectTransform knobRect = (RectTransform)knobGo.transform;
        knobRect.anchorMin = new Vector2(0.5f, 0.5f);
        knobRect.anchorMax = new Vector2(0.5f, 0.5f);
        knobRect.pivot = new Vector2(0.5f, 0.5f);
        knobRect.anchoredPosition = Vector2.zero;
        knobRect.sizeDelta = new Vector2(74f, 74f);
        Image knobImage = knobGo.GetComponent<Image>();
        knobImage.sprite = knobSprite;
        knobImage.color = Color.white;
        knobImage.raycastTarget = false;

        SoundToggle toggle = row.AddComponent<SoundToggle>();
        toggle.kind = kind;
        toggle.track = trackImage;
        toggle.knob = knobRect;
        toggle.label = label;

        row.GetComponent<Button>().onClick.AddListener(toggle.Toggle);
    }

    private static Text MakeText(Transform parent, Font font)
    {
        GameObject go = new GameObject("Label", typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.anchoredPosition = new Vector2(40f, 0f);
        rect.sizeDelta = new Vector2(480f, 110f);

        Text text = go.GetComponent<Text>();
        text.font = font;
        text.fontSize = 40;
        text.alignment = TextAnchor.MiddleLeft;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }
}
