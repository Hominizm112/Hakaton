using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Emitter : MonoComponent
{
    [Header("Settings")]
    public AssetReference emitterRef;

    private ParticleSystem _particleSystem;
    public ParticleSystem ParticleSystem => _particleSystem;

    public override void OnConstruct()
    {
        InitializeAssets();
    }

    private async void InitializeAssets()
    {
        var handle = Addressables.InstantiateAsync(emitterRef);
        await handle.Task;

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            _particleSystem = handle.Result.GetComponent<ParticleSystem>();
            _particleSystem.transform.localScale = Vector3.one;
        }
    }

    public void Emit()
    {
        if (!Injected || _particleSystem == null)
        {
            return;
        }

        _particleSystem.transform.position = transform.position;
        _particleSystem?.Play();
    }


    public override void Dispose()
    {
        base.Dispose();
        if (_particleSystem != null && _particleSystem.gameObject != null)
        {
            Addressables.Release(_particleSystem.gameObject);
        }
    }

}