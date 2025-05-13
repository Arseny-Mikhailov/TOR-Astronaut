using TMPro;
using UnityEngine;

namespace _MyGame.Scripts.UI
{
    public class ScoreBar : MonoBehaviour
    {
        private TextMeshProUGUI _textMeshProUGUI;
        public int Score { get; private set; }

        public void Awake()
        {
            _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            
            _textMeshProUGUI.text = "Score:\n" + Score;
        }

        public void UpScore()
        {
            Score++;
            _textMeshProUGUI.text = "Score:\n" + Score;
        }
    }
}