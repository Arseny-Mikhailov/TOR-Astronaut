using UnityEngine;

namespace _MyGame.Scripts.Gameplay.Player
{
    [CreateAssetMenu(menuName = "Settings/PlayerSettings")]
    public class PlayerSettings : ScriptableObject
    {
        [Header("Player")]
        public int moveSpeed = 40;
        public int damage = 3;
        public float attackRange = 5f;
        public float attackWidth = 2f;
        public float attackSpeed = 1f;
        public float finishLine = -1.5f;
        [Header("Bullet")]
        public float bulletSpeed = 20f;
    }
}