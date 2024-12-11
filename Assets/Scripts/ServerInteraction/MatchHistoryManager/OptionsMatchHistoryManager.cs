using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ServerInteraction.MatchHistoryManager
{
    public class OptionsMatchHistoryManager : MonoBehaviour
    {
        public Button backToDashboardButton;
        public Button soloMatchHistory;
        public Button teamMatchHistory;

        private void Start()
        {
            backToDashboardButton.onClick.AddListener(OnBackToDashboardButtonClicked);
            soloMatchHistory.onClick.AddListener(OnSoloMatchHistoryButtonClicked);
            teamMatchHistory.onClick.AddListener(OnTeamMatchHistoryButtonClicked);
        }

        private void OnBackToDashboardButtonClicked()
        {
            SceneManager.LoadScene("TestScene - Hiep/DashboardScene");
        }
        
        private void OnSoloMatchHistoryButtonClicked()
        {
            SceneManager.LoadScene("TestScene - Hiep/SoloMatchHistoryScene");
        }

        private void OnTeamMatchHistoryButtonClicked()
        {
            SceneManager.LoadScene("TestScene - Hiep/TeamMatchHistoryScene");
        }
    }
}