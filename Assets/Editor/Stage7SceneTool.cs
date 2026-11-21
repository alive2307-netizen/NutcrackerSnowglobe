using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Создаёт префабы Toy, Tinsel, Garland и расставляет их по шару в сцене SnowballLevel.
/// Запускается один раз через меню Nutcracker.
/// </summary>
public static class Stage7SceneTool
{
    private const float PlanetRadius = 30f;
    private const float PickupLift = 1f;

    private struct Placement
    {
        public GameObject prefab;
        public float theta;
        public float phi;

        public Placement(GameObject prefab, float theta, float phi)
        {
            this.prefab = prefab;
            this.theta = theta;
            this.phi = phi;
        }
    }

    [MenuItem("Nutcracker/Apply Stage 7 (pickups)")]
    public static void Apply()
    {
        if (Object.FindFirstObjectByType<ToyPickup>() != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Этап 7 уже применён к этой сцене.", "Ок");
            return;
        }

        EnsureFolder("Assets", "Materials");
        EnsureFolder("Assets", "Prefabs");

        GameObject toy = MakePrefab("Toy", new Color(1f, 0.25f, 0.25f), typeof(ToyPickup));
        GameObject tinsel = MakePrefab("Tinsel", new Color(1f, 0.82f, 0.25f), typeof(TinselPickup));
        GameObject garland = MakePrefab("Garland", new Color(0.4f, 1f, 0.5f), typeof(GarlandPickup));

        Placement[] placements =
        {
            new Placement(toy, 15f, 0f),
            new Placement(toy, 30f, -8f),
            new Placement(tinsel, 62f, 0f),
            new Placement(toy, 55f, 6f),
            new Placement(toy, 70f, -6f),
            new Placement(toy, 82f, 8f),
            new Placement(garland, 125f, 6f),
            new Placement(toy, 120f, -6f),
            new Placement(toy, 160f, 6f),
            new Placement(toy, 185f, 0f),
            new Placement(garland, 205f, -2f),
            new Placement(toy, 200f, 10f),
            new Placement(toy, 230f, -10f),
            new Placement(tinsel, 240f, 0f),
            new Placement(toy, 265f, 6f),
            new Placement(toy, 280f, -8f),
            new Placement(toy, 310f, -4f),
            new Placement(toy, 345f, 4f)
        };

        GameObject parent = new GameObject("Pickups");

        foreach (Placement p in placements)
        {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(p.prefab, parent.transform);
            Vector3 direction = SurfaceDirection(p.theta, p.phi);

            instance.transform.position = direction * (PlanetRadius + PickupLift);
            instance.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
        }

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 7 применён: игрушки, мишура и гирлянды расставлены.");
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

    private static GameObject MakePrefab(string name, Color color, System.Type pickupType)
    {
        string path = "Assets/Prefabs/" + name + ".prefab";

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = name;
        go.transform.localScale = Vector3.one * 0.8f;
        go.GetComponent<MeshRenderer>().sharedMaterial = MakeMaterial(name + "Mat", color);

        SphereCollider trigger = go.GetComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 1.25f;

        go.AddComponent(pickupType);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
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
