using UnityEngine;
using DG.Tweening;

public class Plants : MonoBehaviour, IInteractable, IStateListener
{
    [SerializeField] private PlantsAnimationSettings _animationSettings;
    private bool _isActive = false;
    private Sequence _dropSequence; 
    private bool _isSubscribed = false;
    private Mediator _mediator;

    public void Start()
    {
        SubscribeToGameState();
        CheckCurrentState();
    }

    private void SubscribeToGameState()
    {
        if (!_isSubscribed)
        {
            Game.GamesStateChanged += OnStateChanged;
            _isSubscribed = true;
        }
    }

    private void UnsubscribeFromGameState()
    {
        if (_isSubscribed)
        {
            Game.GamesStateChanged -= OnStateChanged;
            _isSubscribed = false;
        }
    }

    private void CheckCurrentState()
    {
        if (Game.ActualState == Game.State.NightScene)
        {
            StartPeriodicAction();
        }
    }

    public void Interact()
    {
        SpawnLeaf();
    }

    void SpawnLeaf()
    {
        if (_animationSettings.leafPrefab == null)
        {
             _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Leaf Prefab не назначен в ScriptableObject Animation Settings!"));
            return;
        }
        GameObject newLeaf = Instantiate(_animationSettings.leafPrefab, _animationSettings.dropLeavesPoint, Quaternion.identity);
        LeafController leafController = newLeaf.GetComponent<LeafController>();
        if (leafController != null)
        {
            leafController.DropDistance = _animationSettings.dropDistance;
            leafController.DropDuration = _animationSettings.dropDuration;
            leafController.StartLeafAnimation();
        }
    }

    private void StartPeriodicAction()
    {
        if (_isActive) return;

        _dropSequence = DOTween.Sequence()
            .AppendInterval(_animationSettings.actionInterval)
            .AppendCallback(Interact)
            .SetLoops(-1);
        _isActive = true;
    }

    public void StopPeriodicAction()
    {
        if (_dropSequence != null)
        {
            _dropSequence.Kill();
            _dropSequence = null;
        }
        _isActive = false;
    }

    public void OnStateChanged(Game.State newState)
    {
        if (newState == Game.State.NightScene)
        {
            StartPeriodicAction();
        }
        else
        {
            StopPeriodicAction();
        }
    }

    void OnDestroy()
    {
        UnsubscribeFromGameState();
        StopPeriodicAction();
    }
}