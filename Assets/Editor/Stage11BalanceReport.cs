using UnityEditor;
using UnityEngine;

/// <summary>
/// Выводит в Console сводку по трассе: длину круга, скорость и количество объектов.
/// Ничего в сцене не меняет.
/// </summary>
public static class Stage11BalanceReport
{
    [MenuItem("Nutcracker/Stage 11 balance report")]
    public static void Report()
    {
        GameManager game = Object.FindObjectOfType<GameManager>();
        PlayerMovement player = Object.FindObjectOfType<PlayerMovement>();

        if (game == null || player == null)
        {
            EditorUtility.DisplayDialog("Щелкунчик", "Откройте сцену SnowballLevel.", "Ок");
            return;
        }

        float radius = 30f;
        float circumference = 2f * Mathf.PI * radius;

        int trees = CountNamed("Tree");
        int drifts = CountNamed("Snowdrift");
        int ice = CountNamed("IcePatch");
        int toys = CountNamed("Toy");
        int tinsel = CountNamed("Tinsel");
        int garlands = CountNamed("Garland");
        int snowballs = CountNamed("SnowballPickup");
        int mice = Object.FindObjectsOfType<MouseEnemy>().Length;

        string report =
            "[Баланс]\n" +
            "Длина большого круга трассы: " + circumference.ToString("0") + " м\n" +
            "Скорость: мин " + player.minSpeed + ", база " + player.baseSpeed + ", макс " + player.maxSpeed + " м/с\n" +
            "Круг при базовой скорости: " + (circumference / player.baseSpeed).ToString("0") + " с\n" +
            "Ёлки: " + trees + ", сугробы: " + drifts + ", лёд: " + ice + "\n" +
            "Игрушки: " + toys + ", мишура: " + tinsel + ", гирлянды: " + garlands +
            ", снежки: " + snowballs + ", мыши: " + mice;

        Debug.Log(report);
    }

    private static int CountNamed(string prefix)
    {
        int count = 0;

        foreach (Transform t in Object.FindObjectsOfType<Transform>())
        {
            if (t.name.StartsWith(prefix))
            {
                count++;
            }
        }

        return count;
    }
}
