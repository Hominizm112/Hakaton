using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    None,
    ButtonPress,
    CoinToss,
    PC_ButtonPress,
    PC_TextChangeSound,

}
public class AudioHub : MonoBehaviour
{
    [SerializeField] private int initialPoolSize = 5;
    [SerializeField] private float clipCooldown = 0.1f;
    [SerializeField] private List<Sound> sounds;

    private Dictionary<AudioClip, float> _lastPlayedTimes = new();

    private Queue<AudioSource> _audioSourcePool = new Queue<AudioSource>();
    private List<AudioSource> _activeSources = new List<AudioSource>();
    private Dictionary<SoundType, Sound> soundsDict = new();

    private void Awake()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewPooledSource();
        }
        BuildSounds();
    }

    private void BuildSounds()
    {
        foreach (var sound in sounds)
        {
            print($"Added {sound.name}");
            soundsDict.Add(sound.soundType, sound);
        }
    }

    private void Update()
    {
        for (int i = _activeSources.Count - 1; i >= 0; i--)
        {
            if (!_activeSources[i].isPlaying)
            {
                ReturnToPool(_activeSources[i]);
                _activeSources.RemoveAt(i);
            }
        }
    }


    public void PlayOneShot(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        PlayClip(clip, volume, pitch);
    }

    public void PlayOneShot(AudioClip clip)
    {
        PlayOneShot(clip, 1, 1);
    }


    public void PlayOneShot(Sound sound, float volume = 1f, float pitch = 1f)
    {
        PlayClip(sound.AudioClip, volume, pitch, sound.restrictSoundAmount);
    }


    public void PlayOneShot(SoundType soundType, float volume = 1f, float pitch = 1f)
    {
        if (soundType == SoundType.None) return;

        soundsDict.TryGetValue(soundType, out Sound sound);
        if (sound == null)
        {
            Mediator.Instance.GlobalEventBus.Publish(new DebugLogErrorEvent($"Sound not found with type of {soundType}"));
            return;
        }
        PlayClip(sound.AudioClip, volume, pitch, sound.restrictSoundAmount);
    }

    public void PlayOneShot(SoundType soundType, float pitch = 1f)
    {
        PlayOneShot(soundType, 1f, pitch);

    }
    public void PlayOneShot(SoundType soundType)
    {
        PlayOneShot(soundType, 1f, 1f);
    }


    private void PlayClip(AudioClip clip, float volume, float pitch, bool restrictSoundAmount = false)
    {
        if (clip == null) { return; }

        if (restrictSoundAmount && _lastPlayedTimes.TryGetValue(clip, out float lastTime))
        {
            if (Time.time - lastTime < clipCooldown)
            {
                return;
            }
        }


        if (pitch != 1)
        {
            pitch = Random.Range(1 - pitch, 1 + pitch);
        }

        AudioSource source = GetPooledSource();
        source.pitch = pitch;
        source.PlayOneShot(clip, volume);
        _activeSources.Add(source);

        if (restrictSoundAmount)
        {
            _lastPlayedTimes[clip] = Time.time;
        }
    }
    #region Pool
    private AudioSource GetPooledSource()
    {
        if (_audioSourcePool.Count == 0)
        {
            CreateNewPooledSource();
        }

        AudioSource source = _audioSourcePool.Dequeue();
        source.enabled = true;
        return source;
    }

    private void ReturnToPool(AudioSource source)
    {
        source.enabled = false;
        _audioSourcePool.Enqueue(source);
    }

    private void CreateNewPooledSource()
    {
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.playOnAwake = false;
        newSource.enabled = false;
        _audioSourcePool.Enqueue(newSource);
    }
    #endregion
}