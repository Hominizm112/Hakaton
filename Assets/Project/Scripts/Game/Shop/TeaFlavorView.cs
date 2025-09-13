using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


[RequireComponent(typeof(Button))]
public class TeaFlavorView : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private TeaFlavorTag _teaFlavor;
    public TeaFlavorTag TeaFlavor => _teaFlavor;
    private Button _button;
    public Action<TeaFlavorView> OnButtonClick;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => OnButtonClick?.Invoke(this));
    }
    public void SetFlavor(TeaFlavorTag flavor)
    {
        _teaFlavor = flavor;
        text.text = flavor.ToString();
    }

    public void OnDestroy()
    {
        OnButtonClick = null;
        _button.onClick.RemoveAllListeners();
    }
}
