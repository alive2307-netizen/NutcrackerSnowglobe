using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Этап 16: создаёт визуал щелкунчика из примитивов под Player и подключает RunBob.
/// Запускается один раз через меню Nutcracker.
/// </summary>
public static class Stage16NutcrackerVisualTool
{
    [MenuItem("Nutcracker/Apply Stage 16 (nutcracker visual and run bob)")]
    public static void Apply()
    {
        PlayerMovement player = Object.FindFirstObjectByType<PlayerMovement>();

        if (player == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Откройте сцену SnowballLevel с Player.", "Ок");
            return;
        }

        if (player.transform.Find("NutcrackerVisual") != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Этап 16 уже применён к этой сцене.", "Ок");
            return;
        }

        EnsureFolder("Assets", "Materials");

        Material wood = MakeMaterial("NutcrackerWoodMat", new Color(0.55f, 0.27f, 0.07f));
        Material coat = MakeMaterial("NutcrackerCoatMat", new Color(0.8f, 0.08f, 0.08f));
        Material black = MakeMaterial("NutcrackerBlackMat", new Color(0.05f, 0.05f, 0.05f));

        GameObject root = new GameObject("NutcrackerVisual");
        root.transform.SetParent(player.transform, false);

        AddPart(root.transform, PrimitiveType.Cylinder, "LeftLeg", new Vector3(-0.15f, 0.3f, 0f), new Vector3(0.2f, 0.3f, 0.2f), black);
        AddPart(root.transform, PrimitiveType.Cylinder, "RightLeg", new Vector3(0.15f, 0.3f, 0f), new Vector3(0.2f, 0.3f, 0.2f), black);
        AddPart(root.transform, PrimitiveType.Cylinder, "Body", new Vector3(0f, 0.95f, 0f), new Vector3(0.7f, 0.35f, 0.7f), coat);
        AddPart(root.transform, PrimitiveType.Cylinder, "LeftArm", new Vector3(-0.42f, 0.85f, 0f), new Vector3(0.24f, 0.35f, 0.24f), coat);
        AddPart(root.transform, PrimitiveType.Cylinder, "RightArm", new Vector3(0.42f, 0.85f, 0f), new Vector3(0.24f, 0.35f, 0.24f), coat);
        AddPart(root.transform, PrimitiveType.Sphere, "Head", new Vector3(0f, 1.5f, 0f), Vector3.one * 0.5f, wood);
        AddPart(root.transform, PrimitiveType.Cube, "Hat", new Vector3(0f, 1.875f, 0f), new Vector3(0.6f, 0.25f, 0.6f), black);

        RunBob bob = player.GetComponent<RunBob>();

        if (bob == null)
        {
            bob = player.gameObject.AddComponent<RunBob>();
        }

        bob.player = player;
        bob.visual = root.transform;

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 16 применён: визуал щелкунчика и покачивание на бегу.");
    }

    private static void AddPart(Transform parent, PrimitiveType type, string name, Vector3 localPosition, Vector3 localScale, Material material)
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
