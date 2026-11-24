using UnityEngine;

public static class HudSprites
{
    private static Sprite ring;
    private static Sprite disc;

    public static Sprite Ring => ring ??= Make(false);
    public static Sprite Disc => disc ??= Make(true);

    private static Sprite Make(bool filled)
    {
        const int size = 64;
        const float thickness = 7f;
        float radius = size * 0.5f - 2f;
        Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                float alpha = Mathf.Clamp01(radius - d + 0.5f);

                if (!filled)
                {
                    alpha *= Mathf.Clamp01(d - (radius - thickness) + 0.5f);
                }

                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };
        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
    }
}
