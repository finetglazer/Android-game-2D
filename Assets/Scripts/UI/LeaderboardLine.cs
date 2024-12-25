using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class LeaderboardLineGraphDrawer : VisualElement
    {
        private readonly Vector2[] _points;
        private readonly int _maxY;
        private readonly int _minY;
        private readonly int _maxX;
        private readonly float _graphWidth;
        private readonly float _graphHeight;
        private readonly float _labelFontSize;

        public LeaderboardLineGraphDrawer(Vector2[] dataPoints, int turns, int minYValue, int maxYValue, float width, float height, float labelFontSizeValue)
        {
            _points = dataPoints;
            _maxX = turns;
            _minY = minYValue;
            _maxY = maxYValue;
            _graphWidth = width;
            _graphHeight = height;
            _labelFontSize = labelFontSizeValue;

            // Draw graph
            generateVisualContent += OnGenerateVisualContent;

            // Add axis labels
            AddAxisLabels();
            AddPointLabel();
        }

        private void OnGenerateVisualContent(MeshGenerationContext context)
        {
            var painter = context.painter2D;

            // Draw Axes
            painter.BeginPath();
            painter.MoveTo(new Vector2(50, 10)); // Y-axis start
            painter.LineTo(new Vector2(50, _graphHeight + 10)); // Y-axis end
            painter.MoveTo(new Vector2(50, _graphHeight + 10)); // X-axis start
            painter.LineTo(new Vector2(_graphWidth + 50, _graphHeight + 10)); // X-axis end
            painter.Stroke();

            // Draw Line Graph
            if (_points.Length > 0)
            {
                painter.BeginPath();
                painter.MoveTo(MapToGraph(_points[0]));
                for (var i = 1; i < _points.Length; i++)
                    painter.LineTo(MapToGraph(_points[i]));
                painter.Stroke();
            }

            // Draw Circles for Milestones
            foreach (var point in _points)
            {
                var mappedPoint = MapToGraph(point);
                DrawCircle(painter, mappedPoint, 3); // Circle with radius 3
            }
        }
    
        private void AddPointLabel()
        {
            foreach (var point in _points)
            {
                var mappedPoint = MapToGraph(point);

                var label = new Label(point.y.ToString("0.0"))
                {
                    style =
                    {
                        position = Position.Absolute,
                        left = mappedPoint.x + 5, // Offset slightly to the right
                        top = mappedPoint.y - 10, // Offset above the circle
                        fontSize = _labelFontSize - 1,
                        color = Color.black
                    }
                };

                Add(label); // Add the label to the visual tree
            }
        }

        private static void DrawCircle(Painter2D painter, Vector2 center, float radius)
        {
            const int segments = 20; // Number of line segments to approximate the circle
            const float angleStep = 360f / segments;

            painter.BeginPath();
            for (var i = 0; i <= segments; i++)
            {
                var angle = Mathf.Deg2Rad * (i * angleStep);
                var x = center.x + radius * Mathf.Cos(angle);
                var y = center.y + radius * Mathf.Sin(angle);
                if (i == 0)
                    painter.MoveTo(new Vector2(x, y));
                else
                    painter.LineTo(new Vector2(x, y));
            }
            painter.ClosePath();
            painter.Fill(); // Fills the circle with the current color
        }


        private void AddAxisLabels()
        {
            // Add Y-axis labels (Points)
            for (var i = 0; i <= 5; i++)
            {
                var yValue = Mathf.Lerp(_minY, _maxY, i / 5f);
                var yPos = _graphHeight + 10 - (i * _graphHeight / 5); // Map to graph height

                var label = new Label(yValue.ToString("0"))
                {
                    style =
                    {
                        position = Position.Absolute,
                        left = 10, // Near the Y-axis
                        top = yPos - 10, // Adjust for centering
                        fontSize = _labelFontSize,
                        color = Color.black
                    }
                };

                Add(label);
            }

            // Add X-axis labels (Turns)
            for (var i = 0; i <= _maxX; i++)
            {
                var xPos = 50 + (i * _graphWidth / _maxX); // Map to graph width

                var label = new Label(i.ToString())
                {
                    style =
                    {
                        position = Position.Absolute,
                        left = xPos - 10, // Adjust for centering
                        top = _graphHeight + 10, // Below the X-axis
                        fontSize = _labelFontSize,
                        color = Color.black
                    }
                };

                Add(label);
            }

            // Add Y-axis name ("Points")
            var yAxisName = new Label("Points")
            {
                style =
                {
                    position = Position.Absolute,
                    left = 5, // Near the Y-axis
                    top = -15, // Above the graph
                    fontSize = _labelFontSize + 2, // Slightly larger than axis labels
                    color = Color.black
                }
            };
            Add(yAxisName);

            // Add X-axis name ("Turn")
            var xAxisName = new Label("Turn")
            {
                style =
                {
                    position = Position.Absolute,
                    left = _graphWidth + 50, // Centered along the X-axis
                    top = _graphHeight + 10, // Below the graph
                    fontSize = _labelFontSize + 2, // Slightly larger than axis labels
                    color = Color.black
                }
            };
            Add(xAxisName);
        }


        private Vector2 MapToGraph(Vector2 point)
        {
            var x = 50 + (point.x / _maxX) * _graphWidth; // Scale X (turn)
            var y = _graphHeight + 10 + ((point.y - _minY) / (_maxY - _minY)) * _graphHeight; // Scale Y (points)
            return new Vector2(x, y);
        }
    }
}