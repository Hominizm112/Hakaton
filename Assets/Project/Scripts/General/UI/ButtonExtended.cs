using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Events;

public abstract class ButtonExtended : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, Tooltip("Берет компонент объекта, можно указать явно, какая кнопка нужна, тогда будет использовать её.")]
    protected Button _button;
    [SerializeField] protected ButtonExtendedSettings settings;
    [Header("Settings override")]
    [SerializeField] private bool spriteSwapDisable;
    [SerializeField] private bool textColorSwapDisable;


    [SerializeField] private bool mouseDownAnimation;
    [SerializeField, Tooltip("Use if ButtonExtended's creation dynamic")]
    private string mouseDownAnimatorName;
    [SerializeField] private TweenGraphRunner mouseDownAnimator;
    [SerializeField] private bool mouseUpAnimation;
    [SerializeField] private TweenGraphRunner mouseUpAnimator;
    [SerializeField, Tooltip("Use if ButtonExtended's creation dynamic")]
    private string mouseUpAnimatorName;


    [SerializeField] private UnityEvent OnMouseDown;
    [SerializeField] private UnityEvent OnMouseUp;

    public Action OnButtonClick;
    public Action OnButtonHold;

    private Image _image;
    private Sprite _initialSprite;
    private TMP_Text _tMP_Text;
    private Color _initialTextColor;
    private AudioHub _audioHub;

    private bool _isPointerDown = false;
    private bool _isHolding = false;
    private Coroutine _holdCoroutine;

    protected virtual void Start()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
        }
        OnButtonClick += OnClick;
        _button.onClick.AddListener(() => OnButtonClick?.Invoke());


        if (!spriteSwapDisable && settings.spriteSwap)
        {
            _image = GetComponent<Image>();
            _initialSprite = _image.sprite;
            _button.transition = Selectable.Transition.None;
        }

        if (!textColorSwapDisable && settings.textColorSwap)
        {
            _tMP_Text = transform.GetChild(0).GetComponent<TMP_Text>();
            _initialTextColor = _tMP_Text.color;
        }

        if (settings.soundType != SoundType.None)
        {
            _audioHub = Mediator.Instance.GetService<AudioHub>();
            if (_audioHub == null)
            {
                Mediator.Instance.GlobalEventBus.Publish(new DebugLogErrorEvent(("AudioHub service not found!", this).ToString()));
            }
        }



    }

    private void OnEnable()
    {
        if (mouseDownAnimation && mouseDownAnimator == null && !string.IsNullOrEmpty(mouseDownAnimatorName))
        {
            if (Mediator.Instance.TryGetService(out BaseGraphRunnerService service))
            {
                mouseDownAnimator = service.GetRunner(mouseDownAnimatorName);
            }
        }

        if (mouseUpAnimation && mouseUpAnimator == null && !string.IsNullOrEmpty(mouseUpAnimatorName))
        {
            if (Mediator.Instance.TryGetService(out BaseGraphRunnerService service))
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
        if (mouseDownAnimation)
        {
            mouseDownAnimator?.PlaySequence(gameObject);
            mouseUpAnimator?.StopSequence();
        }

        if (settings.holdDelay > 0)
        {
            _holdCoroutine = StartCoroutine(HoldCoroutine());
        }

        if (!spriteSwapDisable && settings.spriteSwap)
        {
            _image.sprite = settings.spriteToSwap;
        }

        if (!textColorSwapDisable && settings.textColorSwap)
        {
            _tMP_Text.color = settings.textColorToSwap;
        }
        if (_audioHub != null)
        {
            _audioHub.PlayOneShot(settings.soundType, 0.1f);
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        _isPointerDown = false;

        if (_isHolding)
        {
            if (_audioHub != null)
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

        if (!spriteSwapDisable && settings.spriteSwap)
        {
            _image.sprite = _initialSprite;
        }

        if (!textColorSwapDisable && settings.textColorSwap)
        {
            _tMP_Text.color = _initialTextColor;
        }

    }

    private IEnumerator HoldCoroutine()
    {
        do
        {
            yield return new WaitForSeconds(settings.holdDelay);

            if (_isPointerDown)
            {
                _isHolding = true;
                OnButtonHold?.Invoke();
                OnHold();
            }
        } while (_isHolding);
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
}
