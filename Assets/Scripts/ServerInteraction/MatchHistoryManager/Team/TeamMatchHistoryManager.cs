using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServerInteraction.MatchHistoryManager.Solo;
using ServerInteraction.Responses;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ServerInteraction.MatchHistoryManager.Team
{
    public class TeamMatchHistoryManager : MonoBehaviour
    {
        public GameObject searchBar;
        public Button backToOptionsMatchHistoryButton;
        public Button searchButton;
        public Button[] matchStatsList;

        private void Start()
        {
            searchButton.onClick.AddListener(OnSearchButtonClicked);
            backToOptionsMatchHistoryButton.onClick.AddListener(OnBackToOptionsMatchHistoryButtonClicked);
            for (var i = 0; i < matchStatsList.Length; ++i)
            {
                var matchStats = matchStatsList[i];
                var i1 = i;
                matchStats.onClick.AddListener(() => OnMatchStatsButtonClicked(i1));
            }
        }

        private void OnMatchStatsButtonClicked(int index)
        {
            SceneManager.LoadScene("TestScene - Hiep/TeamStatsMatchHistoryScene");
            StaticTeamMatchHistoryList.teamMatchHistoryIndex = index;
        }
        
        private void OnBackToOptionsMatchHistoryButtonClicked()
        {
            SceneManager.LoadScene("TestScene - Hiep/OptionsMatchHistoryScene");
        }

        private async void OnSearchButtonClicked()
        {
            try
            {
                await CreateGetTeamMatchHistoryRequest();
            }
            catch (Exception e)
            {
                print(e.ToString());
            }
        }
        
        private async Task CreateGetTeamMatchHistoryRequest()
        {
            var teamName = searchBar.transform.Find("Text Area").transform.Find("Text").GetComponent<TextMeshProUGUI>().text;
            try
            {
                var client = new HttpClient();
                // Create the JSON payload
                var jsonPayload = "{\"teamName\":\""+ teamName + "\"}";

                // Create an HttpContent with the JSON payload
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Send POST request
                var response = await client.PostAsync("http://localhost:8080/api/gameplay/get-team-match-history", content);

                // Ensure the response indicates success
                response.EnsureSuccessStatusCode();

                // Read the response content
                var responseBody = await response.Content.ReadAsStringAsync();
                print(responseBody);
                var teamMatchHistoryResponse = JsonConvert.DeserializeObject<GetTeamMatchHistoryResponse>(responseBody);
                var teamMatchHistoryList = teamMatchHistoryResponse.teamMatchHistoryList;
                StaticTeamMatchHistoryList.teamMatchHistoryList = teamMatchHistoryList;
                for (var i = 0; i < teamMatchHistoryList.Length; ++i)
                {
                    var currentMatchStats = teamMatchHistoryList[i];
                    matchStatsList[i].gameObject.SetActive(true);
                    matchStatsList[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = currentMatchStats.teamName + "\n" + currentMatchStats.startTime;
                }
            }
            catch (Exception e)
            {
                    print(e.Message);
            }
        }
    }
}