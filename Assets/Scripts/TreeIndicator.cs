using UnityEngine;
using UnityEngine.UI;

public class TreeIndicator : MonoBehaviour
{
    public HealthManager health;
    public Image fill;

    private void Update()
    {
        fill.fillAmount = health.maxHealth > 0f ? health.CurrentHealth / health.maxHealth : 0f;
    }
}
