using UnityEngine;

public class LetterPickup : Collectible
{
    public PhraseProgress phrase;
    public int index;
    public TextMesh label;

    private void Update()
    {
        if (Camera.main == null)
        {
            return;
        }

        Vector3 toCamera = (Camera.main.transform.position - transform.position).normalized;
        label.transform.position = transform.position + toCamera * 0.9f;
        label.transform.rotation = Quaternion.LookRotation(-toCamera);
    }

    protected override void OnCollect(PlayerMovement player)
    {
        phrase.Collect(index);
    }
}
