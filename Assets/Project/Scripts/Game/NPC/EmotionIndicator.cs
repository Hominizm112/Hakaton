using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class EmotionIndicator : MonoService
{
    [SerializeField] private GameObject emotionObject;
    [SerializeField] private AnimationSettings animationSettings;
    [SerializeField] private float verticalOffset = -0.5f;

    private SpriteRenderer spriteRenderer;
    private Mediator _mediator;
    private Timer timer = new();

    public Action OnComplete;


    public void Awake()
    {
        _mediator = Mediator.Instance;
        _mediator.RegisterService(this);

        spriteRenderer = emotionObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning($"EmotionIndicator can't find SpriteRenderer connected to object {emotionObject.name}.");
        }

        emotionObject.SetActive(false);

    }

    public void ShowEmotion(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        emotionObject.SetActive(true);
        HandleAnimation();



        timer.OnTimerCompleted += HideEmotion;
        timer.Start(animationSettings.duration);

    }

    private void HandleAnimation()
    {
        Vector3 initialPos = emotionObject.transform.position;
        spriteRenderer.color = new(1, 1, 1, 0);
        spriteRenderer.DOFade(1, animationSettings.duration * 2 / 3).
            SetEase(Ease.Linear).
            OnComplete(() => spriteRenderer.DOFade(0, animationSettings.duration / 3).SetEase(Ease.Linear));
        emotionObject.transform.Translate(Vector3.up * verticalOffset);
        emotionObject.transform.DOMove(initialPos, animationSettings.duration * 2 / 3).SetEase(animationSettings.ease);
    }

    private void HideEmotion()
    {
        OnComplete?.Invoke();
        emotionObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _mediator.UnregisterService(this);
        OnComplete = null;
        timer.Dispose();
    }


}

[Serializable]
public struct Emotion
{
    public NPCBuySatisfaction buySatisfaction;
    public Sprite sprite;
}