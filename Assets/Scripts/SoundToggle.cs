using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour
{
    public enum Kind
    {
        Music,
        Sfx
    }

    private const float KnobOffset = 57f;

    public Kind kind;
    public Image track;
    public RectTransform knob;
    public Text label;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Toggle);
    }

    private void Update()
    {
        bool on = IsOn();
        string title = kind == Kind.Music ? "Музыка" : "Эффекты";
        label.text = title + ": " + (on ? "вкл" : "выкл");
        track.color = on ? new Color(0.3f, 0.8f, 0.4f, 1f) : new Color(0.35f, 0.36f, 0.42f, 1f);

        Vector2 pos = knob.anchoredPosition;
        pos.x = Mathf.Lerp(pos.x, on ? KnobOffset : -KnobOffset, 12f * Time.deltaTime);
        knob.anchoredPosition = pos;
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

    private bool IsOn()
    {
        return kind == Kind.Music ? AudioManager.MusicOn : AudioManager.SfxOn;
    }
}
