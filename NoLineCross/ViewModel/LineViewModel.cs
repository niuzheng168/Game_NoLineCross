namespace NoLineCross.ViewModel
{
    using System.Windows;
    using System.Windows.Shapes;

    using WpfCommon.ModelBase;

    /// <summary>
    ///     The line view model.
    /// </summary>
    public class LineViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        ///     The _id.
        /// </summary>
        private int _id = -1;

        /// <summary>
        ///     The _is highlight.
        /// </summary>
        private bool _isHighlight;

        /// <summary>
        ///     The _x 1.
        /// </summary>
        private double _x1;

        /// <summary>
        ///     The _x 2.
        /// </summary>
        private double _x2;

        /// <summary>
        ///     The _y 1.
        /// </summary>
        private double _y1;

        /// <summary>
        ///     The _y 2.
        /// </summary>
        private double _y2;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineViewModel"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="p1">
        /// The p 1.
        /// </param>
        /// <param name="p2">
        /// The p 2.
        /// </param>
        public LineViewModel(int id, PointViewModel p1, PointViewModel p2)
        {
            this._id = id;
            this.Point1 = p1;
            this.Point2 = p2;

            this.Point1.Neighbors.Add(p2);
            this.Point2.Neighbors.Add(p1);

            this.Point1.Lines.Add(this);
            this.Point2.Lines.Add(this);

            this.Line = new Line();
            this.CalculateLinePoints();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the id.
        /// </summary>
        public int Id
        {
            get
            {
                return this._id;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether is highlight.
        /// </summary>
        public bool IsHighlight
        {
            get
            {
                return this._isHighlight;
            }

            set
            {
                this._isHighlight = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the point 1.
        /// </summary>
        public PointViewModel Point1 { get; set; }

        /// <summary>
        ///     Gets or sets the point 2.
        /// </summary>
        public PointViewModel Point2 { get; set; }

        /// <summary>
        /// Gets or sets the x 1.
        /// </summary>
        public double X1
        {
            get
            {
                return this._x1;
            }

            set
            {
                this._x1 = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the x 2.
        /// </summary>
        public double X2
        {
            get
            {
                return this._x2;
            }

            set
            {
                this._x2 = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the y 1.
        /// </summary>
        public double Y1
        {
            get
            {
                return this._y1;
            }

            set
            {
                this._y1 = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the y 2.
        /// </summary>
        public double Y2
        {
            get
            {
                return this._y2;
            }

            set
            {
                this._y2 = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the line.
        /// </summary>
        public Line Line { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The calculate line points.
        /// </summary>
        public void CalculateLinePoints()
        {
            Point c1 = new Point(
                Point1.CurPosition.X + PointViewModel.Radio,
                Point1.CurPosition.Y + PointViewModel.Radio);

            Point c2 = new Point(
                Point2.CurPosition.X + PointViewModel.Radio,
                Point2.CurPosition.Y + PointViewModel.Radio);

            this.Line.X1 = c1.X;
            this.Line.Y1 = c1.Y;
            this.Line.X2 = c2.X;
            this.Line.Y2 = c2.Y;
        }

        #endregion
    }
}