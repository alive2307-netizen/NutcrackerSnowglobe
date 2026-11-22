using UnityEngine;

/// <summary>Ель: обычный коллайдер (не триггер), столкновение с игроком заканчивает игру.</summary>
public class TreeCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<PlayerMovement>() == null)
        {
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.State == GameState.Running)
        {
            AudioManager.Play(AudioManager.Sfx.TreeHit);
            GameManager.Instance.GameOver("столкновение с ёлкой");
        }
        else
        {
            Debug.LogWarning("На сцене нет GameManager, ёлка не может завершить игру");
        }
    }
}
