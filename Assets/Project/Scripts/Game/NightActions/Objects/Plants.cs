using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Plants : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _leafPrefab;
    [SerializeField] private Vector2 _DropLeavesPoint;
    [SerializeField] private float _DropDistance = 5f;
    [SerializeField] private float _DropDuration = 5f;
    [SerializeField] private float _actionInterval = 10f;//временной интервал между падением
    private bool _isActive = false;
    private Sequence _DropSequence;
    public bool IsActive => _isActive;

    public void Interact()
    {
        SpawnLeaf();
    }

    void SpawnLeaf()
    {
        if (_leafPrefab == null) return;
        GameObject leaf = Instantiate(_leafPrefab, _DropLeavesPoint, Quaternion.identity);//создание копии(листьев будет мало)

        leaf.transform.DOMoveY(_DropLeavesPoint.y - _DropDistance, _DropDuration)
            .SetEase(Ease.InCubic)
            .OnComplete(() => Destroy(leaf));
    }

    void Start()
    {
        StartPeriodicAction();
        Game.GamesStateChanged += CheckGameMode;//подписка на GamesStateChanged
    }

    private void StartPeriodicAction()
    {
        if (_isActive) return;

        _DropSequence = DOTween.Sequence()
            .AppendInterval(_actionInterval)
            .AppendCallback(Interact)
            .SetLoops(-1);
        _isActive = true;
    }

    public void StopPeriodicAction()
    {
        _DropSequence?.Kill();
        _isActive = false;
    }

    private void CheckGameMode(Game.State newState)
    {
        if (newState != Game.State.NightScene)
        {
            StopPeriodicAction();
        }
        else if (newState == Game.State.NightScene)
        {
            StartPeriodicAction();
        }

    }
    
        void OnDestroy()
    {
        Game.GamesStateChanged -= CheckGameMode;
        StopPeriodicAction();
    }


}
