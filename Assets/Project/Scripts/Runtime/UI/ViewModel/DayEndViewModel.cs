using GameCore.UI;
using UniRx;
using UnityEngine;
using Zenject;

public class DayEndViewModel : ViewModel
{
    [Inject] private EventBus _eventBus;

    public ReactiveCommand DayEndedCommand = new();

    public override void Initialize()
    {
        disposables.Add(
            _eventBus.Subscribe<TimeTrackCompletedEvent>(_ => DayEndedCommand.Execute()));
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}