using System;
using UnityEngine;

public class FlavorComponent : InteractionObject
{
    public Action<TeaFlavorTag> OnUnset;
    private TeaFlavorTag _teaFlavorTag;
    public TeaFlavorTag TeaFlavorTag => _teaFlavorTag;
    private bool _flavorSet;
    public bool IsFlavorSet => _flavorSet;
    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetFlavor(TeaFlavorTag teaFlavorTag)
    {
        if (_flavorSet) return;

        _flavorSet = true;
        _teaFlavorTag = teaFlavorTag;

        spriteRenderer.color = RandomUtils.GetRandomColor(teaFlavorTag.GetHashCode());
    }

    public void UnsetFlavor()
    {
        if (_flavorSet)
        {
            spriteRenderer.color = Color.white;
            OnUnset?.Invoke(_teaFlavorTag);
            _flavorSet = false;

        }
    }

    private void OnDisable()
    {
        UnsetFlavor();

    }
    protected override void HandleInteract()
    {
        UnsetFlavor();
    }

}
