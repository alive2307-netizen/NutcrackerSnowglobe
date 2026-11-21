using UnityEngine;

/// <summary>
/// Камера позади и выше игрока в его локальных осях, поэтому она
/// повторяет кривизну планеты вместе с персонажем.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Tooltip("Смещение в локальных осях игрока: вверх, назад.")]
    public Vector3 offset = new Vector3(0f, 4f, -8f);

    [Tooltip("Точка взгляда впереди игрока, м.")]
    public float lookAhead = 6f;

    public float positionSmoothing = 8f;
    public float rotationSmoothing = 8f;

    private void Start()
    {
        transform.position = target.TransformPoint(offset);
        transform.rotation = GetLookRotation();
    }

    private void LateUpdate()
    {
        Vector3 desired = target.TransformPoint(offset);
        float positionBlend = 1f - Mathf.Exp(-positionSmoothing * Time.deltaTime);
        float rotationBlend = 1f - Mathf.Exp(-rotationSmoothing * Time.deltaTime);

        transform.position = Vector3.Lerp(transform.position, desired, positionBlend);
        transform.rotation = Quaternion.Slerp(transform.rotation, GetLookRotation(), rotationBlend);
    }

    private Quaternion GetLookRotation()
    {
        Vector3 lookPoint = target.position + target.forward * lookAhead;
        return Quaternion.LookRotation(lookPoint - transform.position, target.up);
    }
}
