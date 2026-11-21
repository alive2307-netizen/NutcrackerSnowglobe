using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Этап 10: создаёт префаб SnowballPickup, раскладывает подбираемые снежки по трассе
/// и добавляет в правый верхний угол счётчик снежков. Запускается один раз через меню Nutcracker.
/// </summary>
public static class Stage10SceneTool
{
    private const float PlanetRadius = 30f;
    private const float PickupLift = 1f;

    private static readonly Vector2[] Placements =
    {
        new Vector2(25f, 8f),
        new Vector2(90f, -8f),
        new Vector2(112f, -10f),
        new Vector2(175f, -8f),
        new Vector2(195f, 8f),
        new Vector2(235f, 6f),
        new Vector2(275f, -6f),
        new Vector2(320f, -12f)
    };

    [MenuItem("Nutcracker/Apply Stage 10 (snowball pickups and ammo counter)")]
    public static void Apply()
    {
        GameObject planet = GameObject.Find("SnowballPlanet");
        SnowballShooter shooter = Object.FindFirstObjectByType<SnowballShooter>();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (planet == null || shooter == null || canvas == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик",
                "Откройте сцену SnowballLevel. Сначала примените этапы 1-9.", "Ок");
            return;
        }

        if (Object.FindFirstObjectByType<AmmoWidget>() != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Этап 10 уже применён к этой сцене.", "Ок");
            return;
        }

        EnsureFolder("Assets", "Materials");
        EnsureFolder("Assets", "Prefabs");

        GameObject pickup = MakePickupPrefab();

        GameObject parent = new GameObject("SnowballPickups");

        foreach (Vector2 placement in Placements)
        {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(pickup, parent.transform);
            Vector3 direction = SurfaceDirection(placement.x, placement.y);

            instance.transform.position = direction * (PlanetRadius + PickupLift);
            instance.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
        }

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject label = new GameObject("AmmoText", typeof(RectTransform), typeof(Text));
        label.transform.SetParent(canvas.transform, false);

        RectTransform rect = (RectTransform)label.transform;
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-40f, -60f);
        rect.sizeDelta = new Vector2(500f, 70f);

        Text text = label.GetComponent<Text>();
        text.font = font;
        text.text = "";
        text.fontSize = 44;
        text.alignment = TextAnchor.UpperRight;
        text.color = new Color(0.7f, 0.9f, 1f);
        text.raycastTarget = false;

        AmmoWidget widget = label.AddComponent<AmmoWidget>();
        widget.shooter = shooter;
        widget.label = text;

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 10 применён: снежки на трассе и счётчик в углу.");
    }

    private static GameObject MakePickupPrefab()
    {
        string path = "Assets/Prefabs/SnowballPickup.prefab";

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "SnowballPickup";
        go.transform.localScale = Vector3.one * 0.8f;
        go.GetComponent<MeshRenderer>().sharedMaterial = MakeMaterial("SnowballPickupMat", new Color(0.85f, 0.95f, 1f));

        SphereCollider trigger = go.GetComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 1.25f;

        go.AddComponent<SnowballPickup>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
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

    private static Material MakeMaterial(string name, Color color)
    {
        string path = "Assets/Materials/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (material == null)
        {
            material = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(material, path);
        }

        material.color = color;
        EditorUtility.SetDirty(material);
        return material;
    }

    private static void EnsureFolder(string parent, string name)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + name))
        {
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
