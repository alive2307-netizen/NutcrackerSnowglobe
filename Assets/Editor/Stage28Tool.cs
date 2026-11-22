using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Этап 28: удаляет старую подложку ёлки (HealthTree из этапа 6), оставшуюся рядом с новым индикатором.
/// Запускается через меню Nutcracker.
/// </summary>
public static class Stage28Tool
{
    [MenuItem("Nutcracker/Apply Stage 28 (remove old tree HUD)")]
    public static void Apply()
    {
        GameObject old = GameObject.Find("HealthTree");

        if (old == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Старая подложка HealthTree уже удалена или не найдена.", "Ок");
            return;
        }

        Object.DestroyImmediate(old);

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 28 применён: старая подложка ёлки удалена.");
    }
}
