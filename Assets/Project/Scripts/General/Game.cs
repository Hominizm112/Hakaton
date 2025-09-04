using UnityEngine;

public abstract class Game : MonoBehaviour
{
    public enum State
    {
        Gameplay,
        Paused,
        Menu,
        Loading
    }

}
