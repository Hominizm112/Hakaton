using UnityEngine;
using UnityEngine.UI;

public class ButtonSfx : MonoBehaviour
{
    [SerializeField, Tooltip("Sound to play when button is pressed")]
    private SoundType _soundType;
    private Button _button;
    private AudioHub _audioHub;

    private void Awake()
    {
        _button = GetComponent<Button>();
        if (_button == null)
        {
            Debug.LogError("Button component not found!", this);
            return;
        }

        _button.onClick.AddListener(OnButtonClicked);

        _audioHub = Mediator.Instance.GetService<AudioHub>();
        if (_audioHub == null)
        {
            Debug.LogError("AudioHub service not found!", this);
        }
    }

    private void OnButtonClicked()
    {
        if (_audioHub != null)
        {
            _audioHub.PlayOneShot(_soundType);
        }
    }

    private void OnDestroy()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (GetComponent<Button>() == null)
        {
            Debug.LogWarning("ButtonSfx requires a Button component!", this);
        }
    }
#endif
}
