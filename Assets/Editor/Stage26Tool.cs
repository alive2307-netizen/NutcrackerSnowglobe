using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Этап 26: звуки подбора игрушки, мишуры и гирлянды, обратный отсчёт 3-2-1, пауза 3 с после столкновения
/// со звёздочками, возрождение мышей вне камеры. Требует этапы 5, 13, 22 и 25. Запускается через меню Nutcracker.
/// </summary>
public static class Stage26Tool
{
    private const string Dir = "Assets/Audio/Sfx/";

    [MenuItem("Nutcracker/Apply Stage 26 (pickup sounds, countdown, defeat pause, mouse respawn)")]
    public static void Apply()
    {
        AudioManager audio = Object.FindFirstObjectByType<AudioManager>();
        RoundUI ui = Object.FindFirstObjectByType<RoundUI>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();

        if (audio == null || ui == null || canvas == null || player == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик",
                "Откройте сцену SnowballLevel. Сначала примените этапы 5, 13, 22 и 25.", "Ок");
            return;
        }

        if (GameObject.Find("MouseRespawner") != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Этап 26 уже применён к этой сцене.", "Ок");
            return;
        }

        AudioClip toy = AssetDatabase.LoadAssetAtPath<AudioClip>(Dir + "pick_toy.wav");
        AudioClip tinsel = AssetDatabase.LoadAssetAtPath<AudioClip>(Dir + "pick_tinsel.wav");
        AudioClip garland = AssetDatabase.LoadAssetAtPath<AudioClip>(Dir + "pick_garland.wav");

        if (toy == null || tinsel == null || garland == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик",
                "Не найдены pick_toy.wav, pick_tinsel.wav, pick_garland.wav в Assets/Audio/Sfx. Дождитесь импорта и повторите.", "Ок");
            return;
        }

        audio.toyPickupSound = toy;
        audio.tinselPickupSound = tinsel;
        audio.garlandPickupSound = garland;
        EditorUtility.SetDirty(audio);

        if (player.GetComponent<StunStars>() == null)
        {
            player.gameObject.AddComponent<StunStars>();
        }

        new GameObject("MouseRespawner").AddComponent<MouseRespawner>();

        if (ui.countdownText == null)
        {
            ui.countdownText = MakeCountdownText(canvas.transform);
        }

        EditorUtility.SetDirty(ui);

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 26 применён: звуки подбора, отсчёт, пауза после столкновения, возрождение мышей.");
    }

    private static Text MakeCountdownText(Transform parent)
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject go = new GameObject("Countdown", typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(400f, 300f);

        Text text = go.GetComponent<Text>();
        text.font = font;
        text.fontSize = 200;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(1f, 0.85f, 0.4f);
        text.raycastTarget = false;

        go.SetActive(false);
        return text;
    }
}
