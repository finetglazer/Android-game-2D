using ServerInteraction.MatchHistoryManager.Solo;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ServerInteraction.MatchHistoryManager.Team
{
    public class TeamStatsMatchHistoryManager : MonoBehaviour
    {
        public TMP_Text durationText;
        public Button backToTeamMatchHistoryButton;
        public GameObject[] playerFrameList;

        private void Start()
        {
            var teamMatchHistory = StaticTeamMatchHistoryList.teamMatchHistoryList[StaticTeamMatchHistoryList.teamMatchHistoryIndex];
            backToTeamMatchHistoryButton.onClick.AddListener(OnBackToTeamMatchHistoryButtonClicked);
            durationText.text = teamMatchHistory.duration.ToString();
            for (var i = 0; i < teamMatchHistory.teamSize; ++i)
            {
                var playerFrame = playerFrameList[i];
                var teamMemberStats = teamMatchHistory.teamMembers[i];
                playerFrame.SetActive(true);
                playerFrame.transform.Find("Content").GetComponent<TextMeshProUGUI>().text = teamMemberStats.username;
                playerFrame.transform.Find("DeathCount").GetComponentInChildren<TextMeshProUGUI>().text = teamMemberStats.deathCount.ToString();
            }
        }

        private void OnBackToTeamMatchHistoryButtonClicked()
        {
            SceneManager.LoadScene("TestScene - Hiep/TeamMatchHistoryScene");
        }
    }
}