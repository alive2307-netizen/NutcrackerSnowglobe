using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ёлка-2: индикатор здоровья в левом верхнем углу. Заливка Image в режиме Filled
/// идёт снизу вверх; звезда на верхушке загорается при победе.
/// </summary>
public class HealthTreeUI : MonoBehaviour
{
    public HealthManager health;
    public GameManager game;
    public Image fill;
    public Image star;

    public Color starOffColor = new Color(0.4f, 0.4f, 0.45f, 0.6f);
    public Color starOnColor = new Color(1f, 0.85f, 0.3f, 1f);

    private void Awake()
    {
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Vertical;
        fill.fillOrigin = (int)Image.OriginVertical.Bottom;
    }

    private void Update()
    {
        float ratio = health.maxHealth > 0f ? health.CurrentHealth / health.maxHealth : 0f;
        fill.fillAmount = Mathf.Clamp01(ratio);

        star.color = game.State == GameState.Win ? starOnColor : starOffColor;
    }
}
