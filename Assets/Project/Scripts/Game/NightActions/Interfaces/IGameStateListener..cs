using UnityEngine;

public interface IGameStateListener
{
    void CheckGameMode(Game.State newState);
}