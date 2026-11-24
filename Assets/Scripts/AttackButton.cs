using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AttackButton : MonoBehaviour
{
    public SnowballShooter shooter;

    private void Awake()
    {
        bool touch = Application.isMobilePlatform || Touchscreen.current != null || Application.isEditor;

        GetComponent<Button>().onClick.AddListener(() => shooter.Throw());
        gameObject.SetActive(touch);
    }
}
