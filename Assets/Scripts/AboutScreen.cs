using UnityEngine;
using UnityEngine.UI;

public class AboutScreen : MonoBehaviour
{
    public GameObject panel;
    public Button openButton;
    public Button backButton;

    private void Awake()
    {
        openButton.onClick.AddListener(() => panel.SetActive(true));
        backButton.onClick.AddListener(() => panel.SetActive(false));
        panel.SetActive(false);
    }
}
