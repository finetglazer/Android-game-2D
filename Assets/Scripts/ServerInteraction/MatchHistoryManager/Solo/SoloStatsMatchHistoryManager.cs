using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ServerInteraction.MatchHistoryManager.Solo
{
    public class SoloStatsMatchHistoryManager : MonoBehaviour
    {
        public TMP_Text durationText;
        public Button backToMatchHistoryButton;
        public GameObject winnerFrame;
        public GameObject loserFrame;

        private void Start()
        {
            backToMatchHistoryButton.onClick.AddListener(OnBackToMatchHistoryButtonClicked);
            var winnerContent = winnerFrame.GetComponentInChildren<TextMeshProUGUI>();
            var winnerPointsText = winnerFrame.transform.Find("Points").GetComponentInChildren<TextMeshProUGUI>();
            var winnerDamageDealtText = winnerFrame.transform.Find("DamageDealt").GetComponentInChildren<TextMeshProUGUI>();
            var winnerDamageTakenText = winnerFrame.transform.Find("DamageTaken").GetComponentInChildren<TextMeshProUGUI>();
            var loserContent = loserFrame.GetComponentInChildren<TextMeshProUGUI>();
            var loserPointsText = loserFrame.transform.Find("Points").GetComponentInChildren<TextMeshProUGUI>();
            var loserDamageDealtText = loserFrame.transform.Find("DamageDealt").GetComponentInChildren<TextMeshProUGUI>();
            var loserDamageTakenText = loserFrame.transform.Find("DamageTaken").GetComponentInChildren<TextMeshProUGUI>();

            var soloMatchHistoryList = StaticSoloMatchHistoryList.soloMatchHistoryList;
            var index = StaticSoloMatchHistoryList.soloMatchHistoryIndex;
            var player1 = soloMatchHistoryList[index].player1;
            var player2 = soloMatchHistoryList[index].player2;
            var winner = player1.won ? player1 : player2;
            var loser = player1.won ? player2 : player1;
            
            durationText.text = soloMatchHistoryList[index].duration.ToString();
            winnerContent.text = winner.username;
            loserContent.text = loser.username;
            
            winnerPointsText.text = winner.points.ToString("F1");
            winnerDamageDealtText.text = winner.damageDealt.ToString("F1");
            winnerDamageTakenText.text = winner.damageTaken.ToString("F1");
            
            loserPointsText.text = loser.points.ToString("F1");
            loserDamageDealtText.text = loser.damageDealt.ToString("F1");
            loserDamageTakenText.text = loser.damageTaken.ToString("F1");
        }

        private void OnBackToMatchHistoryButtonClicked()
        {
            SceneManager.LoadScene("TestScene - Hiep/MatchHistoryScene");
        }
    }
}