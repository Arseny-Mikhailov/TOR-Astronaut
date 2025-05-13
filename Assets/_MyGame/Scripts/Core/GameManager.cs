using System.Threading;
using _MyGame.Scripts.Gameplay.Asteroid;
using _MyGame.Scripts.Gameplay.Player;
using _MyGame.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace _MyGame.Scripts.Core
{
    public class GameManager : MonoBehaviour
    {
        private CancellationTokenSource _cts;
        
        [Inject] private LoseScreen _loseScreen;
        [Inject] private Player _player;
        [Inject] private HealthBar _healthBar;
        [Inject] private ScoreBar _scoreBar;
        [Inject] private ScoreText _text;
        [Inject] private AsteroidSettings _asteroidSettings;

        private void Awake()
        {
            _cts = new CancellationTokenSource();
            _player.LoseAction += Lose;
        }

        private void Lose()
        {
            
            _loseScreen.gameObject.SetActive(true);
            _scoreBar.gameObject.SetActive(false);
            _healthBar.gameObject.SetActive(false);
            
            _text.WriteScore();
            
            Time.timeScale = 0;
        }

        public void RestartGame()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
            Time.timeScale = 1;
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        public void PlayerTakeDamage()
        {
            _player.TakeDamage(_asteroidSettings.asteroidDamage);
            _healthBar.UpdateHealthBar(_player.Health);
        }
    }
}
