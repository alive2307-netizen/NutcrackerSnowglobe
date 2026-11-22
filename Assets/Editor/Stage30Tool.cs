using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Этап 30: звук грызения мышами, который звучит на паузе после кражи игрушек до экрана поражения.
/// Запускается через меню Nutcracker после импорта gnaw.wav.
/// </summary>
public static class Stage30Tool
{
    private const string GnawPath = "Assets/Audio/Sfx/gnaw.wav";

    [MenuItem("Nutcracker/Apply Stage 30 (mice gnawing on theft defeat)")]
    public static void Apply()
    {
        AudioManager audio = Object.FindFirstObjectByType<AudioManager>();

        if (audio == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Откройте сцену SnowballLevel с AudioManager.", "Ок");
            return;
        }

        AudioClip gnaw = AssetDatabase.LoadAssetAtPath<AudioClip>(GnawPath);

        if (gnaw == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Не найден gnaw.wav в Assets/Audio/Sfx. Дождитесь импорта и повторите.", "Ок");
            return;
        }

        audio.gnawClip = gnaw;
        EditorUtility.SetDirty(audio);

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 30 применён: звук грызения при краже игрушек.");
    }
}
