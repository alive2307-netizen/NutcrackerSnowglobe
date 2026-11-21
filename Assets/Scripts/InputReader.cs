using UnityEngine;
using UnityEngine.InputSystem;

public enum TiltAxis
{
    X,
    Y,
    Z
}

/// <summary>
/// Единая точка чтения ввода: клавиатура (WASD и стрелки), экранный джойстик
/// и геймпад идут через одно действие Move, гироскоп добавляется поверх.
/// Экранный джойстик пишет в виртуальный gamepad, поэтому привязка "&lt;Gamepad&gt;/leftStick".
/// </summary>
public class InputReader : MonoBehaviour
{
    [Header("Gyroscope")]
    public bool useGyro = true;

    [Tooltip("Наклон в градусах, при котором достигается полное отклонение.")]
    public float gyroMaxAngle = 20f;

    [Tooltip("Мёртвая зона в градусах: гасит дрожание рук.")]
    public float gyroDeadZone = 3f;

    [Tooltip("Ось поворота при наклоне влево-вправо. В портретной ориентации обычно Z, подбирается на устройстве.")]
    public TiltAxis tiltAxis = TiltAxis.Z;

    public bool invertTilt;

    private InputAction move;
    private InputAction jump;
    private Quaternion gyroReference = Quaternion.identity;
    private bool gyroCalibrated;

    public Vector2 Move
    {
        get
        {
            Vector2 value = move.ReadValue<Vector2>();
            float tilt = ReadGyroTurn();

            if (Mathf.Abs(tilt) > Mathf.Abs(value.x))
            {
                value.x = tilt;
            }

            return value;
        }
    }

    public bool JumpPressed => jump.WasPressedThisFrame();

    private void Awake()
    {
        move = new InputAction("Move", InputActionType.Value);
        move.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        move.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
        move.AddBinding("<Gamepad>/leftStick");

        jump = new InputAction("Jump", InputActionType.Button);
        jump.AddBinding("<Keyboard>/space");
        jump.AddBinding("<Gamepad>/buttonSouth");
    }

    private void OnEnable()
    {
        move.Enable();
        jump.Enable();

        if (AttitudeSensor.current != null)
        {
            InputSystem.EnableDevice(AttitudeSensor.current);
        }
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();

        if (AttitudeSensor.current != null)
        {
            InputSystem.DisableDevice(AttitudeSensor.current);
        }
    }

    private void Start()
    {
        Debug.Log(AttitudeSensor.current != null ? "Гироскоп найден" : "Гироскоп не найден");
    }

    public void CalibrateGyro()
    {
        if (AttitudeSensor.current == null)
        {
            return;
        }

        gyroReference = AttitudeSensor.current.attitude.ReadValue();
        gyroCalibrated = true;
    }

    private float ReadGyroTurn()
    {
        if (!useGyro || AttitudeSensor.current == null)
        {
            return 0f;
        }

        if (!gyroCalibrated)
        {
            CalibrateGyro();
        }

        Quaternion delta = Quaternion.Inverse(gyroReference) * AttitudeSensor.current.attitude.ReadValue();
        Vector3 euler = delta.eulerAngles;

        float angle;
        switch (tiltAxis)
        {
            case TiltAxis.X:
                angle = euler.x;
                break;
            case TiltAxis.Y:
                angle = euler.y;
                break;
            default:
                angle = euler.z;
                break;
        }

        angle = Mathf.DeltaAngle(0f, angle);
        if (invertTilt)
        {
            angle = -angle;
        }

        if (Mathf.Abs(angle) <= gyroDeadZone)
        {
            return 0f;
        }

        return Mathf.Clamp((angle - Mathf.Sign(angle) * gyroDeadZone) / (gyroMaxAngle - gyroDeadZone), -1f, 1f);
    }
}
