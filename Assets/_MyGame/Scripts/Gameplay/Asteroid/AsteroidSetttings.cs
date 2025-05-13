using UnityEngine;

namespace _MyGame.Scripts.Gameplay.Asteroid
{
    [CreateAssetMenu(menuName = "Settings/AsteroidSettings")]
    public class AsteroidSettings : ScriptableObject
    {
        [Header("Asteroid")]
        public int asteroidDamage = 20;
        public int asteroidHealth = 3;
        public float minSpeed = 5f;
        public float maxSpeed = 5f;
        public float minRotationSpeed = 30f;
        public float maxRotationSpeed = 120f;
        public float finishLine = -1.5f;
        [Header("Spawn")]
        public float minSpawnDelay = 0.5f;
        public float maxSpawnDelay = 2f;
    }
}