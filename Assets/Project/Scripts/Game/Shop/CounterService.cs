using System;
using System.Collections.Generic;
using UnityEngine;

public class CounterService : MonoService
{
    public override List<Type> requiredServices { get; protected set; } = new List<Type> { typeof(TeaComposer) };
    [SerializeField] private List<TeaBox> teaBoxes;

    private void Awake()
    {
        Mediator.Instance.RegisterService(this);
    }

    public override void Initialize(Mediator mediator)
    {
        base.Initialize();
        foreach (var box in teaBoxes)
        {
            box.OnInteract += r => HandleBoxInteraction(r as TeaBox);
        }
    }

    private void HandleBoxInteraction(TeaBox box)
    {
        print($"Selected box: {box.GetInstanceID()}");
        GetService<TeaComposer>().SetTea(box.tea);
    }


}
