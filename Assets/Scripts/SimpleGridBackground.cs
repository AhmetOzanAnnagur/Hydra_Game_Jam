using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SimpleGridBackground : MonoBehaviour
{
    [Header("Texture")]
    public int texSize = 1024;          // 512/1024 is fine
    public int cellPixels = 32;         // square size in pixels
    public int lineThickness = 2;

    [Header("Colors")]
    public Color paperColor = new Color(0.92f, 0.96f, 1.00f, 1f);
    public Color lineColor  = new Color(0.75f, 0.82f, 0.92f, 1f);

    [Header("World")]
    public float pixelsPerUnit = 32f;   // pixel-art vibe
    public float worldWidth = 80f;      // how big the background is in world units

    void Start()
    {
        var sr = GetComponent<SpriteRenderer>();

        // Create texture
        Texture2D tex = new Texture2D(texSize, texSize, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;

        // Fill
        Color[] pixels = new Color[texSize * texSize];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = paperColor;

        // Draw grid lines
        for (int y = 0; y < texSize; y++)
        {
            for (int x = 0; x < texSize; x++)
            {
                bool vLine = (x % cellPixels) < lineThickness;
                bool hLine = (y % cellPixels) < lineThickness;

                if (vLine || hLine)
                    pixels[y * texSize + x] = lineColor;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        // Make sprite from texture
        Sprite sprite = Sprite.Create(
            tex,
            new Rect(0, 0, texSize, texSize),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit
        );

        sr.sprite = sprite;

        // Scale it big in the world
        float spriteWorldSize = texSize / pixelsPerUnit; // width in world units
        float scale = worldWidth / spriteWorldSize;
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
