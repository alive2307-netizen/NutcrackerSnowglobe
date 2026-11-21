using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Этап 16: пересобирает визуал щелкунчика из деталей (сапоги, мундир, пуговицы, пояс, усы, шляпа)
/// и подключает RunBob. Повторный запуск заменяет прежний визуал. Запускается через меню Nutcracker.
/// </summary>
public static class Stage16NutcrackerVisualTool
{
    private const string RootName = "NutcrackerVisual";

    [MenuItem("Nutcracker/Apply Stage 16 (nutcracker visual and run bob)")]
    public static void Apply()
    {
        PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();

        if (player == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Откройте сцену SnowballLevel с Player.", "Ок");
            return;
        }

        Transform old = player.transform.Find(RootName);

        if (old != null)
        {
            Object.DestroyImmediate(old.gameObject);
        }

        MeshRenderer capsule = player.GetComponent<MeshRenderer>();

        if (capsule != null)
        {
            capsule.enabled = false;
        }

        Transform nose = player.transform.Find("Nose");

        if (nose != null)
        {
            nose.GetComponent<MeshRenderer>().enabled = false;
        }

        EnsureFolder("Assets", "Materials");

        Material wood = MakeMaterial("NutcrackerWoodMat", new Color(0.62f, 0.38f, 0.18f));
        Material coat = MakeMaterial("NutcrackerCoatMat", new Color(0.78f, 0.06f, 0.08f));
        Material black = MakeMaterial("NutcrackerBlackMat", new Color(0.05f, 0.05f, 0.05f));
        Material gold = MakeMaterial("NutcrackerGoldMat", new Color(0.95f, 0.75f, 0.2f));
        Material white = MakeMaterial("NutcrackerWhiteMat", new Color(0.95f, 0.95f, 0.92f));

        GameObject root = new GameObject(RootName);
        root.transform.SetParent(player.transform, false);
        root.transform.localPosition = new Vector3(0f, -1.05f, 0f);
        Transform r = root.transform;

        // Сапоги и штаны
        Part(r, PrimitiveType.Cube, "BootLeft", new Vector3(-0.16f, 0.1f, 0.04f), new Vector3(0.24f, 0.2f, 0.36f), black);
        Part(r, PrimitiveType.Cube, "BootRight", new Vector3(0.16f, 0.1f, 0.04f), new Vector3(0.24f, 0.2f, 0.36f), black);
        Part(r, PrimitiveType.Cube, "LegLeft", new Vector3(-0.16f, 0.36f, 0f), new Vector3(0.2f, 0.3f, 0.2f), black);
        Part(r, PrimitiveType.Cube, "LegRight", new Vector3(0.16f, 0.36f, 0f), new Vector3(0.2f, 0.3f, 0.2f), black);

        // Мундир
        Part(r, PrimitiveType.Cube, "Coat", new Vector3(0f, 0.86f, 0f), new Vector3(0.62f, 0.56f, 0.44f), coat);
        Part(r, PrimitiveType.Cube, "Skirt", new Vector3(0f, 0.52f, 0f), new Vector3(0.7f, 0.12f, 0.5f), coat);
        Part(r, PrimitiveType.Cube, "Belt", new Vector3(0f, 0.9f, 0f), new Vector3(0.64f, 0.07f, 0.46f), black);

        // Золотые пуговицы
        Part(r, PrimitiveType.Sphere, "ButtonTop", new Vector3(0f, 1.08f, 0.23f), Vector3.one * 0.08f, gold);
        Part(r, PrimitiveType.Sphere, "ButtonMid", new Vector3(0f, 0.98f, 0.23f), Vector3.one * 0.08f, gold);
        Part(r, PrimitiveType.Sphere, "ButtonLow", new Vector3(0f, 0.88f, 0.23f), Vector3.one * 0.08f, gold);

        // Руки и кисти
        Part(r, PrimitiveType.Cylinder, "ArmLeft", new Vector3(-0.4f, 0.98f, 0f), new Vector3(0.14f, 0.2f, 0.14f), coat);
        Part(r, PrimitiveType.Cylinder, "ArmRight", new Vector3(0.4f, 0.98f, 0f), new Vector3(0.14f, 0.2f, 0.14f), coat);
        Part(r, PrimitiveType.Sphere, "HandLeft", new Vector3(-0.4f, 0.74f, 0f), Vector3.one * 0.14f, black);
        Part(r, PrimitiveType.Sphere, "HandRight", new Vector3(0.4f, 0.74f, 0f), Vector3.one * 0.14f, black);

        // Голова, лицо, щёки, глаза, усы
        Part(r, PrimitiveType.Cube, "Head", new Vector3(0f, 1.42f, 0f), new Vector3(0.44f, 0.44f, 0.44f), wood);
        Part(r, PrimitiveType.Sphere, "CheekLeft", new Vector3(-0.13f, 1.36f, 0.23f), Vector3.one * 0.08f, white);
        Part(r, PrimitiveType.Sphere, "CheekRight", new Vector3(0.13f, 1.36f, 0.23f), Vector3.one * 0.08f, white);
        Part(r, PrimitiveType.Sphere, "EyeLeft", new Vector3(-0.09f, 1.47f, 0.23f), Vector3.one * 0.05f, black);
        Part(r, PrimitiveType.Sphere, "EyeRight", new Vector3(0.09f, 1.47f, 0.23f), Vector3.one * 0.05f, black);
        Part(r, PrimitiveType.Cube, "Mustache", new Vector3(0f, 1.32f, 0.23f), new Vector3(0.26f, 0.05f, 0.04f), black);

        // Шляпа: поля и высокая тулья
        Part(r, PrimitiveType.Cylinder, "HatBrim", new Vector3(0f, 1.64f, 0f), new Vector3(0.36f, 0.03f, 0.36f), black);
        Part(r, PrimitiveType.Cube, "HatCrown", new Vector3(0f, 1.86f, 0f), new Vector3(0.4f, 0.4f, 0.4f), black);

        RunBob bob = player.GetComponent<RunBob>();

        if (bob == null)
        {
            bob = player.gameObject.AddComponent<RunBob>();
        }

        bob.player = player;
        bob.visual = r;

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 16 применён: визуал щелкунчика из деталей.");
    }

    private static void Part(Transform parent, PrimitiveType type, string name, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject part = GameObject.CreatePrimitive(type);
        part.name = name;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = localPosition;
        part.transform.localScale = localScale;
        part.GetComponent<Renderer>().sharedMaterial = material;

        Object.DestroyImmediate(part.GetComponent<Collider>());
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
