using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Отладочная надпись со скоростью и активными эффектами. Нужна, чтобы на этапах 2-3
/// было видно, как работают замедление от сугроба и ускорение от льда.
/// </summary>
public class SpeedHud : MonoBehaviour
{
    public PlayerMovement player;
    public Text label;

    private void Update()
    {
        if (player == null || label == null)
        {
            return;
        }

        string effects = "";

        if (player.IsSlowed)
        {
            effects += "  замедление";
        }

        if (player.IsBoosted)
        {
            effects += "  ускорение";
        }

        label.text = "Скорость: " + player.CurrentSpeed.ToString("0.0") + " м/с"
                     + (player.IsGrounded ? "" : "  (в прыжке)")
                     + effects;
    }
}
