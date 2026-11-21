using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Добавляет в трассу дополнительные ёлки, сугробы и катки в отдельной папке ExtraObstacles.
/// Запускается один раз через меню Nutcracker, нужны префабы Tree, Snowdrift, IcePatch.
/// </summary>
public static class Stage15SceneTool
{
    private const float PlanetRadius = 30f;

    private static readonly Vector2[] TreePlacements =
    {
        new Vector2(62f, -12f), new Vector2(118f, 12f), new Vector2(176f, -6f), new Vector2(196f, -16f),
        new Vector2(238f, 8f), new Vector2(262f, -16f), new Vector2(300f, 14f), new Vector2(322f, -4f)
    };

    private static readonly Vector2[] DriftPlacements =
    {
        new Vector2(25f, 10f), new Vector2(120f, -12f), new Vector2(190f, 12f),
        new Vector2(250f, -14f), new Vector2(312f, -6f)
    };

    private static readonly Vector2[] IcePlacements =
    {
        new Vector2(55f, 4f), new Vector2(165f, 4f), new Vector2(285f, -2f), new Vector2(340f, -2f)
    };

    [MenuItem("Nutcracker/Apply Stage 15 (more trees, drifts and ice)")]
    public static void Apply()
    {
        if (GameObject.Find("ExtraObstacles") != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Этап 15 уже применён к этой сцене.", "Ок");
            return;
        }

        GameObject tree = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tree.prefab");
        GameObject drift = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Snowdrift.prefab");
        GameObject ice = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/IcePatch.prefab");

        if (tree == null || drift == null || ice == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик",
                "Не найдены префабы Tree, Snowdrift, IcePatch. Сначала примените этапы 3-4.", "Ок");
            return;
        }

        GameObject parent = new GameObject("ExtraObstacles");
        Place(tree, TreePlacements, 0f, parent.transform, "Tree");
        Place(drift, DriftPlacements, 0.1f, parent.transform, "Snowdrift");
        Place(ice, IcePlacements, 0.03f, parent.transform, "IcePatch");

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 15 применён: добавлены ёлки, сугробы и катки.");
    }

    private static void Place(GameObject prefab, Vector2[] placements, float lift, Transform parent, string baseName)
    {
        for (int i = 0; i < placements.Length; i++)
        {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            Vector3 direction = SurfaceDirection(placements[i].x, placements[i].y);

            instance.transform.position = direction * (PlanetRadius + lift);
            instance.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
            instance.name = baseName + " (extra " + (i + 1) + ")";
        }
    }

    private static Vector3 SurfaceDirection(float thetaDegrees, float phiDegrees)
    {
        float theta = thetaDegrees * Mathf.Deg2Rad;
        float phi = phiDegrees * Mathf.Deg2Rad;

        return new Vector3(
            Mathf.Sin(phi),
            Mathf.Cos(phi) * Mathf.Cos(theta),
            Mathf.Cos(phi) * Mathf.Sin(theta)).normalized;
    }
}
