using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServerInteraction.Responses;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ServerInteraction.MatchHistoryManager.Solo
{
    public class SoloMatchHistoryManager : MonoBehaviour
    {
        public GameObject searchBar;
        public Button searchButton;
        public Button backToOptionsMatchHistoryButton;
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
            SceneManager.LoadScene("TestScene - Hiep/SoloStatsMatchHistoryScene");
            StaticSoloMatchHistoryList.soloMatchHistoryIndex = index;
        }

        private void OnBackToOptionsMatchHistoryButtonClicked()
        {
            SceneManager.LoadScene("TestScene - Hiep/OptionsMatchHistoryScene");
        }

        private async void OnSearchButtonClicked()
        {
            try
            {
                await CreateGetSoloMatchHistoryRequest();
            }
            catch (Exception e)
            {
                print(e.ToString());
            }
        }
        
        private async Task CreateGetSoloMatchHistoryRequest()
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
                var response = await client.PostAsync("http://localhost:8080/api/gameplay/get-solo-match-history", content);

                // Ensure the response indicates success
                response.EnsureSuccessStatusCode();
                var responseBody = "";
                if (response.IsSuccessStatusCode)
                {
                    responseBody = await response.Content.ReadAsStringAsync();
                    print(responseBody);
                }
                else
                {
                    print(response.StatusCode);
                }

                // Read the response content
                var soloMatchHistoryResponse = JsonConvert.DeserializeObject<GetSoloMatchHistoryResponse>(responseBody);
                var soloMatchHistoryList = soloMatchHistoryResponse.matchHistoryList;
                StaticSoloMatchHistoryList.soloMatchHistoryList = soloMatchHistoryList;
                for (var i = 0; i < soloMatchHistoryList.Length; ++i)
                {
                    var currentMatchStats = soloMatchHistoryList[i];
                    matchStatsList[i].gameObject.SetActive(true);
                    matchStatsList[i].gameObject.GetComponentInChildren<TextMeshProUGUI>().text =
                        currentMatchStats.player1.username + " vs " + currentMatchStats.player2.username + "\n" +
                        currentMatchStats.startTime;
                }
            }
            catch (Exception e)
            {
                print(e.Message);
            }
        }
    }
}