using TMPro;
using UnityEngine;

namespace SingleLeaderboard
{
    public class SingleLeaderboardTriangleGraphDrawer : MonoBehaviour
    {
        public LineRenderer lineRenderer;
        [HideInInspector]public float[] playerValues = { 0.8f, 0.6f, 0.9f }; // top, left, right
        public float graphSize = 100f; // Scale of the graph

        // References to the 3 TextMeshPro UI Texts
        public TMP_Text finalPointsText;
        public TMP_Text deathCountText;
        public TMP_Text finishTimeText;

        private void Start()
        {
            DrawTriangle();
        }

        private void DrawTriangle()
        {
            // Ensure the LineRenderer has 4 points (3 points + returning to the first)
            lineRenderer.positionCount = 4;

            // Calculate the vertices of the triangle based on player values
            var vertices = new Vector3[4];
            vertices[0] = new Vector3(0, playerValues[0] * graphSize, 0); // Top point
            vertices[1] = new Vector3(-playerValues[1] * graphSize, -graphSize / 2, 0); // Bottom-left point
            vertices[2] = new Vector3(playerValues[2] * graphSize, -graphSize / 2, 0); // Bottom-right point
            vertices[3] = vertices[0]; // Close the triangle

            // Assign positions to LineRenderer
            for (var i = 0; i < vertices.Length; i++)
            {
                lineRenderer.SetPosition(i, vertices[i]);
            }

            PlaceTextAtVertex(finalPointsText, new Vector3(vertices[0].x + 120, vertices[0].y, vertices[0].z), playerValues[0]); // Top point
            PlaceTextAtVertex(finishTimeText, new Vector3(vertices[1].x + 40, vertices[1].y, vertices[1].z), playerValues[1]); // Bottom-left point
            PlaceTextAtVertex(deathCountText, new Vector3(vertices[2].x + 110, vertices[2].y, vertices[2].z), playerValues[2]); // Bottom-right point
        }
    
        private void PlaceTextAtVertex(TMP_Text text, Vector3 vertex, float value)
        {
            text.rectTransform.localPosition = vertex;
            text.text = value.ToString("F1");
        }

    }
}