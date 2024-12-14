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

namespace ServerInteraction.MatchHistoryManager
{
    public class MatchHistoryManager : MonoBehaviour
    {
        public GameObject searchBar;
        public Button searchButton;
        public Button backDashboardButton;
        public Button[] soloMatchStatsList;
        public Button[] teamMatchStatsList;

        private void Start()
        {
            searchButton.onClick.AddListener(OnSearchButtonClicked);
            backDashboardButton.onClick.AddListener(OnBackToDashboardButtonClicked);
            for (var i = 0; i < soloMatchStatsList.Length; ++i)
            {
                var matchStats = soloMatchStatsList[i];
                var i1 = i;
                matchStats.onClick.AddListener(() => OnSoloMatchStatsButtonClicked(i1));
            }

            for (var i = 0; i < teamMatchStatsList.Length; ++i)
            {
                var matchStats = teamMatchStatsList[i];
                var i1 = i;
                matchStats.onClick.AddListener(() => OnTeamMatchHistoryButtonClicked(i1));
            }
        }

        private void OnSoloMatchStatsButtonClicked(int index)
        {
            SceneManager.LoadScene("TestScene - Hiep/SoloStatsMatchHistoryScene");
            StaticSoloMatchHistoryList.soloMatchHistoryIndex = index;
        }

        private void OnTeamMatchHistoryButtonClicked(int index)
        {
            SceneManager.LoadScene("TestScene - Hiep/TeamStatsMatchHistoryScene");
            StaticTeamMatchHistoryList.teamMatchHistoryIndex = index;
        }
        
        private void OnBackToDashboardButtonClicked()
        {
            SceneManager.LoadScene("TestScene - Hiep/DashboardScene");
        }

        private async void OnSearchButtonClicked()
        {
            try
            {
                await CreateGetSoloAndTeamMatchHistoryRequest();
            }
            catch (Exception e)
            {
                print(e.ToString());
            }
        }
        
        private async Task CreateGetSoloAndTeamMatchHistoryRequest()
        {
            var username = searchBar.transform.Find("Text Area").transform.Find("Text").GetComponent<TextMeshProUGUI>().text;
            try
            {
                var client = new HttpClient();
                // Create the JSON payload
                var jsonPayload = JsonConvert.SerializeObject(new { username });

                // Create an HttpContent with the JSON payload
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Send POST request
                var soloResponse = await client.PostAsync("http://localhost:8080/api/gameplay/get-solo-match-history", content);
                
                // Ensure the response indicates success
                soloResponse.EnsureSuccessStatusCode();
                
                var soloResponseBody = "";
                
                if (soloResponse.IsSuccessStatusCode)
                {
                    soloResponseBody = await soloResponse.Content.ReadAsStringAsync();
                    print(soloResponseBody);
                }
                else
                {
                    print(soloResponse.StatusCode);
                }

                // Read the response content
                var soloMatchHistoryResponse = JsonConvert.DeserializeObject<GetSoloMatchHistoryResponse>(soloResponseBody);
                var soloMatchHistoryList = soloMatchHistoryResponse.matchHistoryList;
                StaticSoloMatchHistoryList.soloMatchHistoryList = soloMatchHistoryList;
                for (var i = 0; i < soloMatchHistoryList.Length; ++i)
                {
                    var currentSoloMatchStats = soloMatchHistoryList[i];
                    soloMatchStatsList[i].gameObject.SetActive(true);
                    soloMatchStatsList[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text =
                        currentSoloMatchStats.player1.username + " vs " + currentSoloMatchStats.player2.username + "\n" +
                        currentSoloMatchStats.startTime;
                }
            }
            catch (Exception e)
            {
                print(e.Message);
            }

            try
            {
                var client = new HttpClient();
                var jsonPayload = JsonConvert.SerializeObject(new { username });
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var teamResponse = await client.PostAsync("http://localhost:8080/api/gameplay/get-team-match-history-by-user-name", content);
                teamResponse.EnsureSuccessStatusCode();
                var teamResponseBody = "";
                if (teamResponse.IsSuccessStatusCode)
                {
                    teamResponseBody = await teamResponse.Content.ReadAsStringAsync();
                    print(teamResponseBody);
                }
                else
                {
                    print(teamResponse.StatusCode);
                }
                var teamMatchHistoryResponse = JsonConvert.DeserializeObject<GetTeamMatchHistoryByUsernameResponse>(teamResponseBody);
                var teamMatchHistoryList = teamMatchHistoryResponse.matchHistoryList;
                StaticTeamMatchHistoryList.teamMatchHistoryList = teamMatchHistoryList;
                for (var i = 0; i < teamMatchHistoryList.Length; ++i)
                {
                    var currentTeamMatchStats = teamMatchHistoryList[i];
                    teamMatchStatsList[i].gameObject.SetActive(true);
                    teamMatchStatsList[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = 
                        currentTeamMatchStats.teamName + "\n" + currentTeamMatchStats.startTime;
                }
            }
            catch (Exception e)
            {
                print(e.Message);
            }
        }
    }
}