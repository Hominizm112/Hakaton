using UnityEngine;
using DG.Tweening;

public class Plants : MonoBehaviour, IInteractable, IStateListener
{
    [SerializeField] private PlantsAnimationSettings _animationSettings;
    private bool _isActive = false;
    private Sequence _dropSequence; 
    private bool _isSubscribed = false;

    public void Start()
    {
        SubscribeToGameState();
        CheckCurrentState();
        Debug.Log("Start() вызван");
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
         Debug.Log("Interact() вызван");
    }

    void SpawnLeaf()
    {
       Debug.Log("SpawnLeaf() вызван");
        if (_animationSettings.leafPrefab == null)
        {
            Debug.LogError("Leaf Prefab не назначен в ScriptableObject Animation Settings!");
            return;
        }
        GameObject newLeaf = Instantiate(_animationSettings.leafPrefab, _animationSettings.dropLeavesPoint, Quaternion.identity);

        LeafController leafController = newLeaf.GetComponent<LeafController>();

        if (leafController != null)
        {
            // 1. Передаем настройки анимации листу.
            leafController.DropDistance = _animationSettings.dropDistance;
            leafController.DropDuration = _animationSettings.dropDuration;

            // 2. Теперь, когда данные переданы, запускаем анимацию.
            leafController.StartLeafAnimation();
        }
    }

    private void StartPeriodicAction()
    {
        if (_isActive) return;

        Debug.Log("Plants: запуск периодического падения листьев");

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
        Debug.Log("Plants: остановка падения листьев");
    }

    public void OnStateChanged(Game.State newState)
    {
        Debug.Log($"Plants: состояние изменено на {newState}");
        
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