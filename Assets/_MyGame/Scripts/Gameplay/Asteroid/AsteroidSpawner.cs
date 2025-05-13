using System;
using System.Threading;
using _MyGame.Scripts.Systems.Factories;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;
using Random = UnityEngine.Random;

namespace _MyGame.Scripts.Gameplay.Asteroid
{
    public class AsteroidSpawner : IStartable, IDisposable
    {
        private readonly IObjectFactory<Asteroid> _asteroidFactory;
        private readonly AsteroidSpawnPoints _spawnPoints;
        private readonly AsteroidSettings _settings;
        
        private CancellationTokenSource _cts;

        public AsteroidSpawner(
            IObjectFactory<Asteroid> asteroidFactory, 
            AsteroidSpawnPoints spawnPoints, 
            AsteroidSettings settings)
        {
            _asteroidFactory = asteroidFactory;
            _spawnPoints = spawnPoints;
            _settings = settings;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            Spawn().Forget();
        }

        private async UniTaskVoid Spawn()
        {
            while (!_cts.IsCancellationRequested)
            {
                var spawnPoint = _spawnPoints.points[Random.Range(0, _spawnPoints.points.Length)];
                
                await _asteroidFactory.CreateObject(spawnPoint.position, Vector2.down, _cts.Token);
                    
                var delay = Random.Range(_settings.minSpawnDelay, _settings.maxSpawnDelay);
                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: _cts.Token);
            }
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}