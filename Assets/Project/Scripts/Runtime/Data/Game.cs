using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
public class GameService : Service
{
    public enum State
    {
        Gameplay,
        Paused,
        Menu,
        Loading,
        NightScene,
        Trading,
        MyPortfolio,
        Boot,
        Ready

    }

    private ReactiveProperty<State> _state = new();
    public IReadOnlyReactiveProperty<State> GameState => _state;

    public void SetState(State state)
    {
        _state.Value = state;
    }


}
