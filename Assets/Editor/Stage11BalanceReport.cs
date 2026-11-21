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

        float circumference = 2f * Mathf.PI * 30f;

        string report =
            "[Баланс]\n" +
            "Длина большого круга трассы: " + circumference.ToString("0") + " м\n" +
            "Скорость: база " + player.baseSpeed + ", макс " + player.maxBoostedSpeed + " м/с\n" +
            "Круг при базовой скорости: " + (circumference / player.baseSpeed).ToString("0") + " с\n" +
            "Ёлки: " + CountNamed("Tree") + ", сугробы: " + CountNamed("Snowdrift") +
            ", лёд: " + CountNamed("IcePatch") + "\n" +
            "Игрушки: " + CountNamed("Toy") + ", мишура: " + CountNamed("Tinsel") +
            ", гирлянды: " + CountNamed("Garland") + ", снежки: " + CountNamed("SnowballPickup") +
            ", мыши: " + Object.FindObjectsOfType<MouseEnemy>().Length;

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
