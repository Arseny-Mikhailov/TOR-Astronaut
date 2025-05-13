using _MyGame.Scripts.Gameplay.Player;
using UnityEngine;

namespace _MyGame.Scripts.Gameplay.Bullet
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private PlayerSettings settings;
        private Vector2 _direction;

        public void Initialize(Vector2 direction)
        {
            _direction = direction;
        }

        private void Update()
        {
            transform.Translate(_direction * (settings.bulletSpeed * Time.deltaTime));
        }

        private void OnBecameInvisible()
        {
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D otherCollider)
        {
            if (otherCollider.CompareTag("Player")) return;
        
            var asteroid = otherCollider.GetComponent<_MyGame.Scripts.Gameplay.Asteroid.Asteroid>();
            asteroid.TakeDamage(settings.damage);
            Destroy(gameObject);
        }
    }
}