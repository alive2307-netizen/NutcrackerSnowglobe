using UnityEngine;
using UnityEngine.UI;

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

        label.text = player.CurrentSpeed.ToString("0.0") + " м/с";
    }
}
