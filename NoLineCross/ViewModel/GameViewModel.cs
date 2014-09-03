namespace NoLineCross.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    using WpfCommon.ModelBase;

    /// <summary>
    ///     The game view model.
    /// </summary>
    public class GameViewModel : ViewModelBase
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameViewModel" /> class.
        /// </summary>
        public GameViewModel()
        {
            this.GamePoints = new List<PointViewModel>();
            this.GameLines = new List<LineViewModel>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the game lines.
        /// </summary>
        public List<LineViewModel> GameLines { get; set; }

        /// <summary>
        ///     Gets or sets the game points.
        /// </summary>
        public List<PointViewModel> GamePoints { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The check state.
        /// </summary>
        public void CheckState()
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

                    if (this.CheckTwoLineCorss(
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
        public bool CheckTwoLineCorss(Point p1, Point p2, Point p3, Point p4)
        {
            return this.CheckCrose(p1, p2, p3, p4) && this.CheckCrose(p3, p4, p1, p2);
        }

        #endregion

        #region Methods

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
        private bool CheckCrose(Point p1, Point p2, Point p3, Point p4)
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

        private List<int> _tangentPointYList = new List<int>();
        private Dictionary<int, TangentLine> _tangentLines = new Dictionary<int, TangentLine>();
        public Dictionary<int, Intersection> _intersections = new Dictionary<int, Intersection>();
        private int[,] _lineMatrix = null;
        private int[,] _vertexMatrix = null;
        public List<Tuple<int, int>> _connectedPairs = new List<Tuple<int, int>>();

        private const int R = 200;
        public void GenerateGameLines(int lineCount)
        {
            _tangentLines.Clear();
            _intersections.Clear();
            _tangentPointYList.Clear();
            int vertexCount = lineCount * (lineCount - 1) / 2;
            this._lineMatrix = new int[lineCount, lineCount];
            this._vertexMatrix = new int[vertexCount, vertexCount];
            _connectedPairs.Clear();

            for (int i = 0; i < lineCount; i++)
            {
                TangentLine line = this.GenerateTangentLine(i);
                _tangentLines.Add(i, line);
            }

            int intersectionCount = 0;
            for (int i = 0; i < lineCount; i++)
            {
                _lineMatrix[i, i] = -1;
                for (int j = i + 1; j < lineCount; j++)
                {
                    Intersection intersection = this.CalculateIntersection(
                        intersectionCount,
                        _tangentLines[i],
                        _tangentLines[j]);
                    _intersections.Add(intersectionCount, intersection);
                    _lineMatrix[i, j] = intersectionCount;
                    _lineMatrix[j, i] = intersectionCount;

                    intersectionCount++;
                }
            }

            for (int i = 0; i < lineCount; i++)
            {
                List<Intersection> tmpList = new List<Intersection>();
                for (int j = 0; j < lineCount; j++)
                {
                    if (j != i)
                    {
                        tmpList.Add(_intersections[_lineMatrix[i, j]]);
                    }
                }
                tmpList.QuickSort(0, tmpList.Count - 1);

                for (int j = 0; j < tmpList.Count - 1; j++)
                {
                    _connectedPairs.Add(new Tuple<int, int>(tmpList[j].Id, tmpList[j + 1].Id));
                }
            }
        }

        private TangentLine GenerateTangentLine(int id)
        {
            TangentLine line = new TangentLine();
            line.Id = id;
            line.TanPoint = this.GenerateTangentPoint();
            line.K = line.TanPoint.X / line.TanPoint.Y * -1;
            return line;
        }

        private Point GenerateTangentPoint()
        {
            Random ran = new Random();

            Point p = new Point();

            while (true)
            {
                p.Y = ran.Next(1, R);
                if (!_tangentPointYList.Contains((int)p.Y))
                {
                    _tangentPointYList.Add((int)p.Y);
                    break;
                }
            }

            p.X = Math.Sqrt(R * R - p.Y * p.Y);

            int symbol = ran.Next(2);
            if (symbol == 0)
            {
                p.X *= -1;
            }

            symbol = ran.Next(1);
            if (symbol == 0)
            {
                p.Y *= -1;
            }

            return p;
        }

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

        #endregion
    }

    public class Intersection : IComparable<Intersection>
    {
        public int Id { get; set; }

        public Point Coordinate { get; set; }

        public int LineId1 { get; set; }

        public int LineId2 { get; set; }

        public int CompareTo(Intersection other)
        {
            return (int)(this.Coordinate.X - other.Coordinate.X);
        }
    }

    public class TangentLine
    {
        public int Id { get; set; }
        public Point TanPoint { get; set; }

        public double K { get; set; }
    }

    public static class Sort
    {
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
    }
}