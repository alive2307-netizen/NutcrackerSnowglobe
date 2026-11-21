using System.Collections.Generic;
using UnityEngine;

/// <summary>Возвращает исчезнувших мышей на трассу с противоположной стороны шара, вне камеры.</summary>
public class MouseRespawner : MonoBehaviour
{
    [Tooltip("Через сколько секунд исчезнувшая мышь появится снова.")]
    public float respawnDelay = 8f;

    public static MouseRespawner Instance { get; private set; }

    private struct Pending
    {
        public MouseEnemy mouse;
        public float readyAt;
    }

    private readonly List<Pending> pending = new List<Pending>();

    private void Awake()
    {
        Instance = this;
    }

    public void Schedule(MouseEnemy mouse)
    {
        pending.Add(new Pending { mouse = mouse, readyAt = Time.time + respawnDelay });
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Running)
        {
            return;
        }

        for (int i = pending.Count - 1; i >= 0; i--)
        {
            if (Time.time < pending[i].readyAt)
            {
                continue;
            }

            MouseEnemy mouse = pending[i].mouse;
            Vector3 playerDirection = (GameManager.Instance.player.transform.position - mouse.planet.position).normalized;
            mouse.Respawn(playerDirection);
            pending.RemoveAt(i);
        }
    }
}
