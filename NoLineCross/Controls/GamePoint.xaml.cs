namespace NoLineCross.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using NoLineCross.ViewModel;

    using WpfCommon.ModelBase;
    using WpfCommon.ViewBase;

    /// <summary>
    ///     Interaction logic for GamePoint.xaml
    /// </summary>
    public partial class GamePoint : UserControl, IView
    {
        public delegate void PointMoveEventHandler();

        public event PointMoveEventHandler OnPointMove;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePoint"/> class.
        /// </summary>
        public GamePoint()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public ViewModelBase ViewModel
        {
            get
            {
                return this.DataContext as ViewModelBase;
            }
            set
            {
                this.DataContext = value;
            }
        }

        public void OnPositionChanged(Point lastPosition)
        {
            PointViewModel viewModel = this.ViewModel as PointViewModel;
            viewModel.CurPosition = lastPosition;
            foreach (LineViewModel line in viewModel.Lines)
            {
                line.CalculateLinePoints();
            }
        }

        public void OnPositionChangedOver()
        {
            if (OnPointMove != null)
            {
                OnPointMove();
            }
        }

        #endregion

        public void Highlight()
        {
            this._moveThumb.Highlight = true;
        }

        public void DisHighlight()
        {
            this._moveThumb.Highlight = false;
        }
    }
}