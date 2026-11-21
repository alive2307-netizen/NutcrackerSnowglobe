using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

/// <summary>
/// Собирает уровень SnowballLevel: слой Planet, материалы, префабы (сугроб, лёд, ель)
/// и все объекты сцены с настроенными компонентами. Запускается сам при первом открытии
/// проекта, повторно - через меню Nutcracker.
/// </summary>
[InitializeOnLoad]
public static class SnowglobeSceneBuilder
{
    private const string ScenePath = "Assets/Scenes/SnowballLevel.unity";
    private const string PlanetLayerName = "Planet";
    private const float PlanetScale = 60f;
    private const float PlanetRadius = PlanetScale * 0.5f;
    private const string SessionKey = "snowglobe.autobuild.checked";

    private struct Placement
    {
        public GameObject prefab;
        public float theta;
        public float phi;
        public float lift;

        public Placement(GameObject prefab, float theta, float phi, float lift)
        {
            this.prefab = prefab;
            this.theta = theta;
            this.phi = phi;
            this.lift = lift;
        }
    }

    static SnowglobeSceneBuilder()
    {
        if (SessionState.GetBool(SessionKey, false))
        {
            return;
        }

        EditorApplication.delayCall += AutoBuildOnFirstOpen;
    }

    private static void AutoBuildOnFirstOpen()
    {
        SessionState.SetBool(SessionKey, true);
        EnsureInputHandling();

        UnityEngine.SceneManagement.Scene active = EditorSceneManager.GetActiveScene();

        if (active.isDirty || !string.IsNullOrEmpty(active.path))
        {
            return;
        }

        if (File.Exists(ScenePath))
        {
            EditorSceneManager.OpenScene(ScenePath);
            return;
        }

        Build();
    }

