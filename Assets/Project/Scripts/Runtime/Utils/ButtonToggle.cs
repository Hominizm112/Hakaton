using UnityEngine;
using UnityEngine.UI;

public class ButtonToggle : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Image image;
    public Sprite spriteFirst;
    public Sprite spriteSecond;
    private bool toggle;

    public void Toggle()
    {
        toggle = !toggle;

        HandleSpriteChange();
    }

    private void HandleSpriteChange()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = toggle ? spriteFirst : spriteSecond;
        }

        if (image != null)
        {
            image.sprite = toggle ? spriteFirst : spriteSecond;
        }

    }
}
