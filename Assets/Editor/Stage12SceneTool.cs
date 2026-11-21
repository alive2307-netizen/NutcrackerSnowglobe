using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Этап 12: прячет экранные элементы управления на ПК и добавляет кнопку калибровки гироскопа.
/// Джойстик и кнопка прыжка должны быть созданы по инструкции этапа 4. Запускается через меню Nutcracker.
/// </summary>
public static class Stage12SceneTool
{
    [MenuItem("Nutcracker/Apply Stage 12 (mobile controls)")]
    public static void Apply()
    {
        InputReader input = Object.FindFirstObjectByType<InputReader>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (input == null || canvas == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик",
                "Откройте сцену SnowballLevel с InputReader и Canvas. Сначала примените этапы 1-4.", "Ок");
            return;
        }

        if (Object.FindFirstObjectByType<MobileControls>() != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Этап 12 уже применён к этой сцене.", "Ок");
            return;
        }

        string[] touchNames = { "JoystickBackground", "JoystickHandle", "JumpButton", "JumpLabel" };
        var touchObjects = new System.Collections.Generic.List<GameObject>();

        foreach (string name in touchNames)
        {
            GameObject found = GameObject.Find(name);

            if (found != null)
            {
                touchObjects.Add(found);
            }
        }

        if (touchObjects.Count == 0)
        {
            EditorUtility.DisplayDialog("Щелкунчик",
                "Не найдены JoystickBackground и JumpButton. Создайте их по инструкции этапа 4.", "Ок");
            return;
        }

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        Sprite knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

        GameObject buttonObject = new GameObject("CalibrateButton", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(canvas.transform, false);

        RectTransform rect = (RectTransform)buttonObject.transform;
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, 120f);
        rect.sizeDelta = new Vector2(360f, 100f);

        Image image = buttonObject.GetComponent<Image>();
        image.sprite = knob;
        image.color = new Color(0.6f, 0.85f, 1f, 0.8f);

        GameObject label = new GameObject("Text", typeof(RectTransform), typeof(Text));
        label.transform.SetParent(buttonObject.transform, false);
        RectTransform labelRect = (RectTransform)label.transform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        Text text = label.GetComponent<Text>();
        text.font = font;
        text.text = "КАЛИБРОВКА";
        text.fontSize = 36;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(0.05f, 0.1f, 0.2f);
        text.raycastTarget = false;

        MobileControls mobile = canvas.gameObject.AddComponent<MobileControls>();
        mobile.input = input;
        mobile.touchControls = touchObjects.ToArray();
        mobile.calibrateButton = buttonObject.GetComponent<Button>();

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 12 применён: экранное управление на телефоне, кнопка калибровки.");
    }
}
