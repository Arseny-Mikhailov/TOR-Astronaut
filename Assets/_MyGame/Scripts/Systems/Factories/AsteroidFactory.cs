using System;
using System.Threading;
using _MyGame.Scripts.Core;
using _MyGame.Scripts.Gameplay.Asteroid;
using _MyGame.Scripts.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using VContainer.Unity;

namespace _MyGame.Scripts.Systems.Factories
{
    public class AsteroidFactory : IObjectFactory<Asteroid>, IDisposable
    {
        [Inject] private GameManager _gameManager;
             [Inject] private ScoreBar _scoreBar;
        [Inject] private AsteroidSettings _asteroidSettings;
        
        private readonly IObjectResolver _resolver;
        private readonly AssetReference _asteroidPrefab;
        private AsyncOperationHandle<GameObject> _handle;

        public AsteroidFactory(IObjectResolver resolver, AssetReference asteroidPrefab)
        {
            _resolver = resolver;
            _asteroidPrefab = asteroidPrefab;
        }

        public async UniTask<Asteroid> CreateObject(Vector2 position, Vector2 direction, CancellationToken token = default)
        {
            if (!_handle.IsValid())
            {
                _handle = _asteroidPrefab.LoadAssetAsync<GameObject>();
                await _handle.ToUniTask(cancellationToken: token);
                if (token.IsCancellationRequested) return null;
            }
            
            var asteroidObj = _resolver.Instantiate(_handle.Result, position, Quaternion.identity);
            var asteroidComponent = asteroidObj.GetComponent<Asteroid>();
            
            asteroidComponent.Initialize(_asteroidSettings,direction);
            
            asteroidComponent.OnAsteroidMissed += HandleAsteroidMissed;
            asteroidComponent.OnAsteroidDestroyed += HandleAsteroidDestroyed;
            Addressables.Release(_handle);
            
            return asteroidComponent;
        }
        
        private void HandleAsteroidMissed(Asteroid obj)
        {
            _gameManager.PlayerTakeDamage();
        }

        private void HandleAsteroidDestroyed(Asteroid obj)
        {
            _scoreBar.UpScore();
        }

        public void Dispose()
        {
            if (_handle.IsValid())
                Addressables.Release(_handle);
            _resolver?.Dispose();
        }
    }
}