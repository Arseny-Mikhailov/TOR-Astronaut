using _MyGame.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _MyGame.Scripts.UI
{
    
    public class RestartButton : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        
        private Button _button;
        
        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Restart);
        }

        private void Restart()
        {
            _gameManager.RestartGame();
        }
    }
}