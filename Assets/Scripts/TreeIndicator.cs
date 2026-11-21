using UnityEngine;
using UnityEngine.UI;

public class TreeIndicator : MonoBehaviour
{
    public HealthManager health;
    public Image fill;
    public Image[] bulbs;
    public float[] bulbLevels;
    public Color[] bulbColors;

    private void Update()
    {
        float level = health.maxHealth > 0f ? health.CurrentHealth / health.maxHealth : 0f;
        fill.fillAmount = level;

        float pulse = 0.75f + 0.25f * Mathf.Sin(Time.time * 5f);

        for (int i = 0; i < bulbs.Length; i++)
        {
            if (level < bulbLevels[i])
            {
                bulbs[i].color = new Color(0.3f, 0.3f, 0.3f, 0.35f);
                continue;
            }

            Color c = bulbColors[i % bulbColors.Length];
            bulbs[i].color = new Color(c.r * pulse, c.g * pulse, c.b * pulse, 1f);
        }
    }
}
