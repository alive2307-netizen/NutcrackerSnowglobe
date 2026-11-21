using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Этап 17: заменяет примитивы объектов трассы на узнаваемые визуалы в префабах.
/// Коллайдеры и скрипты остаются на корне префабов. Запускается через меню Nutcracker.
/// </summary>
public static class Stage17VisualTool
{
    private const string PrefabDir = "Assets/Prefabs/";
    private const string ConeMeshPath = "Assets/Meshes/ConeMesh.asset";

    private static readonly string[] PrefabNames =
    {
        "Tree", "Snowdrift", "IcePatch", "Mouse", "Toy", "Tinsel", "Garland", "SnowballPickup"
    };

    private static Mesh cone;

    [MenuItem("Nutcracker/Apply Stage 17 (object visuals)")]
    public static void Apply()
    {
        List<string> missing = new List<string>();

        foreach (string name in PrefabNames)
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(PrefabDir + name + ".prefab") == null)
            {
                missing.Add(name);
            }
        }

        if (missing.Count > 0)
        {
            EditorUtility.DisplayDialog("Щелкунчик",
                "Не найдены префабы: " + string.Join(", ", missing) + ". Сначала примените этапы 3-10.", "Ок");
            return;
        }

        GameObject tree = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabDir + "Tree.prefab");

        if (tree.transform.Find("Visual") != null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Этап 17 уже применён.", "Ок");
            return;
        }

        EnsureFolder("Assets", "Materials");
        EnsureFolder("Assets", "Meshes");
        cone = LoadOrCreateCone();

        Edit("Tree", BuildTree);
        Edit("Snowdrift", BuildSnowdrift);
        Edit("IcePatch", BuildIce);
        Edit("Mouse", BuildMouse);
        Edit("Toy", BuildToy);
        Edit("Tinsel", BuildTinsel);
        Edit("Garland", BuildGarland);
        Edit("SnowballPickup", BuildSnowballPickup);

        AssetDatabase.SaveAssets();
        Debug.Log("[Щелкунчик] Этап 17 применён: визуалы объектов трассы обновлены.");
    }

    private static void Edit(string name, System.Action<GameObject> build)
    {
        string path = PrefabDir + name + ".prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(path);
        build(root);
        PrefabUtility.SaveAsPrefabAsset(root, path);
        PrefabUtility.UnloadPrefabContents(root);
    }

    private static void BuildTree(GameObject root)
    {
        foreach (string old in new[] { "Trunk", "CrownLow", "CrownMid", "CrownTop" })
        {
            Transform t = root.transform.Find(old);

            if (t != null)
            {
                UnityEngine.Object.DestroyImmediate(t.gameObject);
            }
        }

        Transform v = Visual(root).transform;
        Material bark = Mat("PropBark", new Color(0.4f, 0.24f, 0.1f));
        Material pine = Mat("PropPine", new Color(0.1f, 0.42f, 0.18f));
        Material snow = Mat("PropSnowCap", new Color(0.97f, 0.98f, 1f), 0.6f);
        Material star = Mat("PropStar", new Color(1f, 0.8f, 0.2f), 0.6f, new Color(0.6f, 0.45f, 0.1f));

        Prim(v, PrimitiveType.Cylinder, new Vector3(0f, 0.5f, 0f), new Vector3(0.25f, 0.5f, 0.25f), bark);

        Tier(v, 0.9f, 1.5f, 1.1f, pine, snow);
        Tier(v, 1.8f, 1.3f, 0.85f, pine, snow);
        Tier(v, 2.6f, 1.1f, 0.55f, pine, snow);

        Prim(v, PrimitiveType.Sphere, new Vector3(0f, 3.85f, 0f), Vector3.one * 0.3f, star);
    }

    private static void Tier(Transform v, float baseY, float height, float radius, Material pine, Material snow)
    {
        Cone(v, new Vector3(0f, baseY, 0f), radius, height, pine);
        Cone(v, new Vector3(0f, baseY + 0.6f * height - 0.02f, 0f), 0.4f * radius * 1.04f, 0.4f * height + 0.02f, snow);
    }

    private static void BuildSnowdrift(GameObject root)
    {
        HideRootRenderer(root);
        Transform v = Visual(root).transform;
        Material snow = Mat("PropSnow", new Color(0.95f, 0.97f, 1f), 0.4f);

        Prim(v, PrimitiveType.Sphere, Vector3.zero, new Vector3(3f, 2f, 3f), snow);
        Prim(v, PrimitiveType.Sphere, new Vector3(0.9f, 0.65f, 0.4f), Vector3.one * 0.4f, snow);
        Prim(v, PrimitiveType.Sphere, new Vector3(0.2f, 0.85f, -0.6f), Vector3.one * 0.4f, snow);
        Prim(v, PrimitiveType.Sphere, new Vector3(-0.6f, 0.7f, -0.7f), Vector3.one * 0.4f, snow);
    }

    private static void BuildIce(GameObject root)
    {
        HideRootRenderer(root);
        Transform v = Visual(root).transform;
        Material ice = Mat("PropIce", new Color(0.6f, 0.85f, 1f), 0.9f);
        Material crack = Mat("PropIceCrack", new Color(1f, 1f, 1f), 0.9f);

        Prim(v, PrimitiveType.Cylinder, new Vector3(0f, 0.025f, 0f), new Vector3(3.5f, 0.025f, 3.5f), ice);
        Prim(v, PrimitiveType.Cube, new Vector3(0.5f, 0.052f, 0.3f), new Vector3(1.6f, 0.005f, 0.03f), crack, new Vector3(0f, 30f, 0f));
        Prim(v, PrimitiveType.Cube, new Vector3(-0.8f, 0.052f, -0.4f), new Vector3(1.4f, 0.005f, 0.03f), crack, new Vector3(0f, -40f, 0f));
        Prim(v, PrimitiveType.Cube, new Vector3(0.2f, 0.052f, -1.1f), new Vector3(1.2f, 0.005f, 0.03f), crack, new Vector3(0f, 80f, 0f));
    }

    private static void BuildMouse(GameObject root)
    {
        Transform old = root.transform.Find("Body");

        if (old != null)
        {
            UnityEngine.Object.DestroyImmediate(old.gameObject);
        }

        Transform v = Visual(root).transform;
        Material fur = Mat("PropMouseFur", new Color(0.55f, 0.55f, 0.6f), 0.3f);
        Material dark = Mat("PropMouseDark", new Color(0.2f, 0.2f, 0.22f));
        Material pink = Mat("PropMousePink", new Color(0.95f, 0.6f, 0.65f));
        Material black = Mat("PropBlack", new Color(0.05f, 0.05f, 0.05f));

        Prim(v, PrimitiveType.Sphere, new Vector3(0f, 0.3f, 0f), new Vector3(0.6f, 0.5f, 0.8f), fur);
        Prim(v, PrimitiveType.Sphere, new Vector3(0f, 0.4f, 0.45f), new Vector3(0.45f, 0.42f, 0.42f), fur);
        Prim(v, PrimitiveType.Sphere, new Vector3(0f, 0.37f, 0.68f), Vector3.one * 0.12f, pink);
        Prim(v, PrimitiveType.Sphere, new Vector3(-0.18f, 0.6f, 0.42f), new Vector3(0.26f, 0.26f, 0.1f), fur);
        Prim(v, PrimitiveType.Sphere, new Vector3(0.18f, 0.6f, 0.42f), new Vector3(0.26f, 0.26f, 0.1f), fur);
        Prim(v, PrimitiveType.Sphere, new Vector3(-0.18f, 0.6f, 0.44f), new Vector3(0.16f, 0.16f, 0.06f), pink);
        Prim(v, PrimitiveType.Sphere, new Vector3(0.18f, 0.6f, 0.44f), new Vector3(0.16f, 0.16f, 0.06f), pink);
        Prim(v, PrimitiveType.Sphere, new Vector3(-0.1f, 0.48f, 0.64f), Vector3.one * 0.06f, black);
        Prim(v, PrimitiveType.Sphere, new Vector3(0.1f, 0.48f, 0.64f), Vector3.one * 0.06f, black);

        foreach (Vector3 leg in new[] { new Vector3(-0.2f, 0.1f, 0.22f), new Vector3(0.2f, 0.1f, 0.22f), new Vector3(-0.2f, 0.1f, -0.22f), new Vector3(0.2f, 0.1f, -0.22f) })
        {
            Prim(v, PrimitiveType.Cylinder, leg, Vector3.one * 0.1f, dark);
        }

        Prim(v, PrimitiveType.Cylinder, new Vector3(0f, 0.15f, -0.6f), new Vector3(0.03f, 0.3f, 0.03f), dark, new Vector3(90f, 0f, 0f));

        AddBob(root, v, 0.03f, 2.5f, 0f, 6f, false);
    }

    private static void BuildToy(GameObject root)
    {
        HideRootRenderer(root);
        Transform v = Visual(root).transform;
        Material red = Mat("PropToyRed", new Color(0.8f, 0.08f, 0.1f), 0.9f, new Color(0.35f, 0.02f, 0.02f));
        Material gold = Mat("PropGold", new Color(1f, 0.78f, 0.25f), 0.9f, new Color(0.25f, 0.18f, 0.02f));

        Prim(v, PrimitiveType.Sphere, Vector3.zero, Vector3.one * 0.8f, red);
        Prim(v, PrimitiveType.Cylinder, new Vector3(0f, 0.44f, 0f), new Vector3(0.16f, 0.08f, 0.16f), gold);

        AddBob(root, v, 0.05f, 1f, 40f, 0f, true);
    }

    private static void BuildTinsel(GameObject root)
    {
        HideRootRenderer(root);
        Transform v = Visual(root).transform;
        Material body = Mat("PropTinsel", new Color(1f, 0.85f, 0.3f), 0.9f, new Color(0.5f, 0.35f, 0.05f));
        Material band = Mat("PropTinselBand", new Color(1f, 1f, 0.85f), 0.9f, new Color(0.6f, 0.6f, 0.3f));

        Prim(v, PrimitiveType.Sphere, Vector3.zero, Vector3.one * 0.8f, body);
        Prim(v, PrimitiveType.Cylinder, Vector3.zero, new Vector3(0.84f, 0.01f, 0.84f), band);
        Prim(v, PrimitiveType.Cylinder, Vector3.zero, new Vector3(0.84f, 0.01f, 0.84f), band, new Vector3(60f, 0f, 0f));
        Prim(v, PrimitiveType.Cylinder, Vector3.zero, new Vector3(0.84f, 0.01f, 0.84f), band, new Vector3(0f, 0f, 60f));

        AddBob(root, v, 0.05f, 1f, 40f, 0f, true);
    }

    private static void BuildGarland(GameObject root)
    {
        HideRootRenderer(root);
        Transform v = Visual(root).transform;
        Material green = Mat("PropGarlandGreen", new Color(0.1f, 0.4f, 0.15f), 0.5f);
        Material[] bulbs =
        {
            Mat("PropBulbRed", new Color(1f, 0.1f, 0.1f), 0.9f, new Color(0.9f, 0.1f, 0.1f)),
            Mat("PropBulbYellow", new Color(1f, 0.9f, 0.2f), 0.9f, new Color(0.9f, 0.8f, 0.1f)),
            Mat("PropBulbBlue", new Color(0.2f, 0.5f, 1f), 0.9f, new Color(0.1f, 0.3f, 0.9f)),
            Mat("PropBulbWhite", new Color(1f, 1f, 1f), 0.9f, new Color(0.9f, 0.9f, 0.9f))
        };

        Prim(v, PrimitiveType.Sphere, Vector3.zero, Vector3.one * 0.8f, green);

        Vector3[] dirs =
        {
            new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(-1f, 0f, 0f), new Vector3(0f, 0f, -1f),
            new Vector3(0.7f, 0.7f, 0f), new Vector3(0f, 0.7f, 0.7f), new Vector3(-0.7f, 0.7f, 0f), new Vector3(0f, 0.7f, -0.7f)
        };

        for (int i = 0; i < dirs.Length; i++)
        {
            Prim(v, PrimitiveType.Sphere, dirs[i].normalized * 0.42f, Vector3.one * 0.12f, bulbs[i % bulbs.Length]);
        }

        AddBob(root, v, 0.04f, 1f, 30f, 0f, true);
    }

    private static void BuildSnowballPickup(GameObject root)
    {
        HideRootRenderer(root);
        Transform v = Visual(root).transform;
        Material white = Mat("PropSnowPickup", new Color(0.95f, 0.97f, 1f), 0.5f, new Color(0.25f, 0.3f, 0.4f));
        Material ring = Mat("PropPickupRing", new Color(0.4f, 0.8f, 1f), 0.9f, new Color(0.2f, 0.6f, 1f));
        Material sparkle = Mat("PropSparkle", new Color(1f, 1f, 1f), 0.9f, new Color(0.9f, 0.9f, 0.9f));

        Prim(v, PrimitiveType.Sphere, Vector3.zero, Vector3.one * 0.8f, white);
        Prim(v, PrimitiveType.Cylinder, Vector3.zero, new Vector3(1f, 0.01f, 1f), ring);

        Vector3[] dirs =
        {
            new Vector3(0.7f, 0.7f, 0f), new Vector3(-0.7f, 0.7f, 0f), new Vector3(0f, 0.7f, 0.7f), new Vector3(0f, -0.7f, 0.7f)
        };

        foreach (Vector3 d in dirs)
        {
            Prim(v, PrimitiveType.Sphere, d.normalized * 0.5f, Vector3.one * 0.07f, sparkle);
        }

        AddBob(root, v, 0.04f, 0.8f, 25f, 0f, true);
    }

    private static void AddBob(GameObject root, Transform visual, float height, float speed, float spin, float wobble, bool pulse)
    {
        ObjectBob bob = root.AddComponent<ObjectBob>();
        bob.visual = visual;
        bob.bobHeight = height;
        bob.bobSpeed = speed;
        bob.spinSpeed = spin;
        bob.wobbleAngle = wobble;
        bob.pulseGlow = pulse;
    }

    private static GameObject Visual(GameObject root)
    {
        Vector3 s = root.transform.localScale;
        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(root.transform, false);
        visual.transform.localScale = new Vector3(1f / s.x, 1f / s.y, 1f / s.z);
        return visual;
    }

    private static void HideRootRenderer(GameObject root)
    {
        MeshRenderer mr = root.GetComponent<MeshRenderer>();

        if (mr != null)
        {
            mr.enabled = false;
        }
    }

    private static void Prim(Transform parent, PrimitiveType type, Vector3 position, Vector3 scale, Material mat, Vector3 euler = default)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = type.ToString();
        go.transform.SetParent(parent, false);
        go.transform.localPosition = position;
        go.transform.localEulerAngles = euler;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
    }

    private static void Cone(Transform parent, Vector3 basePosition, float radius, float height, Material mat)
    {
        GameObject go = new GameObject("Cone");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = basePosition;
        go.transform.localScale = new Vector3(radius, height, radius);
        go.AddComponent<MeshFilter>().sharedMesh = cone;
        go.AddComponent<MeshRenderer>().sharedMaterial = mat;
    }

    private static Mesh LoadOrCreateCone()
    {
        Mesh existing = AssetDatabase.LoadAssetAtPath<Mesh>(ConeMeshPath);

        if (existing != null)
        {
            return existing;
        }

        const int segments = 24;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        Vector3 apex = new Vector3(0f, 1f, 0f);
        Vector3 center = Vector3.zero;

        for (int i = 0; i < segments; i++)
        {
            float a0 = 2f * Mathf.PI * i / segments;
            float a1 = 2f * Mathf.PI * (i + 1) / segments;
            Vector3 b0 = new Vector3(Mathf.Cos(a0), 0f, Mathf.Sin(a0));
            Vector3 b1 = new Vector3(Mathf.Cos(a1), 0f, Mathf.Sin(a1));
            Vector3 mid = (b0 + b1) * 0.5f;

            AddTriangle(vertices, triangles, b0, apex, b1, new Vector3(mid.x, 0f, mid.z));
            AddTriangle(vertices, triangles, center, b0, b1, Vector3.down);
        }

        Mesh mesh = new Mesh { name = "ConeMesh" };
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        AssetDatabase.CreateAsset(mesh, ConeMeshPath);
        return mesh;
    }

    private static void AddTriangle(List<Vector3> vertices, List<int> triangles, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 outward)
    {
        Vector3 n = Vector3.Cross(p1 - p0, p2 - p0);

        if (Vector3.Dot(n, outward) < 0f)
        {
            Vector3 swap = p1;
            p1 = p2;
            p2 = swap;
        }

        int index = vertices.Count;
        vertices.Add(p0);
        vertices.Add(p1);
        vertices.Add(p2);
        triangles.Add(index);
        triangles.Add(index + 1);
        triangles.Add(index + 2);
    }

    private static Material Mat(string name, Color color, float smoothness = 0.3f, Color glow = default)
    {
        string path = "Assets/Materials/" + name + ".mat";
        Material m = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (m == null)
        {
            m = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(m, path);
        }

        m.color = color;
        m.SetFloat("_Glossiness", smoothness);

        if (glow.maxColorComponent > 0f)
        {
            m.EnableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", glow);
        }
        else
        {
            m.DisableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", Color.black);
        }

        EditorUtility.SetDirty(m);
        return m;
    }

    private static void EnsureFolder(string parent, string name)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + name))
        {
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
