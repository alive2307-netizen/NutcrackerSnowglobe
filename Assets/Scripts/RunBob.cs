using UnityEngine;

public class RunBob : MonoBehaviour
{
    [Tooltip("Дочерний объект с моделью щелкунчика. Сам Player не трогаем, физика остаётся прежней.")]
    public Transform visual;

    public PlayerMovement player;

    [Tooltip("Высота подпрыгивания, м.")]
    public float bobHeight = 0.08f;

    [Tooltip("Наклон корпуса вперёд-назад, градусов.")]
    public float tiltAngle = 6f;

    [Tooltip("Шагов в секунду на базовой скорости.")]
    public float stepsPerSecond = 2.5f;

    [Tooltip("Скорость, с которой покачивание нарастает и затухает.")]
    public float blendSpeed = 4f;

    private Vector3 restPosition;
    private Quaternion restRotation;
    private float phase;
    private float amount;

    private void Awake()
    {
        restPosition = visual.localPosition;
        restRotation = visual.localRotation;
    }

    private void Update()
    {
        bool running = player.canMove && player.IsGrounded;
        float speedRatio = running && player.baseSpeed > 0f ? player.CurrentSpeed / player.baseSpeed : 0f;

        amount = Mathf.MoveTowards(amount, Mathf.Clamp01(speedRatio), blendSpeed * Time.deltaTime);
        phase = Mathf.Repeat(phase + 2f * Mathf.PI * stepsPerSecond * speedRatio * Time.deltaTime, 2f * Mathf.PI);

        float wave = Mathf.Sin(phase);
        float lift = Mathf.Abs(wave) * bobHeight * amount;
        float tilt = wave * tiltAngle * amount;

        visual.localPosition = restPosition + Vector3.up * lift;
        visual.localRotation = restRotation * Quaternion.Euler(tilt, 0f, 0f);
    }
}
