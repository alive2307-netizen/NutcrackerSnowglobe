using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Этапы 8-9: создаёт префабы Mouse и Snowball, добавляет игроку SnowballShooter
/// и расставляет мышей на трассе. Запускается один раз через меню Nutcracker.
/// </summary>
public static class Stage8To9SceneTool
{
    private const float PlanetRadius = 30f;

    [MenuItem("Nutcracker/Apply Stage 8-9 (mice and snowballs)")]
    public static void Apply()
    {
        GameObject planet = GameObject.Find("SnowballPlanet");
        PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();

        if (planet == null || player == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик",
                "Откройте сцену SnowballLevel с SnowballPlanet и Player. Сначала примените этапы 1-5.", "Ок");
            return;
        }

        if (player.GetComponent<SnowballShooter>() != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Этапы 8-9 уже применены к этой сцене.", "Ок");
            return;
        }

        EnsureFolder("Assets", "Materials");
        EnsureFolder("Assets", "Prefabs");

        GameObject snowball = MakeSnowballPrefab();
        GameObject mouse = MakeMousePrefab();

        SnowballShooter shooter = player.gameObject.AddComponent<SnowballShooter>();
        shooter.snowballPrefab = snowball;

        float[] startAngles = { 20f, 80f, 140f, 200f, 320f };
        GameObject parent = new GameObject("Mice");

        for (int i = 0; i < startAngles.Length; i++)
        {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(mouse, parent.transform);
            MouseEnemy enemy = instance.GetComponent<MouseEnemy>();
            enemy.planet = planet.transform;
            enemy.planetRadius = PlanetRadius;
            enemy.startAngle = startAngles[i];
            enemy.orbitSpeed = i % 2 == 0 ? 18f : 22f;
            instance.name = "Mouse (" + (i + 1) + ")";
        }

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этапы 8-9 применены: мыши на трассе, стрельба по ЛКМ.");
    }

    private static GameObject MakeMousePrefab()
    {
        string path = "Assets/Prefabs/Mouse.prefab";

        GameObject root = new GameObject("Mouse");

        Rigidbody body = root.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;

        SphereCollider trigger = root.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 0.8f;

        GameObject body3d = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body3d.name = "Body";
        body3d.transform.SetParent(root.transform, false);
        body3d.transform.localScale = new Vector3(0.6f, 0.5f, 0.6f);
        body3d.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        body3d.GetComponent<MeshRenderer>().sharedMaterial = MakeMaterial("MouseMat", new Color(0.5f, 0.5f, 0.55f));
        Object.DestroyImmediate(body3d.GetComponent<CapsuleCollider>());

        root.AddComponent<MouseEnemy>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    private static GameObject MakeSnowballPrefab()
    {
        string path = "Assets/Prefabs/Snowball.prefab";

        GameObject root = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        root.name = "Snowball";
        root.transform.localScale = Vector3.one * 0.6f;
        root.GetComponent<MeshRenderer>().sharedMaterial = MakeMaterial("SnowballMat", Color.white);

        Rigidbody body = root.AddComponent<Rigidbody>();
        body.useGravity = false;
        body.collisionDetectionMode = CollisionDetectionMode.Continuous;

        root.AddComponent<SnowballProjectile>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
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
