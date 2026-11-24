using UnityEngine;
using UnityEngine.UI;

public class GarlandIndicator : MonoBehaviour
{
    public HealthManager health;
    public Image[] bulbs;
    public Color litColor;
    public Color dimColor;

    private void Update()
    {
        bool immune = health.IsInvincible;

        for (int i = 0; i < bulbs.Length; i++)
        {
            bool lit = !immune && i < health.GarlandParts;
            bulbs[i].sprite = HudSprites.Disc;
            bulbs[i].color = lit ? litColor : dimColor;
        }
    }
}
