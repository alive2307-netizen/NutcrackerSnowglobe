using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Этап 31: зимние звуки победы и поражения, щелкунчик опущен на поверхность без подъёма ног на беге.
/// Требует этап 16 и импорт win_winter.wav и lose_winter.wav. Запускается через меню Nutcracker.
/// </summary>
public static class Stage31Tool
{
    private const string WinPath = "Assets/Audio/Sfx/win_winter.wav";
    private const string LosePath = "Assets/Audio/Sfx/lose_winter.wav";
    private const float FeetOffset = -1.05f;

    [MenuItem("Nutcracker/Apply Stage 31 (winter win and lose sounds, nutcracker on ground)")]
    public static void Apply()
    {
        AudioManager audio = Object.FindFirstObjectByType<AudioManager>();
        PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();

        if (audio == null || player == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Откройте сцену SnowballLevel. Сначала примените этапы 13 и 16.", "Ок");
            return;
        }

        AudioClip win = AssetDatabase.LoadAssetAtPath<AudioClip>(WinPath);
        AudioClip lose = AssetDatabase.LoadAssetAtPath<AudioClip>(LosePath);

        if (win == null || lose == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Не найдены win_winter.wav и lose_winter.wav в Assets/Audio/Sfx. Дождитесь импорта и повторите.", "Ок");
            return;
        }

        Transform visual = player.transform.Find("NutcrackerVisual");

        if (visual == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Не найден NutcrackerVisual под Player. Сначала примените этап 16.", "Ок");
            return;
        }

        audio.winSound = win;
        audio.loseSound = lose;
        EditorUtility.SetDirty(audio);

        visual.localPosition = new Vector3(0f, FeetOffset, 0f);

        RunBob bob = player.GetComponent<RunBob>();

        if (bob != null)
        {
            bob.bobHeight = 0f;
            EditorUtility.SetDirty(bob);
        }

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 31 применён: зимние звуки победы и поражения, щелкунчик на поверхности.");
    }
}
