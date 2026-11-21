using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Этап 20: равномерно раскладывает ёлки, сугробы и катки по всей сфере, кроме полосы трассы,
/// и ставит третью часть гирлянды на трассу. Запускается через меню Nutcracker.
/// </summary>
public static class Stage20PlanetDecorTool
{
    private const float PlanetRadius = 30f;
    private const float TrackHalfWidthSin = 0.17f;
    private const float GoldenAngle = 2.39996323f;

    [MenuItem("Nutcracker/Apply Stage 20 (planet decor and third garland)")]
    public static void Apply()
    {
        if (GameObject.Find("PlanetDecor") != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Этап 20 уже применён к этой сцене.", "Ок");
            return;
        }

        GameObject tree = Load("Tree");
        GameObject drift = Load("Snowdrift");
        GameObject ice = Load("IcePatch");
        GameObject garland = Load("Garland");

        if (tree == null || drift == null || ice == null || garland == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик",
                "Не найдены префабы Tree, Snowdrift, IcePatch, Garland. Сначала примените этапы 3-10.", "Ок");
            return;
        }

        GameObject decor = new GameObject("PlanetDecor");
        Scatter(tree, 60, 0f, 0, decor.transform);
        Scatter(drift, 30, 0.1f, 17, decor.transform);
        Scatter(ice, 16, 0.03f, 31, decor.transform);

        PlaceOnTrack(garland, 255f, -5f, 0.8f, "Garland (3rd)");

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 20 применён: декорации по сфере и третья гирлянда.");
    }

    private static void Scatter(GameObject prefab, int count, float lift, int seed, Transform parent)
    {
        for (int i = 0; i < count; i++)
        {
            float y = 1f - 2f * (i + 0.5f) / count;
            float r = Mathf.Sqrt(1f - y * y);
            float a = (i + seed) * GoldenAngle;
            Vector3 dir = new Vector3(Mathf.Sin(a) * r, y, Mathf.Cos(a) * r);

            if (Mathf.Abs(dir.x) < TrackHalfWidthSin)
            {
                continue;
            }

            Place(prefab, dir, lift, parent, prefab.name + " (planet " + (i + 1) + ")");
        }
    }

    private static void PlaceOnTrack(GameObject prefab, float thetaDegrees, float phiDegrees, float lift, string name)
    {
        float theta = thetaDegrees * Mathf.Deg2Rad;
        float phi = phiDegrees * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Sin(phi), Mathf.Cos(phi) * Mathf.Cos(theta), Mathf.Cos(phi) * Mathf.Sin(theta)).normalized;
        Place(prefab, dir, lift, null, name);
    }

    private static void Place(GameObject prefab, Vector3 direction, float lift, Transform parent, string name)
    {
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
        instance.transform.position = direction.normalized * (PlanetRadius + lift);
        instance.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);
        instance.name = name;
    }

    private static GameObject Load(string name)
    {
        return AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/" + name + ".prefab");
    }
}
