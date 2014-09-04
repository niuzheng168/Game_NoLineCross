namespace NoLineCross.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    using NoLineCross.Controls;

    using WpfCommon.ModelBase;

    /// <summary>
    ///  The game view model.
    /// </summary>
    public class GameViewModel : ViewModelBase
    {
        #region Constants

        /// <summary>
        /// The r.
        /// </summary>
        public const int R = 200;

        #endregion

        #region Fields

        public static readonly Random Ran = new Random();

        /// <summary>
        /// The _connected pairs.
        /// </summary>
        public List<Tuple<int, int>> _connectedPairs = new List<Tuple<int, int>>();

        /// <summary>
        /// The _vertex matrix.
        /// </summary>
        private int[,] _vertexMatrix;

        private int _vertexCount = 0;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///  Initializes a new instance of the <see cref="GameViewModel" /> class.
        /// </summary>
        public GameViewModel()
        {
            this.GamePoints = new List<PointViewModel>();
            this.GameLines = new List<LineViewModel>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///  Gets or sets the game lines.
        /// </summary>
        public List<LineViewModel> GameLines { get; set; }

        /// <summary>
        ///  Gets or sets the game points.
        /// </summary>
        public List<PointViewModel> GamePoints { get; set; }

        /// <summary>
        /// Gets or sets the game level.
        /// </summary>
        public int Level { get; set; }

        #endregion

        #region Public Methods and Operators

        public void GenerateGame(int level)
        {
            this.Level = level;

            this.ResetGame();

            this.GenerateGamePoints();

            this.GenerateGameLines();
        }

        private void GenerateGamePoints()
        {
            MainWindow window = this.View as MainWindow;
            int maxLeft = (int)window._gameCanvas.ActualWidth - PointViewModel.Radio * 2;
            int maxHeight = (int)window._gameCanvas.ActualHeight - PointViewModel.Radio * 2;

            for (int i = 0; i < _vertexCount; i++)
            {
                PointViewModel pv = new PointViewModel(i);
                int left = Ran.Next(maxLeft);
                int height = Ran.Next(maxHeight);
                pv.CurPosition = new Point(left, height);
                this.GamePoints.Add(pv);
            }
        }

        private void ResetGame()
        {
            this.GameLines.Clear();
            this.GamePoints.Clear();
            this._connectedPairs.Clear();
            this._vertexCount = this.Level * (this.Level - 1) / 2;
        }

        /// <summary>
        ///  The check state.
        /// </summary>
        public void WinStateCheck()
        {
            bool success = true;

            for (int i = 0; i < this.GameLines.Count; i++)
            {
                for (int j = i + 1; j < this.GameLines.Count; j++)
                {
                    var l1 = this.GameLines[i];
                    var l2 = this.GameLines[j];

                    if (l1.Point1.Id == l2.Point1.Id || l1.Point1.Id == l2.Point2.Id || l1.Point2.Id == l2.Point1.Id
                        || l1.Point2.Id == l2.Point2.Id)
                    {
                        continue;
                    }

                    if (this.IsTwoLineCorss(
                        l1.Point1.CurPosition, 
                        l1.Point2.CurPosition, 
                        l2.Point1.CurPosition, 
                        l2.Point2.CurPosition))
                    {
                        success = false;
                    }
                }
            }

            if (success)
            {
                if (MessageBox.Show("Win! Ready for next level?", "Win", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    MainWindow window = (this.View) as MainWindow;
                    window.JumpToNextLevel();
                }
            }
        }

        /// <summary>
        /// The check two line corss.
        /// </summary>
        /// <param name="p1">
        /// The p 1.
        /// </param>
        /// <param name="p2">
        /// The p 2.
        /// </param>
        /// <param name="p3">
        /// The p 3.
        /// </param>
        /// <param name="p4">
        /// The p 4.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsTwoLineCorss(Point p1, Point p2, Point p3, Point p4)
        {
            return this.IsCross(p1, p2, p3, p4) && this.IsCross(p3, p4, p1, p2);
        }

        /// <summary>
        /// The generate game lines.
        /// </summary>
        public void GenerateGameLines()
        {
            List<Point> points = this.GenerateTangentPointList(this.Level);

            Dictionary<int, TangentLine> tangentLines = new Dictionary<int, TangentLine>();
            for (int i = 0; i < points.Count; i++)
            {
                TangentLine line = this.GenerateTangentLine(i, points[i]);
                tangentLines.Add(i, line);
            }

            int intersectionCount = 0;
            int[,] lineMatrix = new int[this.Level, this.Level];
            Dictionary<int, Intersection> intersections = new Dictionary<int, Intersection>();
            for (int i = 0; i < this.Level; i++)
            {
                lineMatrix[i, i] = -1;
                for (int j = i + 1; j < this.Level; j++)
                {
                    Intersection intersection = this.CalculateIntersection(
                        intersectionCount, 
                        tangentLines[i], 
                        tangentLines[j]);
                    intersections.Add(intersectionCount, intersection);
                    lineMatrix[i, j] = intersectionCount;
                    lineMatrix[j, i] = intersectionCount;

                    intersectionCount++;
                }
            }

            for (int i = 0; i < this.Level; i++)
            {
                List<Intersection> tmpList = new List<Intersection>();
                for (int j = 0; j < this.Level; j++)
                {
                    if (j != i)
                    {
                        tmpList.Add(intersections[lineMatrix[i, j]]);
                    }
                }

                tmpList.QuickSort(0, tmpList.Count - 1);

                for (int j = 0; j < tmpList.Count - 1; j++)
                {
                    this._connectedPairs.Add(new Tuple<int, int>(tmpList[j].Id, tmpList[j + 1].Id));
                }
            }

            for (int i = 0; i < _connectedPairs.Count; i++)
            {
                PointViewModel p1 = GamePoints[_connectedPairs[i].Item1];
                PointViewModel p2 = GamePoints[_connectedPairs[i].Item2];
                LineViewModel line = this.ConnectTwoPoint(i, p1, p2);
                this.GameLines.Add(line);
            }
        }

        private LineViewModel ConnectTwoPoint(int lineId, PointViewModel p1, PointViewModel p2)
        {
            LineViewModel line = new LineViewModel(lineId, p1, p2);
            p1.Lines.Add(line);
            p2.Lines.Add(line);

            p1.Neighbors.Add(p2);
            p2.Neighbors.Add(p1);

            return line;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The calculate intersection.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="line1">
        /// The line 1.
        /// </param>
        /// <param name="line2">
        /// The line 2.
        /// </param>
        /// <returns>
        /// The <see cref="Intersection"/>.
        /// </returns>
        private Intersection CalculateIntersection(int id, TangentLine line1, TangentLine line2)
        {
            Intersection intersection = new Intersection();
            intersection.Id = id;
            intersection.LineId1 = line1.Id;
            intersection.LineId2 = line2.Id;

            Point p = new Point();
            p.X = (line1.K * line1.TanPoint.X - line2.K * line2.TanPoint.X + line2.TanPoint.Y - line1.TanPoint.Y)
                  / (line1.K - line2.K);
            p.Y = line1.K * (p.X - line1.TanPoint.X) + line1.TanPoint.Y;

            double debug = line2.K * (p.X - line2.TanPoint.X) + line2.TanPoint.Y;

            intersection.Coordinate = p;

            return intersection;
        }

        /// <summary>
        /// The check crose.
        /// </summary>
        /// <param name="p1">
        /// The p 1.
        /// </param>
        /// <param name="p2">
        /// The p 2.
        /// </param>
        /// <param name="p3">
        /// The p 3.
        /// </param>
        /// <param name="p4">
        /// The p 4.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsCross(Point p1, Point p2, Point p3, Point p4)
        {
            Point v1 = new Point();

            Point v2 = new Point();

            Point v3 = new Point();

            v1.X = p3.X - p2.X;

            v1.Y = p3.Y - p2.Y;

            v2.X = p4.X - p2.X;

            v2.Y = p4.Y - p2.Y;

            v3.X = p1.X - p2.X;

            v3.Y = p1.Y - p2.Y;

            return (this.CrossMul(v1, v3) * this.CrossMul(v2, v3) <= 0);
        }

        /// <summary>
        /// The cross mul.
        /// </summary>
        /// <param name="pt1">
        /// The pt 1.
        /// </param>
        /// <param name="pt2">
        /// The pt 2.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private double CrossMul(Point pt1, Point pt2)
        {
            return pt1.X * pt2.Y - pt1.Y * pt2.X;
        }


        private TangentLine GenerateTangentLine(int id, Point point)
        {
            TangentLine line = new TangentLine();
            line.Id = id;
            line.TanPoint = point;
            line.K = line.TanPoint.X / line.TanPoint.Y * -1;
            return line;
        }

        private List<Point> GenerateTangentPointList(int count)
        {
            List<Point> points = new List<Point>();
            List<int> _pointYPool = new List<int>();

            for (int i = 0; i < count; i++)
            {
                Point p = new Point();
                while (true)
                {
                    int coorY = Ran.Next(1, R);
                    if (!_pointYPool.Contains(coorY))
                    {
                        p.Y = coorY;
                        p.X = Math.Sqrt(R * R - p.Y * p.Y);

                        int symbol = Ran.Next(2);
                        if (symbol == 0)
                        {
                            p.X *= -1;
                        }

                        symbol = Ran.Next(1);
                        if (symbol == 0)
                        {
                            p.Y *= -1;
                        }

                        points.Add(p);
                        _pointYPool.Add((int)p.Y);
                        break;
                    }
                }
            }

            return points;
        }

        #endregion
    }

    /// <summary>
    /// The intersection.
    /// </summary>
    public class Intersection : IComparable<Intersection>
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the coordinate.
        /// </summary>
        public Point Coordinate { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the line id 1.
        /// </summary>
        public int LineId1 { get; set; }

        /// <summary>
        /// Gets or sets the line id 2.
        /// </summary>
        public int LineId2 { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The compare to.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int CompareTo(Intersection other)
        {
            return (int)(this.Coordinate.X - other.Coordinate.X);
        }

        #endregion
    }

    /// <summary>
    /// The tangent line.
    /// </summary>
    public class TangentLine
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the k.
        /// </summary>
        public double K { get; set; }

        /// <summary>
        /// Gets or sets the tan point.
        /// </summary>
        public Point TanPoint { get; set; }

        #endregion
    }

    /// <summary>
    /// The sort.
    /// </summary>
    public static class Sort
    {
        #region Public Methods and Operators

        /// <summary>
        /// The quick sort.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public static void QuickSort<T>(this IList<T> list, int left, int right) where T : IComparable<T>
        {
            int i = left;
            int j = right;

            int pivotIndex = (left + right) / 2;
            T pivot = list[pivotIndex];

            while (i < j)
            {
                while (list[i].CompareTo(pivot) < 0)
                {
                    i++;
                }

                while (list[j].CompareTo(pivot) > 0)
                {
                    j--;
                }

                if (i < j)
                {
                    T tmp = list[i];
                    list[i] = list[j];
                    list[j] = tmp;

                    i++;
                    j--;
                }
            }

            if (left < j)
            {
                QuickSort(list, left, j);
            }

            if (j + 1 < right)
            {
                QuickSort(list, j + 1, right);
            }
        }

        #endregion
    }
}