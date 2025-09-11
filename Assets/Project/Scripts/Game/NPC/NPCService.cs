using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NPCService : MonoService, IInitializable
{
    public override List<Type> requiredServices { get; protected set; } = new List<Type> { typeof(EmotionIndicator) };
    [SerializeField] private List<Emotion> emotions;
    [Header("Animation Settings")]
    [SerializeField] private float moveInDuration = 1.2f;
    [SerializeField] private Ease moveInEase = Ease.OutBack;
    [SerializeField] private float moveOutDuration = 1f;
    [SerializeField] private Ease moveOutEase = Ease.InBack;
    [SerializeField] private float screenPadding = 1.5f;



    public NPC npc;
    public TeaCommodity teaCommodity;
    private GameObject _activeNpc;
    private SpeechBubble _activeSpeechBubble;
    private EmotionIndicator _emotionIndicator;
    private Camera _mainCamera;
    private Vector3 _targetPosition;
    private Mediator _mediator;
    private Sequence _currentAnimation;

    public void Initialize(Mediator mediator)
    {
        _mediator = Mediator.Instance;
        mediator.RegisterService(this);
        _mainCamera = Camera.main;

    }

    protected override void OnAllServicesReady()
    {
        _emotionIndicator = GetService<EmotionIndicator>();
        if (_emotionIndicator != null)
        {
            CreateNPC();
        }
        else
        {
            Debug.LogError("EmotionIndicator service not found!");
        }
    }

    public void CreateNPC()
    {
        _activeNpc = Instantiate(npc.npcPrefab);
        _activeSpeechBubble = _activeNpc.GetComponent<SpeechBubble>();

        SetupNPCPosition();
        AnimateNPCIn();


    }

    #region  Animation

    private void SetupNPCPosition()
    {
        if (_mainCamera == null) return;

        Vector3 rightEdge = _mainCamera.ViewportToWorldPoint(new Vector3(1.1f, 0.5f, Mathf.Abs(_mainCamera.transform.position.z)));
        _targetPosition = _mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Mathf.Abs(_mainCamera.transform.position.z)));

        rightEdge.z = 0;
        _targetPosition.z = 0;

        Vector3 startPosition = new Vector3(rightEdge.x * screenPadding, _targetPosition.y, 0);
        _activeNpc.transform.position = startPosition;

    }

    private void AnimateNPCIn()
    {
        _currentAnimation?.Kill();

        _currentAnimation = DOTween.Sequence();

        _currentAnimation.Append(_activeNpc.transform.DOMove(_targetPosition, moveInDuration)
            .SetEase(moveInEase));


        _currentAnimation.OnComplete(() =>
        {
            _currentAnimation = null;
            StartDialogue();

        });

        _currentAnimation.Play();
    }

    private void AnimateNPCOut(Action onComplete = null)
    {
        if (_mainCamera == null || _activeNpc == null) return;

        Vector3 rightEdge = _mainCamera.ViewportToWorldPoint(new Vector3(1.1f, 0.5f, Mathf.Abs(_mainCamera.transform.position.z)));
        Vector3 exitPosition = new Vector3(-rightEdge.x * screenPadding, _targetPosition.y, 0);

        _currentAnimation?.Kill();
        _currentAnimation = DOTween.Sequence();

        _currentAnimation.Append(_activeNpc.transform.DOMove(exitPosition, moveOutDuration)
            .SetEase(moveOutEase));


        _currentAnimation.OnComplete(() =>
        {
            onComplete?.Invoke();
            _currentAnimation = null;
        });

        _currentAnimation.Play();
    }

    public void RemoveNPC(bool animate = true)
    {
        if (_activeNpc == null) return;

        if (animate)
        {
            AnimateNPCOut(() =>
            {
                Destroy(_activeNpc);
                _activeNpc = null;
                CreateNPC();
            });
        }
        else
        {
            Destroy(_activeNpc);
            _activeNpc = null;
        }
    }

    #endregion

    #region  Dialogue
    private void StartDialogue()
    {
        _activeSpeechBubble.OnNextLineRequested = null;
        _activeSpeechBubble.SetText(npc.SmallTalkLine);
        _activeSpeechBubble.OnNextLineRequested += ShowRequest;
    }

    private void ShowRequest()
    {
        _activeSpeechBubble.OnNextLineRequested = null;
        _activeSpeechBubble.SetText(npc.RequestLine);
        _activeSpeechBubble.OnNextLineRequested += HandleBuyResult;
    }

    private void HandleBuyResult()
    {
        _emotionIndicator.OnComplete += () => RemoveNPC();
        npc.BuyTea(teaCommodity, HandleItemBought);

        _activeSpeechBubble.OnNextLineRequested = null;
    }

    private void HandleItemBought(NPCBuyResult buyResult)
    {
        _activeSpeechBubble.SetText(buyResult.dialogueLine);
        _emotionIndicator.ShowEmotion(GetEmotionSprite(buyResult.satisfaction));
    }

    #endregion

    private Sprite GetEmotionSprite(NPCBuySatisfaction buySatisfaction)
    {
        return emotions.Find((r) => r.buySatisfaction == buySatisfaction).sprite;
    }

    public void HandleMoveInDialogue()
    {
        _activeSpeechBubble?.Interact();
    }

    private void OnDestroy()
    {
        _currentAnimation?.Kill();

        if (_activeNpc != null)
        {
            _activeNpc.transform.DOKill();
        }
    }

}
