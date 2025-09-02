using UnityEngine;
using UnityEngine.UI;

public class SliderSfx : MonoBehaviour
{
    [SerializeField, Tooltip("Sound to play when button is pressed")]
    private SoundType _soundType;
    private Slider _slider;
    private AudioHub _audioHub;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        if (_slider == null)
        {
            Debug.LogError("Button component not found!", this);
            return;
        }

        _slider.onValueChanged.AddListener((float f) => OnValueChanged());

        _audioHub = Mediator.Instance.GetService<AudioHub>();
        if (_audioHub == null)
        {
            Debug.LogError("AudioHub service not found!", this);
        }
    }

    private void OnValueChanged()
    {
        if (_audioHub != null)
        {
            _audioHub.PlayOneShot(_soundType);
        }
    }

    private void OnDestroy()
    {
        if (_slider != null)
        {
            _slider.onValueChanged.RemoveListener((float f) => OnValueChanged());
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (GetComponent<Slider>() == null)
        {
            Debug.LogWarning("SliderSfx requires a Slider component!", this);
        }
    }
#endif
}
