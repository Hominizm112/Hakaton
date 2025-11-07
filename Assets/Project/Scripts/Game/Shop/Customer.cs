using Unity.VisualScripting;
using UnityEngine;

public class Customer : MonoBehaviour, IInitializable
{
    private bool _isAnimating;
    public bool Animating => _isAnimating;

    public void SetAnimating(bool isAnimating)
    {
        _isAnimating = isAnimating;
    }
    public void Initialize()
    {

    }
    public NPC npc;
}
