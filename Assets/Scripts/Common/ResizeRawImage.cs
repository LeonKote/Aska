using UnityEngine;
using UnityEngine.UI;

public class ResizeRawImage : MonoBehaviour
{
    public float maxWidth;
    public float maxHeight;

    private RawImage rawImage;

    public void AdjustSize()
    {
        if (rawImage == null)
            rawImage = GetComponent<RawImage>();

        Texture texture = rawImage.texture;
        if (texture == null)
            return;

        int textureWidth = texture.width;
        int textureHeight = texture.height;

        float newWidth, newHeight;

        if (textureWidth > maxWidth || textureHeight > maxHeight)
        {
            float widthRatio = maxWidth / textureWidth;
            float heightRatio = maxHeight / textureHeight;
            float scaleFactor = Mathf.Min(widthRatio, heightRatio);

            newWidth = textureWidth * scaleFactor;
            newHeight = textureHeight * scaleFactor;
        }
        else
        {
            newWidth = textureWidth;
            newHeight = textureHeight;
        }

        rawImage.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
    }
}