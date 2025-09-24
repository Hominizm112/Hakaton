using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "Audio/Sound")]
public class Sound : ScriptableObject
{
    public SoundType soundType;
    public List<AudioClip> audioClip;
    public bool restrictSoundAmount = true;

    public AudioClip AudioClip => GetAudioClip();


    private AudioClip GetAudioClip()
    {
        if (audioClip.Count == 0)
        {
            return audioClip[0];
        }

        return RandomUtils.GetRandomItemInList(audioClip);
    }
}
