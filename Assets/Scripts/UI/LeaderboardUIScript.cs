using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ServerInteraction.Responses;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
   
    public class LeaderboardUIScript : MonoBehaviour
    {
        public static List<SoloStats> SoloStatsList;
        private readonly List<PlayerData> _playerDataList = new();
        private VisualElement _root;
        private int _biggestClimb;
        private int _biggestDrop;
        private double _mostPointsGained;
        private double _mostPointsLost;
        private string _biggestDropUsername = "-";
        private string _biggestClimbUsername = "-";
        private string _mostPointsGainedUsername = "-";
        private string _mostPointsLostUsername = "-";
    
        private void Start()
        {
            var uiDocument = GetComponent<UIDocument>();
            _root = uiDocument.rootVisualElement;
            var sortByTotalPointsList = new List<SoloStats>(SoloStatsList);
            var sortByPreviousPointsList = new List<SoloStats>(SoloStatsList);
            sortByTotalPointsList.Sort((a, b) => b.totalPoints.CompareTo(a.totalPoints));
            sortByPreviousPointsList.Sort((a, b) => b.previousPoints.CompareTo(a.previousPoints));
            for (var i = 0; i < sortByTotalPointsList.Count; ++i)
            {
                var curr = sortByTotalPointsList[i];
                var rankDiff = sortByPreviousPointsList.IndexOf(curr) - i;
                var pointsStr = Regex.Matches(curr.matchHistory, @"-?\d+(\.\d+)?").ToArray();
                var points = pointsStr.Select(point => float.Parse(point.ToString())).ToList();
                if (rankDiff > _biggestClimb)
                {
                    _biggestClimb = rankDiff;
                    _biggestClimbUsername = curr.userName;
                }
                if (curr.totalPoints - curr.previousPoints > _mostPointsGained)
                {
                    _mostPointsGained = curr.totalPoints - curr.previousPoints;
                    _mostPointsGainedUsername = curr.userName;
                }
                if (rankDiff < _biggestDrop)
                {
                    _biggestDrop = rankDiff;
                    _biggestDropUsername = curr.userName;
                }
                if (curr.totalPoints - curr.previousPoints < _mostPointsLost)
                {
                    _mostPointsLost = curr.totalPoints - curr.previousPoints;
                    _mostPointsLostUsername = curr.userName;
                }
                _playerDataList.Add(new PlayerData(i + 1, rankDiff, curr.userName, curr.totalPoints, curr.previousPoints, curr.totalPoints - curr.previousPoints, curr.matchHistory, points.ToArray()));
            }
            var topRankedContainer = _root.Q<VisualElement>("TopRankedContainer");
            var biggestClimberContainer = _root.Q<VisualElement>("BiggestClimberContainer");
            var mostPointsGainedContainer = _root.Q<VisualElement>("MostPointsGainedContainer");
            var biggestDropContainer = _root.Q<VisualElement>("BiggestDropContainer");
            var mostPointsLostContainer = _root.Q<VisualElement>("MostPointsLostContainer");
        
            topRankedContainer.Q<Label>("Username").text = sortByTotalPointsList[0].userName;
            biggestClimberContainer.Q<Label>("Username").text = _biggestClimbUsername;
            mostPointsGainedContainer.Q<Label>("Username").text = _mostPointsGainedUsername;
            biggestDropContainer.Q<Label>("Username").text = _biggestDropUsername;
            mostPointsLostContainer.Q<Label>("Username").text = _mostPointsLostUsername;

            var biggestClimbStatus = biggestClimberContainer.Q<VisualElement>("StatusContainer").Q<Label>("Status");
            biggestClimbStatus.style.color = Color.green;
            biggestClimbStatus.text = _biggestClimb.ToString();
            mostPointsGainedContainer.Q<Label>("PointsGained").text = _mostPointsGained.ToString("F1");
            var biggestDropStatus = biggestDropContainer.Q<VisualElement>("StatusContainer").Q<Label>("Status");
            biggestDropStatus.style.color = Color.red;
            biggestDropStatus.text = Mathf.Abs(_biggestDrop).ToString();
            mostPointsLostContainer.Q<Label>("PointsLost").text = _mostPointsLost.ToString("F1");

            var rankingBoardStatsRowList = _root.Query<VisualElement>("RankingBoardStatsRow").ToList();
            foreach (var row in rankingBoardStatsRowList)
            {
                row.style.display = DisplayStyle.None;
            }
        
            for (var i = 0; i < _playerDataList.Count; ++i)
            {
                var curRow = rankingBoardStatsRowList[i];
                var curPlayer = _playerDataList[i];
                curRow.Q<Label>("RankingBoardRankText").text = curPlayer.rank.ToString();
                var status = curRow.Q<VisualElement>("RankingBoardDifferenceRankContainer").Q<Label>("Status");
                var upArrowIcon = curRow.Q<VisualElement>("RankingBoardDifferenceRankContainer").Q<Label>("UpArrowIcon");
                var downArrowIcon = curRow.Q<VisualElement>("RankingBoardDifferenceRankContainer").Q<Label>("DownArrowIcon");
                switch (curPlayer.rankDiff)
                {
                    case 0:
                        curRow.Q<VisualElement>("RankingBoardDifferenceRankContainer").style.display = DisplayStyle.None;
                        break;
                    case > 0:
                        status.style.color = Color.green;
                        downArrowIcon.style.display = DisplayStyle.None;
                        break;
                    default:
                        status.style.color = Color.red;
                        upArrowIcon.style.display = DisplayStyle.None;
                        break;
                }
                curRow.Q<Label>("RankingBoardUsernameText").text = curPlayer.userName;
                curRow.Q<Label>("RankingBoardTotalPointsText").text = curPlayer.totalPoints.ToString(CultureInfo.InvariantCulture);
                curRow.Q<Label>("RankingBoardPreviousPointsText").text = curPlayer.previousPoints.ToString(CultureInfo.InvariantCulture);
                curRow.Q<Label>("RankingBoardDifferenceRankText").text = curPlayer.pointsDiff.ToString(CultureInfo.InvariantCulture);
                var matchWindowContainer = curRow.Q<VisualElement>("RankingBoardMatchWindowContainer");
                var winIconList = matchWindowContainer.Query<Label>("WinMatchIcon").ToList();
                var loseIconList = matchWindowContainer.Query<Label>("LoseMatchIcon").ToList();
                var iconIndex = 0;
                for (var index = Mathf.Max(curPlayer.points.Length - 5, 0); index < curPlayer.points.Length; ++index)
                {
                    var matchHistory = curPlayer.matchHistory.Split(",").ToArray();
                    var curMatchRes = matchHistory[index][0].ToString();   // "W" or "L"
                    if (curMatchRes == "W")
                    {
                        winIconList[iconIndex].style.display = DisplayStyle.Flex;
                    }
                    else
                    {
                        loseIconList[iconIndex].style.display = DisplayStyle.Flex;
                    }

                    ++iconIndex;
                }

                var extendButton = curRow.Q<Button>("RankingBoardExtendButton");
                var lineGraphContainer = _root.Q<VisualElement>("LineGraphContainer");
                extendButton.RegisterCallback<ClickEvent>(_ =>
                {
                    lineGraphContainer.Clear();
                    curPlayer.lineGraphVisible = !curPlayer.lineGraphVisible;
                    lineGraphContainer.style.display = curPlayer.lineGraphVisible ? DisplayStyle.Flex : DisplayStyle.None;
                    if (!curPlayer.lineGraphVisible) return;
                    var pointList = curPlayer.points.Select((t, index) => new Vector2(index + 1, t)).ToArray();
                    lineGraphContainer.Add(new LeaderboardLineGraphDrawer(pointList, pointList.Length, 0, 2000, 300, 100,
                        5));
                });
                curRow.style.display = DisplayStyle.Flex;
                curRow.style.top = (i == 0 ? 5 : 5 + 25 * i);
            }
        }
    }
    class PlayerData
    {
        public int rank;
        public int rankDiff;
        public string userName;
        public float totalPoints;
        public float previousPoints;
        public float pointsDiff;
        public string matchHistory;     // Choose 5 closest matches
        public float[] points;
        public bool lineGraphVisible;

        public PlayerData(int rank, int rankDiff, string userName, float totalPoints, float previousPoints, float pointsDiff, string matchHistory, float[] points)
        {
            this.rank = rank;
            this.rankDiff = rankDiff;
            this.userName = userName;
            this.totalPoints = totalPoints;
            this.previousPoints = previousPoints;
            this.pointsDiff = pointsDiff;
            this.matchHistory = matchHistory;
            this.points = points;
        }
    }

}