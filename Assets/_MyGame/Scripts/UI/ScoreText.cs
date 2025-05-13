using TMPro;
using UnityEngine;
using VContainer;

namespace _MyGame.Scripts.UI
{
   public class ScoreText : MonoBehaviour
   {
      [Inject] private ScoreBar _scoreBar;
      
      [SerializeField] private TextMeshProUGUI text;

    
      public void WriteScore()
      {
         text.text = "Score: " + _scoreBar.Score;
      }
   }
}
