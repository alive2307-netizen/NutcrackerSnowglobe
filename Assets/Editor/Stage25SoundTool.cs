using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Этап 25: подключает звуки укуса мыши, удара о сугроб и скольжения по катку.
/// Запускается через меню Nutcracker после импорта файлов из Assets/Audio/Sfx.
/// </summary>
public static class Stage25SoundTool
{
    private const string Dir = "Assets/Audio/Sfx/";

    [MenuItem("Nutcracker/Apply Stage 25 (bite, snow hit, skating sounds)")]
    public static void Apply()
    {
        AudioManager audio = Object.FindFirstObjectByType<AudioManager>();

        if (audio == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Откройте сцену SnowballLevel с AudioManager. Сначала примените этап 13.", "Ок");
            return;
        }

        AudioClip bite = AssetDatabase.LoadAssetAtPath<AudioClip>(Dir + "bite.wav");
        AudioClip snow = AssetDatabase.LoadAssetAtPath<AudioClip>(Dir + "snow_hit.wav");
        AudioClip skate = AssetDatabase.LoadAssetAtPath<AudioClip>(Dir + "skate.wav");

        if (bite == null || snow == null || skate == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик",
                "Не найдены bite.wav, snow_hit.wav, skate.wav в Assets/Audio/Sfx. Дождитесь импорта и повторите.", "Ок");
            return;
        }

        audio.biteSound = bite;
        audio.snowHitSound = snow;
        audio.skateClip = skate;
        EditorUtility.SetDirty(audio);

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 25 применён: укус мыши, удар о сугроб, скольжение по катку.");
    }
}
