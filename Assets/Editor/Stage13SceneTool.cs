using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Этап 13: создаёт AudioManager с музыкой и звуками, добавляет RoundDirector на Canvas.
/// Файлы из Assets/Audio должны быть импортированы Unity (они появятся сами после распаковки).
/// Запускается один раз через меню Nutcracker.
/// </summary>
public static class Stage13SceneTool
{
    private const string MusicDir = "Assets/Audio/Music/";
    private const string SfxDir = "Assets/Audio/Sfx/";

    [MenuItem("Nutcracker/Apply Stage 13 (audio and finale)")]
    public static void Apply()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        GameManager game = Object.FindFirstObjectByType<GameManager>();

        if (canvas == null || game == null || Camera.main == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик",
                "Откройте сцену SnowballLevel с Canvas, GameManager и Main Camera.", "Ок");
            return;
        }

        if (Object.FindFirstObjectByType<AudioManager>() != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Этап 13 уже применён к этой сцене.", "Ок");
            return;
        }

        AudioManager audio = new GameObject("AudioManager").AddComponent<AudioManager>();
        audio.menuMusic = LoadClip(MusicDir + "menu.ogg");
        audio.roundMusic = LoadClip(MusicDir + "round.ogg");
        audio.winMusic = LoadClip(MusicDir + "win.ogg");
        audio.loseMusic = LoadClip(MusicDir + "lose.ogg");
        audio.clickSound = LoadClip(SfxDir + "click.wav");
        audio.pickupSound = LoadClip(SfxDir + "pickup.wav");
        audio.shotSound = LoadClip(SfxDir + "shot.wav");
        audio.splatSound = LoadClip(SfxDir + "splat.wav");
        audio.hurtSound = LoadClip(SfxDir + "hurt.wav");
        audio.winSound = LoadClip(SfxDir + "win.wav");
        audio.loseSound = LoadClip(SfxDir + "lose.wav");

        RoundDirector director = canvas.gameObject.AddComponent<RoundDirector>();
        director.game = game;
        director.canvasRect = (RectTransform)canvas.transform;
        director.cameraTransform = Camera.main.transform;
        director.mouseSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 13 применён: музыка, звуки, конфетти и мыши на экране.");
    }

    private static AudioClip LoadClip(string path)
    {
        AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);

        if (clip == null)
        {
            Debug.LogWarning("[Щелкунчик] Не найден клип " + path + ". Импортируйте файлы из Assets/Audio.");
        }

        return clip;
    }
}
