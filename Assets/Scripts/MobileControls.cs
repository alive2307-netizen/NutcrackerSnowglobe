using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MobileControls : MonoBehaviour
{
    public InputReader input;
    public GameObject[] touchControls;
    public Button calibrateButton;

    private bool touch;

    private void Awake()
    {
        touch = Application.isMobilePlatform || Touchscreen.current != null || Application.isEditor;

        foreach (GameObject control in touchControls)
        {
            if (control != null)
            {
                control.SetActive(touch);
            }
        }

        calibrateButton.onClick.AddListener(Calibrate);
    }

    private void Update()
    {
        bool inMenu = GameManager.Instance == null || GameManager.Instance.State == GameState.Menu;
        calibrateButton.gameObject.SetActive(touch && inMenu);
    }

    public void Calibrate()
    {
        input.CalibrateGyro();
    }
}
