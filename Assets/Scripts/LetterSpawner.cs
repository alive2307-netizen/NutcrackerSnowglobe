using UnityEngine;

public class LetterSpawner : MonoBehaviour
{
    public Transform planet;
    public PhraseProgress phrase;
    public float planetRadius = 30f;
    public float surfaceLift = 1.2f;

    [Tooltip("Ось, вокруг которой стоят буквы. По умолчанию совпадает с орбитой мышей.")]
    public Vector3 orbitAxis = Vector3.right;

    public float startAngle = 30f;
    public float spacingAngle = 12f;

    private void Start()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        Vector3 axis = orbitAxis.normalized;
        Vector3 a = Vector3.Cross(axis, Vector3.up);

        if (a.sqrMagnitude < 0.0001f)
        {
            a = Vector3.Cross(axis, Vector3.forward);
        }

        a.Normalize();
        Vector3 b = Vector3.Cross(axis, a);

        for (int i = 0; i < PhraseProgress.Letters.Length; i++)
        {
            float rad = (startAngle + spacingAngle * i) * Mathf.Deg2Rad;
            Vector3 dir = a * Mathf.Cos(rad) + b * Mathf.Sin(rad);
            CreateLetter(i, planet.position + dir * (planetRadius + surfaceLift), font);
        }
    }

    private void CreateLetter(int index, Vector3 position, Font font)
    {
        GameObject root = new GameObject("Letter_" + PhraseProgress.Letters[index]);
        root.transform.position = position;

        SphereCollider trigger = root.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 1.2f;

        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "Ball";
        ball.transform.SetParent(root.transform, false);
        ball.transform.localScale = Vector3.one * 1.6f;
        Destroy(ball.GetComponent<Collider>());
        ball.GetComponent<Renderer>().material = MakeMaterial(PhraseProgress.ColorOf(index));

        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(root.transform, false);
        TextMesh label = labelObject.AddComponent<TextMesh>();
        label.text = PhraseProgress.Letters[index].ToString();
        label.font = font;
        label.fontSize = 96;
        label.characterSize = 0.14f;
        label.anchor = TextAnchor.MiddleCenter;
        label.alignment = TextAlignment.Center;
        label.color = Color.white;
        labelObject.GetComponent<MeshRenderer>().sharedMaterial = font.material;

        LetterPickup pickup = root.AddComponent<LetterPickup>();
        pickup.phrase = phrase;
        pickup.index = index;
        pickup.label = label;
    }

    private static Material MakeMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        Material material = new Material(shader);

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }

        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }

        return material;
    }
}
