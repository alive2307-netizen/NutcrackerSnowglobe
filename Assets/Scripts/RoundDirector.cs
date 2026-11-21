using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Следит за состоянием раунда: меняет музыку, играет звук выигрыша или поражения
/// и показывает финал. Победа - конфетти над камерой, поражение - мыши, бегущие по экрану.
/// </summary>
public class RoundDirector : MonoBehaviour
{
    private const int MouseCount = 10;
    private const float HalfWidth = 620f;
    private const float ConfettiBurst = 160;

    public GameManager game;
    public RectTransform canvasRect;
    public Sprite mouseSprite;
    public Transform cameraTransform;

    private GameState lastState = (GameState)(-1);
    private ParticleSystem confetti;
    private RectTransform miceRoot;
    private RectTransform[] mice;
    private float[] miceSpeeds;

    private void Awake()
    {
        CreateConfetti();
        CreateMice();
    }

    private void Update()
    {
        if (game.State != lastState)
        {
            lastState = game.State;
            OnStateChanged(lastState);
        }

        MoveMice();
    }

    private void OnStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                AudioManager.PlayMusic(AudioManager.MusicTrack.Menu);
                miceRoot.gameObject.SetActive(false);
                break;

            case GameState.Running:
                AudioManager.Play(AudioManager.Sfx.Click);
                AudioManager.PlayMusic(AudioManager.MusicTrack.Round);
                miceRoot.gameObject.SetActive(false);
                break;

            case GameState.Win:
                AudioManager.PlayMusic(AudioManager.MusicTrack.Win);
                AudioManager.Play(AudioManager.Sfx.Win);
                confetti.Play();
                confetti.Emit((int)ConfettiBurst);
                break;

            case GameState.Lose:
                AudioManager.PlayMusic(AudioManager.MusicTrack.Lose);
                AudioManager.Play(AudioManager.Sfx.Lose);
                miceRoot.gameObject.SetActive(true);
                break;
        }
    }

    private void CreateConfetti()
    {
        GameObject go = new GameObject("Confetti");
        go.transform.SetParent(cameraTransform, false);
        go.transform.localPosition = new Vector3(0f, 2.5f, 9f);

        confetti = go.AddComponent<ParticleSystem>();
        confetti.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        ParticleSystem.MainModule main = confetti.main;
        main.playOnAwake = false;
        main.loop = false;
        main.duration = 4f;
        main.startLifetime = 4f;
        main.startSpeed = 2f;
        main.startSize = 0.3f;
        main.gravityModifier = 0.5f;
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.3f, 0.3f), new Color(0.3f, 1f, 0.5f));

        ParticleSystem.ShapeModule shape = confetti.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(14f, 0.5f, 1f);

        ParticleSystem.EmissionModule emission = confetti.emission;
        emission.enabled = false;
    }

    private void CreateMice()
    {
        GameObject root = new GameObject("RunningMice", typeof(RectTransform));
        root.transform.SetParent(canvasRect, false);

        miceRoot = (RectTransform)root.transform;
        miceRoot.anchorMin = Vector2.zero;
        miceRoot.anchorMax = Vector2.one;
        miceRoot.offsetMin = Vector2.zero;
        miceRoot.offsetMax = Vector2.zero;

        mice = new RectTransform[MouseCount];
        miceSpeeds = new float[MouseCount];

        for (int i = 0; i < MouseCount; i++)
        {
            GameObject m = new GameObject("Mouse", typeof(RectTransform), typeof(Image));
            m.transform.SetParent(miceRoot, false);

            RectTransform rect = (RectTransform)m.transform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(110f, 64f);
            rect.anchoredPosition = new Vector2(Random.Range(-HalfWidth, HalfWidth), Random.Range(-760f, -220f));

            Image image = m.GetComponent<Image>();
            image.sprite = mouseSprite;
            image.color = new Color(0.55f, 0.55f, 0.6f);
            image.raycastTarget = false;

            mice[i] = rect;
            miceSpeeds[i] = Random.Range(260f, 520f);
        }

        miceRoot.gameObject.SetActive(false);
    }

    private void MoveMice()
    {
        if (!miceRoot.gameObject.activeSelf)
        {
            return;
        }

        for (int i = 0; i < mice.Length; i++)
        {
            Vector2 pos = mice[i].anchoredPosition;
            pos.x += miceSpeeds[i] * Time.deltaTime;

            if (pos.x > HalfWidth + 80f)
            {
                pos.x = -HalfWidth - 80f;
            }

            mice[i].anchoredPosition = pos;
        }
    }
}
