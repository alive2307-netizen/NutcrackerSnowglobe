using UnityEngine;
using UnityEngine.UI;

public class AmmoWidget : MonoBehaviour
{
    public SnowballShooter shooter;
    public Text label;

    private void Update()
    {
        if (shooter == null || label == null)
        {
            return;
        }

        label.text = "Снежки: " + shooter.Ammo + " / " + shooter.maxAmmo;
    }
}
