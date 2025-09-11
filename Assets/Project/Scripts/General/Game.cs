using UnityEngine;
using System;
public abstract class Game : MonoBehaviour
{
    public enum State
    {
        Gameplay,
        Paused,
        Menu,
        Loading,
        NightScene,
        TradingActions,
        TradingObligations,

    }

    public static event Action<State> GamesStateChanged;
    private static State _actualState;
    public static State ActualState
    {
        get => _actualState;
        set
        {
            if (_actualState != value)
            {
                _actualState = value;
                GamesStateChanged?.Invoke(_actualState);

            }
        }
    }

}
