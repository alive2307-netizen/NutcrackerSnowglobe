using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MobileControls : MonoBehaviour
{
    public InputReader input;
    public GameObject[] touchControls;
    public Button calibrateButton;

    private void Awake()
    {
        bool touch = Application.isMobilePlatform || Touchscreen.current != null || Application.isEditor;

        foreach (GameObject control in touchControls)
        {
            if (control != null)
            {
                control.SetActive(touch);
            }
        }

        calibrateButton.gameObject.SetActive(touch);
        calibrateButton.onClick.AddListener(Calibrate);
    }

    public void Calibrate()
    {
        input.CalibrateGyro();
    }
}
