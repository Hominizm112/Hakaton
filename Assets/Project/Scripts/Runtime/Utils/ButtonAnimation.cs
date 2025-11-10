using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    [SerializeField] private string propertyName;
    [SerializeField] private Animator animator;

    public void ClickDown()
    {
        animator.SetBool(propertyName, true);
    }


    public void ClickUp()
    {
        animator.SetBool(propertyName, false);
    }
}
