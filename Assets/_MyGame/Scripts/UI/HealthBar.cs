using UnityEngine;
using UnityEngine.UI;

namespace _MyGame.Scripts.UI
{
    public class HealthBar : MonoBehaviour
    {
        private Image _bar;
        
        private const int MaxHealth = 100;

        public void Awake()
        {
            _bar = GetComponent<Image>();
           
            UpdateHealthBar(MaxHealth);
        }

        public void UpdateHealthBar (float health)
        {
            _bar.fillAmount = health / MaxHealth;
        }
    }
}