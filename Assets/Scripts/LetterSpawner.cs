using UnityEngine;

public class LetterSpawner : MonoBehaviour
{
    private const int MaxAttempts = 4000;

    public Transform planet;
    public PhraseProgress phrase;
    public float planetRadius = 30f;
    public float surfaceLift = 1.2f;

    [Tooltip("Минимальный угол между буквами по поверхности планеты, градусов.")]
    public float minSeparation = 28f;

    private void Start()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        Vector3[] placed = new Vector3[PhraseProgress.LetterCount];
        int count = 0;

        for (int i = 0; i < PhraseProgress.Phrase.Length && count < placed.Length; i++)
        {
            if (PhraseProgress.Phrase[i] == ' ')
            {
                continue;
            }

            placed[count] = RandomFreeDirection(placed, count);
            CreateLetter(i, planet.position + placed[count] * (planetRadius + surfaceLift), font);
            count++;
        }
    }

    private Vector3 RandomFreeDirection(Vector3[] placed, int count)
    {
        Vector3 best = Random.onUnitSphere;
        float bestGap = -1f;

        for (int attempt = 0; attempt < MaxAttempts; attempt++)
        {
            Vector3 dir = Random.onUnitSphere;
            float gap = float.MaxValue;

            for (int j = 0; j < count; j++)
            {
                gap = Mathf.Min(gap, Vector3.Angle(dir, placed[j]));
            }

            if (gap >= minSeparation)
            {
                return dir;
            }

            if (gap > bestGap)
            {
                bestGap = gap;
                best = dir;
            }
        }

        return best;
    }

    private void CreateLetter(int index, Vector3 position, Font font)
    {
        GameObject root = new GameObject("Letter_" + PhraseProgress.Phrase[index]);
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
        label.text = PhraseProgress.Phrase[index].ToString();
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
