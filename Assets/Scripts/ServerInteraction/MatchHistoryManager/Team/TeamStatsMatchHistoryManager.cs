using ServerInteraction.MatchHistoryManager.Solo;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ServerInteraction.MatchHistoryManager.Team
{
    public class TeamStatsMatchHistoryManager : MonoBehaviour
    {
        public TMP_Text durationText;
        [FormerlySerializedAs("backToTeamMatchHistoryButton")] public Button backToMatchHistoryButton;
        public GameObject[] playerFrameList;

        private void Start()
        {
            var teamMatchHistory = StaticTeamMatchHistoryList.teamMatchHistoryList[StaticTeamMatchHistoryList.teamMatchHistoryIndex];
            backToMatchHistoryButton.onClick.AddListener(OnBackToMatchHistoryButtonClicked);
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

        private void OnBackToMatchHistoryButtonClicked()
        {
            SceneManager.LoadScene("TestScene - Hiep/MatchHistoryScene");
        }
    }
}