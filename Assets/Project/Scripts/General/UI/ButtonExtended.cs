using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

[Serializable]
public struct HoldSettings
{
    public float time;
    public UnityEvent unityEvent;
}
public abstract class ButtonExtended : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, Tooltip("Берет компонент объекта, можно указать явно, какая кнопка нужна, тогда будет использовать её.")]
    protected Button _button;
    [SerializeField] public ButtonExtendedSettings settings;
    [Header("Settings override")]
    [SerializeField] private bool spriteSwapOverride;
    [SerializeField] private Sprite spriteOverride;
    [SerializeField] private bool textColorSwapOverride;
    [SerializeField] private Color textColorOverride;

    [Header("Animation Settings")]
    [SerializeField] private bool mouseDownAnimation;
    [SerializeField, Tooltip("Use if ButtonExtended's creation dynamic")]
    private string mouseDownAnimatorName;
    [SerializeField] private TweenGraphRunner mouseDownAnimator;
    [Space(5)]
    [SerializeField] private bool mouseUpAnimation;
    [SerializeField] private TweenGraphRunner mouseUpAnimator;
    [SerializeField, Tooltip("Use if ButtonExtended's creation dynamic")]
    private string mouseUpAnimatorName;


    [SerializeField] private UnityEvent OnMouseDown;
    [SerializeField] private UnityEvent OnMouseUp;
    [SerializeField] private List<HoldSettings> holdSettings;

    public Action OnButtonClick;
    public Action<ButtonExtended> OnMouseDownWithReference;
    public Action<float> OnButtonHold;

    private Image _image;
    private Sprite _initialSprite;
    private TMP_Text _tMP_Text;
    private Color _initialTextColor;
    private AudioHub _audioHub;

    private bool _isPointerDown = false;
    private bool _isHolding = false;
    private Coroutine _holdCoroutine;

    public bool SpriteSwapOverride => spriteSwapOverride;
    public bool TextColorSwapOverride => textColorSwapOverride;
    public Sprite OverrideSprite => spriteOverride;
    public Color OverrideTextColor => textColorOverride;

    private bool UseSpriteSwap => spriteSwapOverride || (settings != null ? settings.spriteToSwap : null);
    private Sprite TargetSprite => spriteSwapOverride ? spriteOverride : (settings != null ? settings.spriteToSwap : null);
    private bool UseTextColorSwap => textColorSwapOverride || (settings != null && settings.textColorSwap);
    private Color TargetTextColor => textColorSwapOverride ? textColorOverride : (settings != null ? settings.textColorToSwap : Color.white);

    protected virtual void Start()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
        }
        _button.onClick.AddListener(HandleButtonClick);

        if (UseSpriteSwap)
        {
            _image = GetComponent<Image>();
            if (_image != null)
            {
                _initialSprite = _image.sprite;
                _button.transition = Selectable.Transition.None;
            }
        }


        if (UseTextColorSwap)
        {
            _tMP_Text = GetComponentInChildren<TMP_Text>();
            if (_tMP_Text != null)
            {
                _initialTextColor = _tMP_Text.color;
            }
        }

        if (!spriteSwapOverride && (settings.spriteSwap || spriteOverride))
        {
            _image = GetComponent<Image>();
            _initialSprite = _image.sprite;
            _button.transition = Selectable.Transition.None;
        }

        if (settings != null && settings.soundType != SoundType.None)
        {
            _audioHub = Mediator.Instance?.GetService<AudioHub>();
            if (_audioHub == null)
            {
                Mediator.Instance?.GlobalEventBus?.Publish(new DebugLogErrorEvent(("AudioHub service not found!", this).ToString()));
            }
        }


    }

    private void HandleButtonClick()
    {
        print("click");
        OnClick();
        OnButtonClick?.Invoke();
    }

    private void OnEnable()
    {
        if (mouseDownAnimation && mouseDownAnimator == null && !string.IsNullOrEmpty(mouseDownAnimatorName))
        {
            if (Mediator.Instance?.TryGetService(out BaseGraphRunnerService service) == true)
            {
                mouseDownAnimator = service.GetRunner(mouseDownAnimatorName);
            }
        }

        if (mouseUpAnimation && mouseUpAnimator == null && !string.IsNullOrEmpty(mouseUpAnimatorName))
        {
            if (Mediator.Instance?.TryGetService(out BaseGraphRunnerService service) == true)
            {
                mouseUpAnimator = service.GetRunner(mouseUpAnimatorName);
            }
        }
    }



    protected virtual void OnClick()
    {
        HandleClick();
    }

    protected virtual void OnHold()
    {
        if (_audioHub != null && settings.holdSoundType != SoundType.None)
        {
            _audioHub.PlayOneShot(settings.holdSoundType, 0.1f);
        }
        HandleHold();
    }

    protected abstract void HandleClick();
    protected abstract void HandleHold();

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        _isPointerDown = true;
        OnMouseDown?.Invoke();
        OnMouseDownWithReference?.Invoke(this);

        if (mouseDownAnimation)
        {
            mouseDownAnimator?.PlaySequence(gameObject);
            mouseUpAnimator?.StopSequence();
        }

        if (settings != null && settings.holdDelay > 0)
        {
            _holdCoroutine = StartCoroutine(HoldCoroutine());
        }

        if (UseSpriteSwap && _image != null && TargetSprite != null)
        {
            _image.sprite = TargetSprite;
        }

        if (UseTextColorSwap && _tMP_Text != null)
        {
            _tMP_Text.color = TargetTextColor;
        }

        if (_audioHub != null && settings != null)
        {
            _audioHub.PlayOneShot(settings.soundType, 0.1f);
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        _isPointerDown = false;

        if (_isHolding)
        {
            if (_audioHub != null && settings != null)
            {
                _audioHub.PlayOneShot(settings.soundType, 0.1f);
            }
        }
        _isHolding = false;
        OnMouseUp?.Invoke();

        if (mouseUpAnimation)
        {
            mouseUpAnimator?.PlaySequence(gameObject);
            mouseDownAnimator?.StopSequence();
        }

        if (_holdCoroutine != null)
        {
            StopCoroutine(_holdCoroutine);
            _holdCoroutine = null;
        }

        if (UseSpriteSwap && _image != null && _initialSprite != null)
        {
            _image.sprite = _initialSprite;
        }

        if (UseTextColorSwap && _tMP_Text != null)
        {
            _tMP_Text.color = _initialTextColor;
        }

    }

    private IEnumerator HoldCoroutine()
    {
        float timeHolding = 0;
        List<HoldSettings> holdSettingsContained = new(holdSettings);
        do
        {
            yield return new WaitForSeconds(settings.holdDelay);

            if (_isPointerDown)
            {
                _isHolding = true;
                OnButtonHold?.Invoke(timeHolding);
                OnHold();
                timeHolding += Time.deltaTime + settings.holdDelay;
                for (int i = holdSettingsContained.Count - 1; i >= 0; i--)
                {
                    if (timeHolding >= holdSettingsContained[i].time)
                    {
                        holdSettingsContained[i].unityEvent?.Invoke();
                        holdSettingsContained.RemoveAt(i);
                    }
                }
            }
        } while (_isHolding);
    }


    public void SetSpriteOverride(Sprite newSprite, bool enableOverride = true)
    {
        spriteSwapOverride = enableOverride;
        spriteOverride = newSprite;
    }

    public void SetTextColorOverride(Color newColor, bool enableOverride = true)
    {
        textColorSwapOverride = enableOverride;
        textColorOverride = newColor;
    }

    public void DisableSpriteOverride()
    {
        spriteSwapOverride = false;
    }

    public void DisableTextColorOverride()
    {
        textColorSwapOverride = false;
    }


    protected virtual void OnDestroy()
    {
        _button?.onClick.RemoveAllListeners();
        OnButtonHold = null;

        if (_holdCoroutine != null)
        {
            StopCoroutine(_holdCoroutine);
        }
    }

    public void Print(string message)
    {
        print(message);
    }
}
