using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Stage39Tool
{
    private const float RespawnDelay = 90f;

    [MenuItem("Nutcracker/Apply Stage 39 (respawn toys and mice)")]
    public static void Apply()
    {
        MouseRespawner mice = Object.FindFirstObjectByType<MouseRespawner>(FindObjectsInactive.Include);

        if (mice == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Не найден MouseRespawner в сцене. Откройте SnowballLevel.", "Ок");
            return;
        }

        mice.respawnDelay = RespawnDelay;
        EditorUtility.SetDirty(mice);

        int toys = 0;
        foreach (ToyPickup toy in Object.FindObjectsByType<ToyPickup>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            toy.respawnDelay = RespawnDelay;
            EditorUtility.SetDirty(toy);
            toys++;
        }

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 39 применён: респаун игрушек и крыс через " + RespawnDelay + " с, игрушек: " + toys + ".");
    }
}
