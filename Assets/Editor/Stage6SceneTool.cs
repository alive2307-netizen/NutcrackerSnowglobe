using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Добавляет на Canvas сцены SnowballLevel индикатор Ёлка-2 (левый верхний угол).
/// Запускается один раз через меню Nutcracker.
/// </summary>
public static class Stage6SceneTool
{
    [MenuItem("Nutcracker/Apply Stage 6 (tree health indicator)")]
    public static void Apply()
    {
        GameManager game = Object.FindFirstObjectByType<GameManager>();
        HealthManager health = Object.FindFirstObjectByType<HealthManager>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (game == null || health == null || canvas == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик",
                "Откройте сцену SnowballLevel с GameManager, HealthManager и Canvas. Сначала примените этап 5.", "Ок");
            return;
        }

        if (Object.FindFirstObjectByType<HealthTreeUI>() != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Этап 6 уже применён к этой сцене.", "Ок");
            return;
        }

        Sprite square = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        GameObject tree = new GameObject("HealthTree", typeof(RectTransform));
        tree.transform.SetParent(canvas.transform, false);
        RectTransform treeRect = (RectTransform)tree.transform;
        treeRect.anchorMin = new Vector2(0f, 1f);
        treeRect.anchorMax = new Vector2(0f, 1f);
        treeRect.pivot = new Vector2(0f, 1f);
        treeRect.anchoredPosition = new Vector2(40f, -60f);
        treeRect.sizeDelta = new Vector2(160f, 320f);

        Image background = MakeImage("Background", tree.transform, square,
            new Color(1f, 1f, 1f, 0.12f), Vector2.zero, Vector2.one);

        Image fill = MakeImage("Fill", tree.transform, square,
            new Color(0.2f, 0.75f, 0.4f, 0.95f), Vector2.zero, Vector2.one);

        Image star = MakeImage("Star", tree.transform, square,
            new Color(0.4f, 0.4f, 0.45f, 0.6f), new Vector2(0.5f, 1f), Vector2.zero);
        RectTransform starRect = (RectTransform)star.transform;
        starRect.pivot = new Vector2(0.5f, 0f);
        starRect.anchoredPosition = new Vector2(0f, 10f);
        starRect.sizeDelta = new Vector2(70f, 70f);

        HealthTreeUI widget = tree.AddComponent<HealthTreeUI>();
        widget.health = health;
        widget.game = game;
        widget.fill = fill;
        widget.star = star;

        background.raycastTarget = false;
        fill.raycastTarget = false;
        star.raycastTarget = false;

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 6 применён: Ёлка-2 в левом верхнем углу.");
    }

    private static Image MakeImage(string name, Transform parent, Sprite sprite, Color color,
        Vector2 anchor, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;

        if (size == Vector2.one)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
        else
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.sizeDelta = Vector2.zero;
        }

        Image image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        return image;
    }
}
