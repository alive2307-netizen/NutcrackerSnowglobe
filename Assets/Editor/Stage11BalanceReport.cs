using UnityEditor;
using UnityEngine;

/// <summary>
/// Этап 11: выводит в Console сводку баланса раунда: длину трассы, расстояние за раунд
/// при минимальной, базовой и максимальной скорости, количество препятствий и предметов.
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
        float duration = game.roundDuration;

        float slowDistance = player.minSpeed * duration;
        float baseDistance = player.baseSpeed * duration;
        float fastDistance = player.maxSpeed * duration;

        int trees = CountNamed("Tree");
        int drifts = CountNamed("Snowdrift");
        int ice = CountNamed("IcePatch");
        int toys = CountNamed("Toy");
        int tinsel = CountNamed("Tinsel");
        int garlands = CountNamed("Garland");
        int snowballs = CountNamed("SnowballPickup");
        int mice = Object.FindObjectsOfType<MouseEnemy>().Length;

        string report =
            "[Баланс раунда]\n" +
            "Длина большого круга трассы: " + circumference.ToString("0") + " м\n" +
            "Длительность раунда: " + duration.ToString("0") + " с\n" +
            "Путь за раунд: при " + player.minSpeed + " м/с — " + slowDistance.ToString("0") +
            " м, при " + player.baseSpeed + " м/с — " + baseDistance.ToString("0") +
            " м, при " + player.maxSpeed + " м/с — " + fastDistance.ToString("0") + " м\n" +
            "Кругов при базовой скорости: " + (baseDistance / circumference).ToString("0.0") + "\n" +
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
