using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Stage35Tool
{
    private const float SlotWidth = 90f;
    private const float WordGap = 50f;
    private const int WordStartIndex = 6;

    [MenuItem("Nutcracker/Apply Stage 35 (phrase letters, second win)")]
    public static void Apply()
    {
        RoundUI ui = Object.FindFirstObjectByType<RoundUI>();
        PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();

        if (ui == null || ui.hudPanel == null || player == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Откройте сцену SnowballLevel с RoundUI и игроком. Сначала примените этапы 5, 21 и 34.", "Ок");
            return;
        }

        if (Object.FindFirstObjectByType<PhraseProgress>() != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Этап 35 уже применён к этой сцене.", "Ок");
            return;
        }

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        int count = PhraseProgress.Letters.Length;
        float width = count * SlotWidth + WordGap;

        GameObject row = new GameObject("PhraseRow", typeof(RectTransform), typeof(PhraseProgress));
        row.transform.SetParent(ui.hudPanel.transform, false);

        RectTransform rect = (RectTransform)row.transform;
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(width, 120f);
        rect.anchoredPosition = new Vector2(0f, -200f);

        Text[] slots = new Text[count];

        for (int i = 0; i < count; i++)
        {
            float x = i * SlotWidth + (i >= WordStartIndex ? WordGap : 0f) - width * 0.5f + SlotWidth * 0.5f;

            GameObject go = new GameObject("Slot" + i, typeof(RectTransform), typeof(Text), typeof(Outline));
            go.transform.SetParent(row.transform, false);

            RectTransform slotRect = (RectTransform)go.transform;
            slotRect.anchorMin = new Vector2(0.5f, 0.5f);
            slotRect.anchorMax = new Vector2(0.5f, 0.5f);
            slotRect.pivot = new Vector2(0.5f, 0.5f);
            slotRect.sizeDelta = new Vector2(SlotWidth, 120f);
            slotRect.anchoredPosition = new Vector2(x, 0f);

            Text text = go.GetComponent<Text>();
            text.font = font;
            text.fontSize = 96;
            text.alignment = TextAnchor.MiddleCenter;
            text.text = "_";
            text.raycastTarget = false;
            slots[i] = text;

            Outline outline = go.GetComponent<Outline>();
            outline.effectColor = new Color(0.05f, 0.1f, 0.2f, 0.9f);
            outline.effectDistance = new Vector2(3f, -3f);
        }

        PhraseProgress progress = row.GetComponent<PhraseProgress>();
        progress.slots = slots;

        GameObject spawnerObject = new GameObject("Letters");
        LetterSpawner spawner = spawnerObject.AddComponent<LetterSpawner>();
        spawner.planet = player.GetComponent<PlanetGravity>().planet;
        spawner.phrase = progress;

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 35 применён: буквы фразы на уровне, второе условие победы, строка фразы в центре экрана.");
    }
}
