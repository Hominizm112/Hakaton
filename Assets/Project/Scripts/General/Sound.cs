using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "Audio/Sound")]
public class Sound : ScriptableObject
{
    public SoundType soundType;
    public AudioClip audioClip;
    public bool restrictSoundAmount = true;
}
