using UnityEngine;
using UnityEngine.UI;

public class PhraseProgress : MonoBehaviour
{
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
    private int collected;

    public static int LetterCount
    {
        get
        {
            int count = 0;

            foreach (char c in Phrase)
            {
                if (c != ' ')
                {
                    count++;
                }
            }

            return count;
        }
    }

    public static Color ColorOf(int charIndex)
    {
        return Palette[charIndex % Palette.Length];
    }

    private void Awake()
    {
        found = new bool[slots.Length];

        for (int i = 0; i < slots.Length; i++)
        {
            bool space = Phrase[i] == ' ';
            slots[i].text = space ? "" : "_";
            slots[i].color = new Color(1f, 1f, 1f, 0.35f);
        }
    }

    public void Collect(int charIndex)
    {
        if (found[charIndex])
        {
            return;
        }

        found[charIndex] = true;
        collected++;
        slots[charIndex].text = Phrase[charIndex].ToString();
        slots[charIndex].color = ColorOf(charIndex);

        if (collected >= LetterCount)
        {
            GameManager.Instance.Win("Фраза собрана: " + Phrase);
        }
    }
}
