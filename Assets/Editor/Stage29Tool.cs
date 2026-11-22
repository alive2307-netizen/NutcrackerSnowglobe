using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Этап 29: добавляет падающий снег на камеру. Снег идёт во время отсчёта, раунда и паузы после столкновения.
/// Запускается через меню Nutcracker.
/// </summary>
public static class Stage29Tool
{
    [MenuItem("Nutcracker/Apply Stage 29 (falling snow during play)")]
    public static void Apply()
    {
        Camera cam = Object.FindFirstObjectByType<Camera>();

        if (cam == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Откройте сцену SnowballLevel с камерой.", "Ок");
            return;
        }

        if (cam.GetComponent<SnowFall>() != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Этап 29 уже применён к этой сцене.", "Ок");
            return;
        }

        cam.gameObject.AddComponent<SnowFall>();

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 29 применён: падающий снег во время игры.");
    }
}
