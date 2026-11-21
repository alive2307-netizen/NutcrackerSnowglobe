using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour
{
    public enum Kind
    {
        Music,
        Sfx
    }

    public Kind kind;
    public Text label;

    private void Update()
    {
        bool on = kind == Kind.Music ? AudioManager.MusicOn : AudioManager.SfxOn;
        string title = kind == Kind.Music ? "Музыка" : "Эффекты";
        label.text = title + ": " + (on ? "вкл" : "выкл");
    }

    public void Toggle()
    {
        if (kind == Kind.Music)
        {
            AudioManager.ToggleMusic();
        }
        else
        {
            AudioManager.ToggleSfx();
        }
    }
}
