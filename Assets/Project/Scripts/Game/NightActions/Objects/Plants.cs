using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Plants : MonoBehaviour, IInteractable, IStateListener
{
    [SerializeField] private PlantsAnimationSettings _animationSettings;
    private bool _isActive = false;
    private Sequence _DropSequence;
    public bool IsActive => _isActive;

    public void Interact()
    {
        SpawnLeaf();
    }

    void SpawnLeaf()
    {
        if (_animationSettings.leafPrefab == null) return;
        GameObject leaf = Instantiate(_animationSettings.leafPrefab, _animationSettings.dropLeavesPoint, Quaternion.identity);//создание копии(листьев будет мало)

        leaf.transform.DOMoveY(_animationSettings.dropLeavesPoint.y - _animationSettings.dropDistance, _animationSettings.dropDuration)
            .SetEase(Ease.InCubic)
            .OnComplete(() => Destroy(leaf));
    }

    void Start()
    {
        Mediator.Instance?.SubscribeToState(this, Game.State.NightScene);
        if (!IsActive&&Mediator.Instance?.CurrentState == Game.State.NightScene) {
          StartPeriodicAction();
      }
    }

    private void StartPeriodicAction()
    {
        if (_isActive) return;

        _DropSequence = DOTween.Sequence()
            .AppendInterval(_animationSettings.actionInterval)
            .AppendCallback(Interact)
            .SetLoops(-1);
        _isActive = true;
    }

    void OnDestroy()
    {
        Mediator.Instance?.UnsubscribeFromState(this, Game.State.NightScene);
        StopPeriodicAction();
    }
    public void StopPeriodicAction()
    {
        _DropSequence?.Kill();
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

}
