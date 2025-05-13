using System;
using System.Threading;
using _MyGame.Scripts.Gameplay.Bullet;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using VContainer.Unity;

namespace _MyGame.Scripts.Systems.Factories
{
    public class BulletFactory : IObjectFactory<Bullet>, IDisposable
    {
        private readonly IObjectResolver _resolver;
        private readonly AssetReference _bulletPrefab;
        private AsyncOperationHandle<GameObject> _handle;

        public BulletFactory(IObjectResolver resolver, AssetReference bulletPrefab)
        {
            _resolver = resolver;
            _bulletPrefab = bulletPrefab;
        }

        public async UniTask<Bullet> CreateObject(Vector2 position, Vector2 direction, CancellationToken token = default)
        {
            if (!_handle.IsValid())
            {
                _handle = _bulletPrefab.LoadAssetAsync<GameObject>();
                await _handle.ToUniTask(cancellationToken: token);
                if (token.IsCancellationRequested) return null;
            }
            var bulletObj = _resolver.Instantiate(_handle.Result, position, Quaternion.identity);
            bulletObj.TryGetComponent<Bullet>(out var bulletComponent);
    
            Addressables.Release(_handle);
            bulletComponent.Initialize(direction);
            
            return bulletComponent;
        }

        public void Dispose()
        {
            if (_handle.IsValid())
                Addressables.Release(_handle);
            _resolver?.Dispose();
        }
    }
}