using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace ServerInteraction
{
    public class LeaderboardManager : MonoBehaviour
    {
        public GameObject rowPrefab;
        public Transform contentParent;
        
        [System.Serializable]
        public class LeaderboardEntry
        {
            public int rank;
            public string username;
            public float finishTime;
            public int deathCount;
            public float points;

            public LeaderboardEntry(int rank, string username, float finishTime, int deathCount, float points)
            {
                this.rank = rank;
                this.username = username;
                this.finishTime = finishTime;
                this.deathCount = deathCount;
                this.points = points;
            }
        }

        private readonly List<LeaderboardEntry> _leaderboardData = new();

        private void OnEnable()
        {
            var playerRankingResponse = DashboardManager.PlayerRankingResponse;
            for (var i = 0; i < playerRankingResponse.usernameList.Length; i++)
            {
                var rank = i + 1;
                var username = playerRankingResponse.usernameList[i];
                var finishTime = playerRankingResponse.finalFinishTimeList[i];
                var deathCount = playerRankingResponse.totalDeathCountList[i];
                var points = playerRankingResponse.finalPointsList[i];
                _leaderboardData.Add(new LeaderboardEntry(rank, username, finishTime, deathCount, points));
            }
            GenerateLeaderboard();
        }

        private void GenerateLeaderboard()
        {
            for (var i = 0; i < _leaderboardData.Count; i++)
            {
                var entry = _leaderboardData[i];
                var newRow = Instantiate(rowPrefab, contentParent);
                var textComponents = newRow.GetComponentsInChildren<TMP_Text>();

                textComponents[0].text = entry.rank.ToString();
                textComponents[1].text = entry.username;
                textComponents[2].text = entry.finishTime.ToString(CultureInfo.InvariantCulture);
                textComponents[3].text = entry.deathCount.ToString();
                textComponents[4].text = entry.points.ToString(CultureInfo.InvariantCulture);
                
                var rectTransform = newRow.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y - 100f * i);
            }
            GameObject.Find("RowPrefab").SetActive(false);
        }
    }
}