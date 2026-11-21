using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Добавляет в HUD раунда строку очков (ScoreText) и связывает её с RoundUI.
/// Запускается один раз через меню Nutcracker.
/// </summary>
public static class Stage7FixTool
{
    [MenuItem("Nutcracker/Apply Stage 7 fix (score HUD)")]
    public static void Apply()
    {
        RoundUI ui = Object.FindFirstObjectByType<RoundUI>();

        if (ui == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик",
                "Откройте сцену SnowballLevel с RoundUI. Сначала примените этап 5.", "Ок");
            return;
        }

        if (ui.scoreText != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Строка очков уже добавлена.", "Ок");
            return;
        }

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject go = new GameObject("ScoreText", typeof(RectTransform), typeof(Text));
        go.transform.SetParent(ui.hudPanel.transform, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, -390f);
        rect.sizeDelta = new Vector2(1000f, 70f);

        Text text = go.GetComponent<Text>();
        text.font = font;
        text.text = "";
        text.fontSize = 44;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(1f, 0.85f, 0.5f);
        text.raycastTarget = false;

        ui.scoreText = text;

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Добавлена строка очков в HUD.");
    }
}
