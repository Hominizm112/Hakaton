using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TextSizeFitter : MonoBehaviour
{
    public RectTransform rectTransform;
    public TMP_Text text;
    public float maxLength = 100f;

    public void OnValidate()
    {
        if (rectTransform != null && text != null)
        {
            UpdateSize();
        }
    }


    public void UpdateSize()
    {
        if (rectTransform != null && text != null)
        {
            Vector2 preferredSize = text.GetPreferredValues();
            rectTransform.sizeDelta = new Vector2(Mathf.Min(preferredSize.x, maxLength), rectTransform.sizeDelta.y);
        }
    }

    private void Start()
    {
        UpdateSize();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UpdateSize();
        }
#endif
    }
}
