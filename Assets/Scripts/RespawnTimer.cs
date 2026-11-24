using System.Collections.Generic;
using UnityEngine;

public class RespawnTimer : MonoBehaviour
{
    private struct Entry
    {
        public GameObject target;
        public float remaining;
    }

    private static RespawnTimer instance;
    private readonly List<Entry> entries = new List<Entry>();

    public static void Schedule(GameObject target, float delay)
    {
        if (instance == null)
        {
            instance = new GameObject("RespawnTimer").AddComponent<RespawnTimer>();
        }

        instance.entries.Add(new Entry { target = target, remaining = delay });
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Running)
        {
            return;
        }

        for (int i = entries.Count - 1; i >= 0; i--)
        {
            Entry entry = entries[i];

            if (entry.target == null)
            {
                entries.RemoveAt(i);
                continue;
            }

            entry.remaining -= Time.deltaTime;

            if (entry.remaining > 0f)
            {
                entries[i] = entry;
                continue;
            }

            entry.target.SetActive(true);
            entries.RemoveAt(i);
        }
    }
}
