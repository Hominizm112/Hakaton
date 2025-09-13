using UnityEngine;

public class ObjectDisabler : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }
}
