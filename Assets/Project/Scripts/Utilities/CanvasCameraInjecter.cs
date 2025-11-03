using UnityEngine;

public class CanvasCameraInjecter : MonoBehaviour
{
    public bool SetOverrideSorting;
    void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        canvas.overrideSorting = SetOverrideSorting;
    }


}
