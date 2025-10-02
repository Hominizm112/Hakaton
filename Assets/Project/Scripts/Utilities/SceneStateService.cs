using UnityEngine;
using UnityEngine.Events;

public class SceneStateService : MonoBehaviour
{
    [SerializeField] public UnityEvent OnSceneStart;
    [SerializeField] public UnityEvent OnSceneAwake;

    void Start()
    {
        OnSceneStart?.Invoke();
    }

    void Awake()
    {
        OnSceneAwake?.Invoke();
    }
}
