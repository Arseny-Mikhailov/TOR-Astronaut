using _MyGame.Scripts.Gameplay;
using _MyGame.Scripts.Gameplay.Asteroid;
using _MyGame.Scripts.Gameplay.Bullet;
using _MyGame.Scripts.Gameplay.Player;
using _MyGame.Scripts.Systems.Factories;
using _MyGame.Scripts.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace _MyGame.Scripts.Core
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private AssetReference bulletPrefab;
        [SerializeField] private AssetReference asteroidPrefab;
        [SerializeField] private AsteroidSpawnPoints asteroidSpawnPoints;
        [SerializeField] private AsteroidSettings asteroidSettings;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(asteroidSettings);
            builder.RegisterInstance(asteroidSpawnPoints);
            
            builder.RegisterComponent(Camera.main);
            builder.RegisterComponentInHierarchy<GameManager>();
            builder.RegisterComponentInHierarchy<Player>();
            builder.RegisterComponentInHierarchy<HealthBar>();
            builder.RegisterComponentInHierarchy<ScoreBar>();
            builder.RegisterComponentInHierarchy<ScoreText>();
            builder.RegisterComponentInHierarchy<LoseScreen>();
            builder.RegisterComponentInHierarchy<RestartButton>();
            
            builder.Register<BulletFactory>(Lifetime.Scoped)
                .As<IObjectFactory<Bullet>>()
                .WithParameter(bulletPrefab);

            builder.Register<AsteroidFactory>(Lifetime.Scoped)
                .As<IObjectFactory<Asteroid>>()
                .WithParameter(asteroidPrefab);
            
            builder.Register<AsteroidSpawner>(Lifetime.Scoped);
            builder.RegisterEntryPoint<AsteroidSpawner>();
        }
    }
}