    [MenuItem("Nutcracker/Build Level Scene (Stages 1-4)")]
    public static void BuildFromMenu()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        EnsureInputHandling();
        Build();
    }

    private static void Build()
    {
        int planetLayer = EnsureLayer(PlanetLayerName);

        EnsureFolder("Assets", "Scenes");
        EnsureFolder("Assets", "Materials");
        EnsureFolder("Assets", "Prefabs");

        Material snowMat = MakeMaterial("SnowMat", new Color(0.94f, 0.96f, 1f));
        Material driftMat = MakeMaterial("DriftMat", new Color(0.82f, 0.88f, 0.97f));
        Material iceMat = MakeMaterial("IceMat", new Color(0.45f, 0.8f, 1f));
        Material trunkMat = MakeMaterial("TrunkMat", new Color(0.4f, 0.26f, 0.15f));
        Material crownMat = MakeMaterial("CrownMat", new Color(0.1f, 0.4f, 0.25f));
        Material playerMat = MakeMaterial("PlayerMat", new Color(0.8f, 0.15f, 0.2f));
        Material noseMat = MakeMaterial("NoseMat", new Color(1f, 0.85f, 0.2f));
        PhysicsMaterial noFriction = MakeNoFriction();

        EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        GameObject driftPrefab = MakeSnowdriftPrefab(driftMat);
        GameObject icePrefab = MakeIcePrefab(iceMat);
        GameObject treePrefab = MakeTreePrefab(trunkMat, crownMat);

        GameObject planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        planet.name = "SnowballPlanet";
        planet.transform.position = Vector3.zero;
        planet.transform.localScale = Vector3.one * PlanetScale;
        planet.layer = planetLayer;
        planet.GetComponent<MeshRenderer>().sharedMaterial = snowMat;

        GameObject inputObject = new GameObject("InputReader");
        InputReader inputReader = inputObject.AddComponent<InputReader>();

        GameObject player = BuildPlayer(planet.transform, inputReader, planetLayer, playerMat, noseMat, noFriction);
        PlayerMovement movement = player.GetComponent<PlayerMovement>();

        BuildCamera(player.transform);

        GameObject gameManager = new GameObject("GameManager");
        gameManager.AddComponent<GameManager>().player = movement;

        BuildObstacles(driftPrefab, icePrefab, treePrefab);
        BuildUi(movement);

        UnityEngine.SceneManagement.Scene scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.SaveScene(scene, ScenePath);
        AddToBuildSettings(ScenePath);
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = player;

        Debug.Log("[Щелкунчик] Сцена создана: " + ScenePath + ". Нажмите Play.");
    }

    // ------------------------------------------------------------------ игрок и камера

    private static GameObject BuildPlayer(
        Transform planet, InputReader input, int planetLayer,
        Material bodyMat, Material noseMat, PhysicsMaterial noFriction)
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = new Vector3(0f, PlanetRadius + 1.05f, 0f);
        player.GetComponent<MeshRenderer>().sharedMaterial = bodyMat;

        CapsuleCollider capsule = player.GetComponent<CapsuleCollider>();
        capsule.sharedMaterial = noFriction;

        Rigidbody body = player.AddComponent<Rigidbody>();
        body.mass = 1f;
        body.useGravity = false;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.constraints = RigidbodyConstraints.FreezeRotation;

        PlanetGravity gravity = player.AddComponent<PlanetGravity>();
        gravity.planet = planet;

        PlayerMovement movement = player.AddComponent<PlayerMovement>();
        movement.input = input;
        movement.groundMask = 1 << planetLayer;

        GameObject nose = GameObject.CreatePrimitive(PrimitiveType.Cube);
        nose.name = "Nose";
        nose.transform.SetParent(player.transform, false);
        nose.transform.localPosition = new Vector3(0f, 0.45f, 0.5f);
        nose.transform.localScale = new Vector3(0.3f, 0.3f, 0.5f);
        nose.GetComponent<MeshRenderer>().sharedMaterial = noseMat;
        Object.DestroyImmediate(nose.GetComponent<BoxCollider>());

        return player;
    }

    private static void BuildCamera(Transform target)
    {
        Camera cam = Camera.main;

        if (cam == null)
        {
            GameObject camObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            camObject.tag = "MainCamera";
            cam = camObject.GetComponent<Camera>();
        }

        CameraFollow follow = cam.gameObject.AddComponent<CameraFollow>();
        follow.target = target;
        cam.transform.position = target.TransformPoint(follow.offset);
        cam.transform.LookAt(target.position + target.forward * follow.lookAhead, target.up);
    }

    // -------------------------------------------------------------------- препятствия

    private static void BuildObstacles(GameObject drift, GameObject ice, GameObject tree)
    {
        // theta - угол вдоль маршрута игрока (он стартует в 0 и бежит к росту theta),
        // phi - боковой сдвиг: плюс это вправо от игрока. 1 градус - около 0.52 м.
        Placement[] placements =
        {
            new Placement(drift, 40f, 0f, 0.1f),
            new Placement(ice, 100f, 0f, 0.03f),
            new Placement(tree, 135f, 14f, 0f),
            new Placement(tree, 150f, -14f, 0f),
            new Placement(drift, 170f, -3f, 0.1f),
            new Placement(ice, 215f, -5f, 0.03f),
            new Placement(tree, 250f, 0f, 0f),
            new Placement(drift, 290f, 5f, 0.1f),
            new Placement(tree, 330f, 10f, 0f)
        };

        GameObject parent = new GameObject("Obstacles");
        int index = 0;

        foreach (Placement p in placements)
        {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(p.prefab, parent.transform);
            Vector3 direction = SurfaceDirection(p.theta, p.phi);

            instance.transform.position = direction * (PlanetRadius + p.lift);
            instance.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
            instance.name = p.prefab.name + " (" + (++index) + ")";
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

    private static GameObject MakeSnowdriftPrefab(Material mat)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Snowdrift";
        go.transform.localScale = new Vector3(3f, 1f, 3f);
        go.GetComponent<MeshRenderer>().sharedMaterial = mat;

        // Сферный коллайдер масштабируется по наибольшей оси и вышел бы слишком высоким,
        // поэтому вместо него плоский триггер-бокс: сугроб можно перепрыгнуть.
        Object.DestroyImmediate(go.GetComponent<SphereCollider>());
        BoxCollider trigger = go.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = Vector3.one;

        go.AddComponent<SlowZone>();
        return SavePrefab(go, "Assets/Prefabs/Snowdrift.prefab");
    }

    private static GameObject MakeIcePrefab(Material mat)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = "IcePatch";
        go.transform.localScale = new Vector3(3.5f, 0.05f, 3.5f);
        go.GetComponent<MeshRenderer>().sharedMaterial = mat;

        Object.DestroyImmediate(go.GetComponent<CapsuleCollider>());
        BoxCollider trigger = go.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(1f, 10f, 1f);

        go.AddComponent<SpeedBoost>();
        return SavePrefab(go, "Assets/Prefabs/IcePatch.prefab");
    }

    private static GameObject MakeTreePrefab(Material trunkMat, Material crownMat)
    {
        GameObject root = new GameObject("Tree");

        CapsuleCollider collider = root.AddComponent<CapsuleCollider>();
        collider.center = new Vector3(0f, 2f, 0f);
        collider.radius = 0.9f;
        collider.height = 4f;
        collider.isTrigger = false;

        root.AddComponent<TreeCollision>();

        AddTreePart(root.transform, "Trunk", new Vector3(0f, 0.7f, 0f), new Vector3(0.4f, 0.7f, 0.4f), trunkMat);
        AddTreePart(root.transform, "CrownLow", new Vector3(0f, 1.7f, 0f), new Vector3(2.2f, 0.45f, 2.2f), crownMat);
        AddTreePart(root.transform, "CrownMid", new Vector3(0f, 2.6f, 0f), new Vector3(1.6f, 0.45f, 1.6f), crownMat);
        AddTreePart(root.transform, "CrownTop", new Vector3(0f, 3.5f, 0f), new Vector3(1f, 0.45f, 1f), crownMat);

        return SavePrefab(root, "Assets/Prefabs/Tree.prefab");
    }

    private static void AddTreePart(Transform parent, string name, Vector3 position, Vector3 scale, Material mat)
    {
        GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        part.name = name;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = position;
        part.transform.localScale = scale;
        part.GetComponent<MeshRenderer>().sharedMaterial = mat;
        Object.DestroyImmediate(part.GetComponent<CapsuleCollider>());
    }

    private static GameObject SavePrefab(GameObject go, string path)
    {
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    // ------------------------------------------------------------------------------ UI

    private static void BuildUi(PlayerMovement player)
    {
        GameObject canvasObject = new GameObject(
            "Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        Sprite knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        Transform root = canvasObject.transform;

        RectTransform background = MakeImage(
            "JoystickBackground", root, knob, new Color(1f, 1f, 1f, 0.3f),
            new Vector2(0f, 0f), new Vector2(250f, 280f), new Vector2(300f, 300f));
        background.GetComponent<Image>().raycastTarget = false;

        RectTransform handle = MakeImage(
            "JoystickHandle", root, knob, new Color(1f, 0.6f, 0.15f, 0.9f),
            new Vector2(0f, 0f), new Vector2(250f, 280f), new Vector2(120f, 120f));
        OnScreenStick stick = handle.gameObject.AddComponent<OnScreenStick>();
        stick.controlPath = "<Gamepad>/leftStick";
        stick.movementRange = 100f;

        RectTransform jump = MakeImage(
            "JumpButton", root, knob, new Color(0.6f, 0.85f, 1f, 0.8f),
            new Vector2(1f, 0f), new Vector2(-170f, 280f), new Vector2(180f, 180f));
        OnScreenButton jumpButton = jump.gameObject.AddComponent<OnScreenButton>();
        jumpButton.controlPath = "<Gamepad>/buttonSouth";

        MakeText("JumpLabel", jump, font, "ПРЫЖОК", 34, new Color(0.05f, 0.1f, 0.2f),
            new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(180f, 60f));

        MakeText("Hint", root, font,
            "A/D или стрелки - поворот, W/S - скорость, Пробел - прыжок",
            34, Color.white, new Vector2(0.5f, 1f), new Vector2(0f, -70f), new Vector2(1000f, 100f));

        Text speedText = MakeText("SpeedHud", root, font, "", 40, new Color(1f, 0.9f, 0.5f),
            new Vector2(0.5f, 1f), new Vector2(0f, -170f), new Vector2(1000f, 80f));

        SpeedHud hud = canvasObject.AddComponent<SpeedHud>();
        hud.player = player;
        hud.label = speedText;

        // Без InputSystemUIInputModule экранные элементы не получают касаний.
        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
    }

    private static RectTransform MakeImage(
        string name, Transform parent, Sprite sprite, Color color,
        Vector2 anchor, Vector2 position, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.color = color;

        return rect;
    }

    private static Text MakeText(
        string name, Transform parent, Font font, string content, int fontSize,
        Color color, Vector2 anchor, Vector2 position, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);

        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Text text = go.GetComponent<Text>();
        text.font = font;
        text.text = content;
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;
        text.raycastTarget = false;

        return text;
    }

    // ---------------------------------------------------------------------- ассеты

    private static Material MakeMaterial(string name, Color color)
    {
        string path = "Assets/Materials/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (material == null)
        {
            Shader shader = Shader.Find("Standard");

            if (shader == null)
            {
                shader = Shader.Find("Universal Render Pipeline/Lit");
            }

            material = new Material(shader);
            AssetDatabase.CreateAsset(material, path);
        }

        material.color = color;
        EditorUtility.SetDirty(material);
        return material;
    }

    private static PhysicsMaterial MakeNoFriction()
    {
        string path = "Assets/Materials/NoFriction.asset";
        PhysicsMaterial material = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(path);

        if (material == null)
        {
            material = new PhysicsMaterial();
            AssetDatabase.CreateAsset(material, path);
        }

        material.dynamicFriction = 0f;
        material.staticFriction = 0f;
        material.bounciness = 0f;
        material.frictionCombine = PhysicsMaterialCombine.Minimum;
        material.bounceCombine = PhysicsMaterialCombine.Minimum;
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

    private static int EnsureLayer(string layerName)
    {
        int existing = LayerMask.NameToLayer(layerName);

        if (existing >= 0)
        {
            return existing;
        }

        Object[] assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        SerializedObject tagManager = new SerializedObject(assets[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");

        for (int i = 8; i < 32; i++)
        {
            SerializedProperty slot = layers.GetArrayElementAtIndex(i);

            if (string.IsNullOrEmpty(slot.stringValue))
            {
                slot.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
                return i;
            }
        }

        Debug.LogError("[Щелкунчик] Нет свободного слоя для " + layerName);
        return 0;
    }

    private static void EnsureInputHandling()
    {
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");

        if (assets == null || assets.Length == 0)
        {
            return;
        }

        SerializedObject settings = new SerializedObject(assets[0]);
        SerializedProperty handler = settings.FindProperty("activeInputHandler");

        if (handler == null || handler.intValue == 1 || handler.intValue == 2)
        {
            return;
        }

        handler.intValue = 2;
        settings.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog(
            "Щелкунчик",
            "Active Input Handling переключён на Both. Перезапустите Unity "
            + "(закройте редактор и откройте проект снова), иначе ввод не заработает.",
            "Понятно");
    }

    private static void AddToBuildSettings(string path)
    {
        System.Collections.Generic.List<EditorBuildSettingsScene> scenes =
            new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

        foreach (EditorBuildSettingsScene existing in scenes)
        {
            if (existing.path == path)
            {
                return;
            }
        }

        scenes.Insert(0, new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
