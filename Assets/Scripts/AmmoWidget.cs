using UnityEngine;
using UnityEngine.UI;

public class AmmoWidget : MonoBehaviour
{
    public SnowballShooter shooter;
    public Image[] slots;
    public Sprite emptySprite;
    public Sprite filledSprite;

    private void Update()
    {
        if (shooter == null || slots == null)
        {
            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            bool filled = i < shooter.Ammo;
            slots[i].sprite = filled ? filledSprite : emptySprite;
        }
    }
}
