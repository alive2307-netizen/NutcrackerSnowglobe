using UnityEngine;
using UnityEngine.UI;

public class AmmoWidget : MonoBehaviour
{
    public SnowballShooter shooter;
    public Image[] slots;

    private void Update()
    {
        if (shooter == null || slots == null)
        {
            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].sprite = i < shooter.Ammo ? HudSprites.Disc : HudSprites.Ring;
        }
    }
}
