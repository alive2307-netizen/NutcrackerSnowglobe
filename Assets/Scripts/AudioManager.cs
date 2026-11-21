using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum Sfx
    {
        Click,
        Pickup,
        Shot,
        Splat,
        Hurt,
        Win,
        Lose
    }

    public enum MusicTrack
    {
        Menu,
        Round,
        Win,
        Lose
    }

    private static AudioManager instance;

    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip roundMusic;
    public AudioClip winMusic;
    public AudioClip loseMusic;

    [Header("Sound effects")]
    public AudioClip clickSound;
    public AudioClip pickupSound;
    public AudioClip shotSound;
    public AudioClip splatSound;
    public AudioClip hurtSound;
    public AudioClip winSound;
    public AudioClip loseSound;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        instance = this;
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
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

    public static void PlayMusic(MusicTrack track)
    {
        if (instance == null)
        {
            return;
        }

        AudioClip clip = instance.ClipFor(track);

        if (clip == null || instance.musicSource.clip == clip)
        {
            return;
        }

        instance.musicSource.clip = clip;
        instance.musicSource.Play();
    }

    private AudioClip ClipFor(Sfx effect)
    {
        switch (effect)
        {
            case Sfx.Click: return clickSound;
            case Sfx.Pickup: return pickupSound;
            case Sfx.Shot: return shotSound;
            case Sfx.Splat: return splatSound;
            case Sfx.Hurt: return hurtSound;
            case Sfx.Win: return winSound;
            default: return loseSound;
        }
    }

    private AudioClip ClipFor(MusicTrack track)
    {
        switch (track)
        {
            case MusicTrack.Menu: return menuMusic;
            case MusicTrack.Round: return roundMusic;
            case MusicTrack.Win: return winMusic;
            default: return loseMusic;
        }
    }
}
