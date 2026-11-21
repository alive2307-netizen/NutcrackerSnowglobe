using UnityEngine;

/// <summary>
/// Заглушка звука (этап 13). Клипы не назначены, поэтому Play ничего не делает.
/// Треки и лицензии перечислены в Assets/Audio/TRACKS.md.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public enum Sfx
    {
        Pickup,
        Shot,
        Hurt,
        Splat,
        Win,
        Lose
    }

    private static AudioManager instance;

    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip roundMusic;

    [Header("Sound effects")]
    public AudioClip pickupClip;
    public AudioClip shotClip;
    public AudioClip hurtClip;
    public AudioClip splatClip;
    public AudioClip winClip;
    public AudioClip loseClip;

    private AudioSource sfxSource;

    private void Awake()
    {
        instance = this;
        sfxSource = gameObject.AddComponent<AudioSource>();
    }

    public static void Play(Sfx effect)
    {
        if (instance == null)
        {
            return;
        }

        AudioClip clip = instance.ClipFor(effect);

        if (clip != null)
        {
            instance.sfxSource.PlayOneShot(clip);
        }
    }

    private AudioClip ClipFor(Sfx effect)
    {
        switch (effect)
        {
            case Sfx.Pickup: return pickupClip;
            case Sfx.Shot: return shotClip;
            case Sfx.Hurt: return hurtClip;
            case Sfx.Splat: return splatClip;
            case Sfx.Win: return winClip;
            default: return loseClip;
        }
    }
}
