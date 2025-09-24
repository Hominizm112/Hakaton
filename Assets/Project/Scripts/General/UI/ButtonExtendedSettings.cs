using UnityEngine;

[CreateAssetMenu(fileName = "ButtonExtendedSettings", menuName = "UI/ButtonExtendedSettings")]
public class ButtonExtendedSettings : ScriptableObject
{
    [SerializeField, Tooltip("Sound to play when button is pressed")]
    public SoundType soundType;
    [SerializeField] public bool spriteSwap;
    [SerializeField] public Sprite spriteToSwap;
    [SerializeField] public bool textColorSwap;
    [SerializeField] public Color textColorToSwap;
    [SerializeField] public SoundType holdSoundType;
    [SerializeField] public float holdDelay = .2f;
}
