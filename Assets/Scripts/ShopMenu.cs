using UnityEngine;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    public Button openButton;
    public Button closeButton;
    public GameObject panel;
    public Text coinsText;

    private bool open;

    private void Awake()
    {
        openButton.onClick.AddListener(() => open = true);
        closeButton.onClick.AddListener(() => open = false);
    }

    private void Update()
    {
        bool menu = GameManager.Instance == null || GameManager.Instance.State == GameState.Menu;

        if (!menu)
        {
            open = false;
        }

        panel.SetActive(open);
        coinsText.text = "Монеты: " + CoinWallet.Coins;
    }
}
