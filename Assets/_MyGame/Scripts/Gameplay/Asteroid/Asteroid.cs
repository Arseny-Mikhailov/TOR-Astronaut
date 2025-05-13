using System;
using _MyGame.Scripts.Core;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace _MyGame.Scripts.Gameplay.Asteroid
{
    public class Asteroid : MonoBehaviour, IDamageable
    {
        public event Action<Asteroid> OnAsteroidMissed;
        public event Action<Asteroid> OnAsteroidDestroyed;

        private float _health;
        private float _speed;
        private float _rotationSpeed;
        private Vector2 _direction;
        private Tween _rotationTween;

        private AsteroidSettings _settings;

        public void Initialize(AsteroidSettings settings, Vector2 direction)
        {
            _settings = settings;
            _health = settings.asteroidHealth;
            _speed = Random.Range(settings.minSpeed, settings.maxSpeed);
            _direction = direction;
            SetRotation();
        }

        private void SetRotation()
        {
            _rotationSpeed = Random.Range(_settings.minRotationSpeed, _settings.maxRotationSpeed);
            transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
    
            var rotationDuration = 360f / _rotationSpeed;
            var direction = Random.Range(0, 2);
            var targetRotation = direction switch
            {
                0 => new Vector3(0, 0, 360f),
                1 => new Vector3(0, 0, -360f),
                _ => new Vector3()
            };

            _rotationTween = transform.DORotate(targetRotation, rotationDuration).SetLoops(-1);
        }

        private void Update()
        {
            transform.Translate(_direction * (_speed * Time.deltaTime), Space.World);

            if (transform.position.y > _settings.finishLine) return;
            
            OnAsteroidMissed?.Invoke(this);
            DestroyAsteroid();
        }

        public void TakeDamage(float damage)
        {
            _health -= damage;
            
            if (!(_health <= 0)) return;
            
            OnAsteroidDestroyed?.Invoke(this);
            DestroyAsteroid();
        }

        private void DestroyAsteroid()
        {
            _rotationTween?.Kill();
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            _rotationTween?.Kill();
        }
    }
}