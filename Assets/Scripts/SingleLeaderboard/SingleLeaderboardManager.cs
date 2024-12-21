using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SingleLeaderboard
{
    public class SingleLeaderboardManager : MonoBehaviour
    {
        public Button backToDashboardButton;
        public GameObject[] rowData;
        public GameObject graph;
        private string[] _usernameList;
        private float[] _finishTimeList;
        private float[] _finalPointsList;
        private int[] _deathCountList;

        private void Start()
        {
            backToDashboardButton.onClick.AddListener(OnBackToDashboardButtonClicked);
            var statsLists = StaticSingleLeaderboardLists.allSingleStatsLists;
            _usernameList = statsLists.usernameList;
            _finishTimeList = statsLists.finalFinishTimeList;
            _finalPointsList = statsLists.finalPointsList;
            _deathCountList = statsLists.totalDeathCountList;
            for (var i = 0; i < _usernameList.Length; ++i)
            {
                var i1 = i;
                if (i != 0)
                {
                    var prevRowPosition = rowData[i - 1].GetComponent<RectTransform>().localPosition;
                    rowData[i].GetComponent<RectTransform>().localPosition =
                        new Vector3(prevRowPosition.x, prevRowPosition.y - 58, prevRowPosition.z);
                }
                rowData[i].SetActive(true);
                rowData[i].transform.Find("RankText").GetComponent<TextMeshProUGUI>().text = "#" + (i + 1);
                rowData[i].transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>().text = _usernameList[i];
                rowData[i].transform.Find("PointsCircle").transform.Find("FilledCircle").GetComponent<Image>().fillAmount = _finalPointsList[i] / 100;
                rowData[i].transform.Find("PointsCircle").transform.Find("Text").GetComponent<TextMeshProUGUI>().text = _finalPointsList[i].ToString("F1");
                rowData[i].transform.Find("FinishTimeText").GetComponent<TextMeshProUGUI>().text = _finishTimeList[i].ToString("F1");
                rowData[i].transform.Find("DeathCountText").GetComponent<TextMeshProUGUI>().text = _deathCountList[i].ToString();
                rowData[i].transform.Find("MoreButton").GetComponent<Button>().onClick.AddListener(() => OnMoreButtonClicked(i1));
            }
        }

        private void OnBackToDashboardButtonClicked()
        {
            SceneManager.LoadScene("Scenes/DashboardScene");
        }

        private void OnMoreButtonClicked(int index)
        {
            graph.SetActive(true);
            graph.transform.Find("GraphContainerBorder").gameObject.SetActive(true);
            var graphContainer = graph.transform.Find("GraphContainer");
            graphContainer.gameObject.SetActive(true);
            graphContainer.transform.Find("CloseGraphButton").GetComponent<Button>().onClick.AddListener(OnCloseGraphButtonClicked);
            var graphDrawer = graphContainer.transform.Find("GraphDrawer").GetComponent<SingleLeaderboardTriangleGraphDrawer>();
            var minFinalPoints = _finalPointsList.Min();
            var maxFinalPoints = _finalPointsList.Max();
            var minFinishTime = _finishTimeList.Min();
            var maxFinishTime = _finishTimeList.Max();
            var minDeathCount = _deathCountList.Min();
            var maxDeathCount = _deathCountList.Max();
            graphDrawer.playerValues[0] = (maxFinalPoints - minFinalPoints != 0) ? (_finalPointsList[index] - minFinalPoints) / (maxFinalPoints - minFinalPoints) : 1;
            graphDrawer.playerValues[1] = (maxFinishTime - minFinishTime != 0) ? 1 - (_finishTimeList[index] - minFinishTime) / (maxFinishTime - minFinishTime) : 1; 
            graphDrawer.playerValues[2] = (maxDeathCount - minDeathCount != 0) ? 1 - (_deathCountList[index] - minDeathCount) * 1.0f / (maxDeathCount - minDeathCount) : 1;
            graphDrawer.finalPointsText.text = graphDrawer.playerValues[0].ToString("F1");
            graphDrawer.finishTimeText.text = graphDrawer.playerValues[1].ToString("F1");
            graphDrawer.deathCountText.text = graphDrawer.playerValues[2].ToString("F1");
           
            graphDrawer.DrawTriangle();
            print("points: " + graphDrawer.playerValues[0] + " time: " + graphDrawer.playerValues[1] + " death: " + graphDrawer.playerValues[2]);
        }

        private void OnCloseGraphButtonClicked()
        {
            graph.SetActive(false);
        }
    }
}