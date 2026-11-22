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
        Lose,
        Bite,
        SnowHit,
        ToyPickup,
        TinselPickup,
        GarlandPickup,
        TreeHit
    }

    public enum MusicTrack
    {
        Menu,
        Round,
        Win,
        Lose
    }

    private const string MusicKey = "music_on";
    private const string SfxKey = "sfx_on";

    private static AudioManager instance;

    public static bool MusicOn { get; private set; } = true;
    public static bool SfxOn { get; private set; } = true;

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
    public AudioClip biteSound;
    public AudioClip snowHitSound;
    public AudioClip toyPickupSound;
    public AudioClip tinselPickupSound;
    public AudioClip garlandPickupSound;
    public AudioClip treeHitSound;

    [Header("Looping")]
    public AudioClip skateClip;
    public AudioClip gnawClip;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioSource skateSource;
    private AudioSource gnawSource;

    private void Awake()
    {
        instance = this;
        MusicOn = PlayerPrefs.GetInt(MusicKey, 1) == 1;
        SfxOn = PlayerPrefs.GetInt(SfxKey, 1) == 1;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.mute = !MusicOn;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.mute = !SfxOn;

        skateSource = gameObject.AddComponent<AudioSource>();
        skateSource.clip = skateClip;
        skateSource.loop = true;
        skateSource.volume = 0.6f;
        skateSource.mute = !SfxOn;

        gnawSource = gameObject.AddComponent<AudioSource>();
        gnawSource.clip = gnawClip;
        gnawSource.loop = true;
        gnawSource.volume = 0.7f;
        gnawSource.mute = !SfxOn;
    }

    public static void ToggleMusic()
    {
        MusicOn = !MusicOn;
        PlayerPrefs.SetInt(MusicKey, MusicOn ? 1 : 0);

        if (instance != null)
        {
            instance.musicSource.mute = !MusicOn;
        }
    }

    public static void ToggleSfx()
    {
        SfxOn = !SfxOn;
        PlayerPrefs.SetInt(SfxKey, SfxOn ? 1 : 0);

        if (instance != null)
        {
            instance.sfxSource.mute = !SfxOn;
            instance.skateSource.mute = !SfxOn;
            instance.gnawSource.mute = !SfxOn;
        }
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

    public static void SetSkating(bool on)
    {
        if (instance == null || instance.skateClip == null)
        {
            return;
        }

        if (on && !instance.skateSource.isPlaying)
        {
            instance.skateSource.Play();
        }
        else if (!on && instance.skateSource.isPlaying)
        {
            instance.skateSource.Stop();
        }
    }

    public static void SetGnawing(bool on)
    {
        if (instance == null || instance.gnawClip == null)
        {
            return;
        }

        if (on && !instance.gnawSource.isPlaying)
        {
            instance.gnawSource.Play();
        }
        else if (!on && instance.gnawSource.isPlaying)
        {
            instance.gnawSource.Stop();
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
            case Sfx.Bite: return biteSound;
            case Sfx.SnowHit: return snowHitSound;
            case Sfx.ToyPickup: return toyPickupSound;
            case Sfx.TinselPickup: return tinselPickupSound;
            case Sfx.GarlandPickup: return garlandPickupSound;
            case Sfx.TreeHit: return treeHitSound;
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
