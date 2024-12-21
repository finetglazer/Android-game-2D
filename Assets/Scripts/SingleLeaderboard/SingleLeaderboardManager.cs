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
                        new Vector3(prevRowPosition.x, prevRowPosition.y - 94, prevRowPosition.z);
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
            print("adad");
            SceneManager.LoadScene("Scenes/DashboardScene");
        }

        private void OnMoreButtonClicked(int index)
        {
            graph.SetActive(true);
            graph.transform.Find("GraphContainerBorder").gameObject.SetActive(true);
            var graphContainer = graph.transform.Find("GraphContainer");
            graphContainer.gameObject.SetActive(true);
            graphContainer.transform.Find("CloseGraphButton").GetComponent<Button>().onClick.AddListener(OnCloseGraphButtonClicked);
            var graphDrawer = graphContainer.GetComponent<SingleLeaderboardTriangleGraphDrawer>();
            graphDrawer.playerValues[0] = _finalPointsList[index] / 100;
            graphDrawer.playerValues[1] = _finishTimeList[index] / _finishTimeList[0];
            var minDeathCount = _deathCountList.Min();
            graphDrawer.playerValues[2] = minDeathCount * 1.0f / _deathCountList[index];
            graphDrawer.finalPointsText.text = graphDrawer.playerValues[0].ToString("F1");
            graphDrawer.finishTimeText.text = graphDrawer.playerValues[1].ToString("F1");
            graphDrawer.deathCountText.text = graphDrawer.playerValues[2].ToString("F1");
        }

        private void OnCloseGraphButtonClicked()
        {
            graph.SetActive(false);
        }
    }
}