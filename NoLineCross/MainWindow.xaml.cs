namespace NoLineCross
{
    using System.Windows;
    using System.Windows.Controls;

    using NoLineCross.Controls;
    using NoLineCross.ViewModel;

    using WpfCommon.ModelBase;
    using WpfCommon.ViewBase;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IView
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.ViewModel = new GameViewModel();
            this.ViewModel.View = this;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the view model.
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

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Jump To Next Level.
        /// </summary>
        public void JumpToNextLevel()
        {
            GameViewModel gameViewModel = this.ViewModel as GameViewModel;

            int level;
            if (int.TryParse(this._textbox.Text, out level) == false)
            {
                level = 4;
            }

            gameViewModel.GenerateGame(level + 1);
            this._textbox.Text = (level + 1).ToString();

            this.SyncViewModelToUI();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The button base_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            GameViewModel gameViewModel = this.ViewModel as GameViewModel;

            int level;
            if (int.TryParse(this._textbox.Text, out level) == false)
            {
                level = 4;
            }

            gameViewModel.GenerateGame(level);

            this.SyncViewModelToUI();
        }

        /// <summary>
        /// The button debug_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonDebug_OnClick(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        ///     Sync ViewModel To UI.
        /// </summary>
        private void SyncViewModelToUI()
        {
            this._gameCanvas.Children.Clear();
            GameViewModel gameViewModel = this.ViewModel as GameViewModel;

            if (gameViewModel != null)
            {
                foreach (PointViewModel pointViewModel in gameViewModel.GamePoints)
                {
                    GamePoint point = new GamePoint();
                    point.Template = this.Resources["MoveThumbTemplate"] as ControlTemplate;
                    point.OnPointMoveCompleted += gameViewModel.WinStateCheck;

                    point.ViewModel = pointViewModel;
                    pointViewModel.View = point;

                    this._gameCanvas.Children.Add(point);
                    Canvas.SetLeft(point, pointViewModel.CurPosition.X);
                    Canvas.SetTop(point, pointViewModel.CurPosition.Y);
                    Panel.SetZIndex(point, 10);
                }

                foreach (LineViewModel lineViewModel in gameViewModel.GameLines)
                {
                    this._gameCanvas.Children.Add(lineViewModel.Segment);
                }
            }
        }

        #endregion
    }
}