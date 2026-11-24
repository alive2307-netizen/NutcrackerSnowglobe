using UnityEngine;
using UnityEngine.UI;

public class PhraseProgress : MonoBehaviour
{
    public const string Letters = "СНОВЫМГОДОМ";
    public const string Phrase = "С НОВЫМ ГОДОМ";

    private static readonly Color[] Palette =
    {
        new Color(1f, 0.25f, 0.25f),
        new Color(1f, 0.82f, 0.25f),
        new Color(0.3f, 0.6f, 1f),
        new Color(0.4f, 1f, 0.5f),
        new Color(0.62f, 0.25f, 0.95f),
        new Color(1f, 1f, 1f)
    };

    public Text[] slots;

    private bool[] found;

    public static Color ColorOf(int index)
    {
        return Palette[index % Palette.Length];
    }

    private void Awake()
    {
        found = new bool[slots.Length];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].text = "_";
            slots[i].color = new Color(1f, 1f, 1f, 0.35f);
        }
    }

    public void Collect(int index)
    {
        if (found[index])
        {
            return;
        }

        found[index] = true;
        slots[index].text = Letters[index].ToString();
        slots[index].color = ColorOf(index);

        foreach (bool f in found)
        {
            if (!f)
            {
                return;
            }
        }

        GameManager.Instance.Win("Фраза собрана: " + Phrase);
    }
}
