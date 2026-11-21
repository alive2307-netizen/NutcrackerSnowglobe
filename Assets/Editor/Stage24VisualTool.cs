using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Этап 24: круглая планета из высокополигональной сетки, отдельные визуалы для игрушки, мишуры,
/// фонарика-гирлянды и снежка с мягким свечением, тихий звук подбора. Повторный запуск пересобирает визуалы.
/// </summary>
public static class Stage24VisualTool
{
    private const string PrefabDir = "Assets/Prefabs/";
    private const string PlanetMeshPath = "Assets/Meshes/PlanetMesh.asset";
    private const string SoundPath = "Assets/Audio/Sfx/pickup_soft.wav";

    [MenuItem("Nutcracker/Apply Stage 24 (round planet, detailed pickups, soft pickup sound)")]
    public static void Apply()
    {
        GameObject planet = GameObject.Find("SnowballPlanet");
        string[] prefabs = { "Toy", "Tinsel", "Garland", "SnowballPickup" };

        if (planet == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Откройте сцену SnowballLevel с SnowballPlanet.", "Ок");
            return;
        }

        foreach (string name in prefabs)
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(PrefabDir + name + ".prefab") == null)
            {
                EditorUtility.DisplayDialog("Щелкунчик", "Не найден префаб " + name + ". Сначала примените этапы 7 и 10.", "Ок");
                return;
            }
        }

        EnsureFolder("Assets", "Materials");
        EnsureFolder("Assets", "Meshes");

        ReplacePlanetMesh(planet);

        Edit("Toy", BuildToy);
        Edit("Tinsel", BuildTinsel);
        Edit("Garland", BuildGarland);
        Edit("SnowballPickup", BuildSnowballPickup);

        AssetDatabase.SaveAssets();

        AudioManager audio = Object.FindFirstObjectByType<AudioManager>();
        AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(SoundPath);

        if (audio != null && clip != null)
        {
            audio.pickupSound = clip;
            EditorUtility.SetDirty(audio);
        }
        else
        {
            Debug.LogWarning("[Щелкунчик] Звук подбора не назначен: проверьте, что " + SoundPath + " импортирован и есть AudioManager.");
        }

        Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, scene.path);

        Debug.Log("[Щелкунчик] Этап 24 применён: круглая планета, новые визуалы подбираемых предметов, мягкий звук.");
    }

    private static void ReplacePlanetMesh(GameObject planet)
    {
        if (AssetDatabase.LoadAssetAtPath<Mesh>(PlanetMeshPath) != null)
        {
            AssetDatabase.DeleteAsset(PlanetMeshPath);
        }

        Mesh mesh = MakeSphereMesh(5);
        AssetDatabase.CreateAsset(mesh, PlanetMeshPath);
        planet.GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private static Mesh MakeSphereMesh(int subdivisions)
    {
        List<Vector3> verts = new List<Vector3>();
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        Vector3[] seed =
        {
            new Vector3(-1f, t, 0f), new Vector3(1f, t, 0f), new Vector3(-1f, -t, 0f), new Vector3(1f, -t, 0f),
            new Vector3(0f, -1f, t), new Vector3(0f, 1f, t), new Vector3(0f, -1f, -t), new Vector3(0f, 1f, -t),
            new Vector3(t, 0f, -1f), new Vector3(t, 0f, 1f), new Vector3(-t, 0f, -1f), new Vector3(-t, 0f, 1f)
        };

        foreach (Vector3 s in seed)
        {
            verts.Add(s.normalized);
        }

        int[] faces =
        {
            0, 11, 5, 0, 5, 1, 0, 1, 7, 0, 7, 10, 0, 10, 11,
            1, 5, 9, 5, 11, 4, 11, 10, 2, 10, 7, 6, 7, 1, 8,
            3, 9, 4, 3, 4, 2, 3, 2, 6, 3, 6, 8, 3, 8, 9,
            4, 9, 5, 2, 4, 11, 6, 2, 10, 8, 6, 7, 9, 8, 1
        };

        List<int> tris = new List<int>(faces);

        for (int level = 0; level < subdivisions; level++)
        {
            List<int> next = new List<int>(tris.Count * 4);
            Dictionary<long, int> cache = new Dictionary<long, int>();

            for (int i = 0; i < tris.Count; i += 3)
            {
                int a = tris[i];
                int b = tris[i + 1];
                int c = tris[i + 2];
                int ab = Midpoint(verts, cache, a, b);
                int bc = Midpoint(verts, cache, b, c);
                int ca = Midpoint(verts, cache, c, a);

                next.Add(a); next.Add(ab); next.Add(ca);
                next.Add(b); next.Add(bc); next.Add(ab);
                next.Add(c); next.Add(ca); next.Add(bc);
                next.Add(ab); next.Add(bc); next.Add(ca);
            }

            tris = next;
        }

        Vector3[] positions = new Vector3[verts.Count];
        Vector3[] normals = new Vector3[verts.Count];

        for (int i = 0; i < verts.Count; i++)
        {
            normals[i] = verts[i];
            positions[i] = verts[i] * 0.5f;
        }

        for (int i = 0; i < tris.Count; i += 3)
        {
            Vector3 p0 = positions[tris[i]];
            Vector3 p1 = positions[tris[i + 1]];
            Vector3 p2 = positions[tris[i + 2]];

            if (Vector3.Dot(Vector3.Cross(p1 - p0, p2 - p0), p0 + p1 + p2) < 0f)
            {
                int swap = tris[i + 1];
                tris[i + 1] = tris[i + 2];
                tris[i + 2] = swap;
            }
        }

        Mesh mesh = new Mesh { name = "PlanetMesh", vertices = positions, normals = normals, triangles = tris.ToArray() };
        mesh.RecalculateBounds();
        return mesh;
    }

    private static int Midpoint(List<Vector3> verts, Dictionary<long, int> cache, int i, int j)
    {
        long key = i < j ? ((long)i << 32) | (uint)j : ((long)j << 32) | (uint)i;
        int index;

        if (cache.TryGetValue(key, out index))
        {
            return index;
        }

        verts.Add((verts[i] + verts[j]).normalized);
        index = verts.Count - 1;
        cache[key] = index;
        return index;
    }

    private static void Edit(string name, System.Action<GameObject> build)
    {
        string path = PrefabDir + name + ".prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(path);

        Transform old = root.transform.Find("Visual");

        if (old != null)
        {
            UnityEngine.Object.DestroyImmediate(old.gameObject);
        }

        build(root);
        PrefabUtility.SaveAsPrefabAsset(root, path);
        PrefabUtility.UnloadPrefabContents(root);
    }

    private static void BuildToy(GameObject root)
    {
        HideRootRenderer(root);
        Transform v = Visual(root).transform;
        Material red = Mat("PickupToyRed", new Color(0.8f, 0.07f, 0.1f), 0.95f, new Color(0.25f, 0.02f, 0.02f));
        Material brass = Mat("PickupBrass", new Color(1f, 0.78f, 0.3f), 0.9f, new Color(0.2f, 0.15f, 0.02f));
        Material halo = Halo("PickupToyHalo", new Color(1f, 0.6f, 0.6f, 0.1f));

        Prim(v, PrimitiveType.Sphere, Vector3.zero, Vector3.one * 0.8f, red);
        Prim(v, PrimitiveType.Cylinder, new Vector3(0f, 0.44f, 0f), new Vector3(0.18f, 0.07f, 0.18f), brass);
        Prim(v, PrimitiveType.Cylinder, new Vector3(0f, 0.55f, 0f), new Vector3(0.05f, 0.07f, 0.05f), brass);
        Prim(v, PrimitiveType.Sphere, Vector3.zero, Vector3.one * 1.3f, halo);

        SetBob(root, v, 0.05f, 1f, 40f, 0f, true);
    }

    private static void BuildTinsel(GameObject root)
    {
        HideRootRenderer(root);
        Transform v = Visual(root).transform;
        Material gold = Mat("PickupTinselGold", new Color(1f, 0.82f, 0.3f), 0.9f, new Color(0.45f, 0.32f, 0.05f));
        Material core = Mat("PickupTinselCore", new Color(1f, 0.9f, 0.5f), 0.9f, new Color(0.5f, 0.4f, 0.1f));
        Material halo = Halo("PickupTinselHalo", new Color(1f, 0.85f, 0.4f, 0.12f));

        for (int k = 0; k < 14; k++)
        {
            float a = k * 360f / 14f * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(a) * 0.12f, 0f, Mathf.Sin(a) * 0.12f);
            Prim(v, PrimitiveType.Cylinder, pos, new Vector3(0.03f, 0.36f, 0.03f), gold);
        }

        for (int k = 0; k < 10; k++)
        {
            float a = (k * 36f + 18f) * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(a) * 0.2f, 0.04f, Mathf.Sin(a) * 0.2f);
            Vector3 tilt = new Vector3(Mathf.Sin(a) * 15f, 0f, -Mathf.Cos(a) * 15f);
            Prim(v, PrimitiveType.Cylinder, pos, new Vector3(0.02f, 0.5f, 0.02f), gold, tilt);
        }

        Prim(v, PrimitiveType.Sphere, Vector3.zero, Vector3.one * 0.16f, core);
        Prim(v, PrimitiveType.Sphere, Vector3.zero, Vector3.one * 1.35f, halo);

        SetBob(root, v, 0.05f, 1f, 40f, 0f, true);
    }

    private static void BuildGarland(GameObject root)
    {
        HideRootRenderer(root);
        Transform v = Visual(root).transform;
        Material glass = Mat("PickupLanternGlass", new Color(1f, 0.85f, 0.4f), 0.95f, new Color(1f, 0.55f, 0.12f));
        Material brass = Mat("PickupLanternBrass", new Color(0.85f, 0.6f, 0.2f), 0.9f, new Color(0.15f, 0.1f, 0.02f));
        Material halo = Halo("PickupLanternHalo", new Color(1f, 0.85f, 0.45f, 0.16f));

        Prim(v, PrimitiveType.Sphere, Vector3.zero, new Vector3(0.36f, 0.44f, 0.36f), glass);
        Prim(v, PrimitiveType.Cylinder, new Vector3(0f, 0.26f, 0f), new Vector3(0.22f, 0.05f, 0.22f), brass);
        Prim(v, PrimitiveType.Cylinder, new Vector3(0f, -0.26f, 0f), new Vector3(0.2f, 0.04f, 0.2f), brass);
        Prim(v, PrimitiveType.Cylinder, new Vector3(0f, 0.36f, 0f), new Vector3(0.04f, 0.08f, 0.04f), brass);
        Prim(v, PrimitiveType.Sphere, Vector3.zero, new Vector3(0.8f, 0.95f, 0.8f), halo);

        SetBob(root, v, 0.04f, 1f, 30f, 0f, true);
    }

    private static void BuildSnowballPickup(GameObject root)
    {
        HideRootRenderer(root);
        Transform v = Visual(root).transform;
        Material white = Mat("PickupSnow", new Color(0.95f, 0.97f, 1f), 0.5f, new Color(0.25f, 0.3f, 0.4f));
        Material ring = Mat("PickupIceRing", new Color(0.4f, 0.8f, 1f), 0.9f, new Color(0.2f, 0.6f, 1f));
        Material halo = Halo("PickupSnowHalo", new Color(0.6f, 0.85f, 1f, 0.14f));

        Prim(v, PrimitiveType.Sphere, Vector3.zero, Vector3.one * 0.8f, white);
        Prim(v, PrimitiveType.Cylinder, Vector3.zero, new Vector3(1f, 0.01f, 1f), ring);
        Prim(v, PrimitiveType.Sphere, Vector3.zero, Vector3.one * 1.4f, halo);

        SetBob(root, v, 0.04f, 0.8f, 25f, 0f, true);
    }

    private static void SetBob(GameObject root, Transform visual, float height, float speed, float spin, float wobble, bool pulse)
    {
        ObjectBob bob = root.GetComponent<ObjectBob>();

        if (bob == null)
        {
            bob = root.AddComponent<ObjectBob>();
        }

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

    private static Material Halo(string name, Color color)
    {
        string path = "Assets/Materials/" + name + ".mat";
        Material m = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (m == null)
        {
            m = new Material(Shader.Find("Sprites/Default"));
            AssetDatabase.CreateAsset(m, path);
        }

        m.color = color;
        EditorUtility.SetDirty(m);
        return m;
    }

    private static Material Mat(string name, Color color, float smoothness, Color glow)
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
