namespace NoLineCross
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

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
            GameViewModel game = new GameViewModel();
            Random r = new Random();
            int lvl;
            int pointCount;
            if (int.TryParse(this._textbox.Text, out lvl) == false)
            {
                pointCount = 6;
            }
            else
            {
                pointCount = lvl * (lvl - 1) / 2;
            }

            game.GenerateGameLines(lvl);

            this._gameCanvas.Children.Clear();

            for (int i = 0; i < pointCount; i++)
            {
                GamePoint point = new GamePoint();
                point.OnPointMove += game.CheckState;
                PointViewModel pv = new PointViewModel(i);

                point.ViewModel = pv;
                pv.View = point;

                int left = r.Next((int)this._gameCanvas.ActualWidth - (PointViewModel.Radio * 2));
                int height = r.Next((int)this._gameCanvas.ActualHeight - (PointViewModel.Radio * 2));
                pv.CurPosition = new Point(left, height);

                this._gameCanvas.Children.Add(point);
                Canvas.SetLeft(point, pv.CurPosition.X);
                Canvas.SetTop(point, pv.CurPosition.Y);
                Canvas.SetZIndex(point, 10);
                game.GamePoints.Add(pv);
            }

            int lineCount = 0;

            foreach (Tuple<int, int> pair in game._connectedPairs)
            {
                int a = pair.Item1;
                int b = pair.Item2;
                LineViewModel lv = new LineViewModel(lineCount++, game.GamePoints[a], game.GamePoints[b]);
                lv.CalculateLinePoints();
                lv.Line.StrokeThickness = 2;
                lv.Line.Stroke = new SolidColorBrush(Colors.Black);
                this._gameCanvas.Children.Add(lv.Line);
                game.GameLines.Add(lv);
            }

            return;


            for (int i = 0; i < pointCount; i++)
            {
                int remainCount = 3 - game.GamePoints[i].Neighbors.Count;

                if (remainCount == 0 && i + 1 < pointCount)
                {
                    LineViewModel lv = new LineViewModel(lineCount++, game.GamePoints[i], game.GamePoints[i + 1]);
                    lv.CalculateLinePoints();
                    lv.Line.StrokeThickness = 2;
                    lv.Line.Stroke = new SolidColorBrush(Colors.Black);
                    this._gameCanvas.Children.Add(lv.Line);
                    game.GameLines.Add(lv);
                }

                for (int j = i + 1; j <= i + remainCount && j < pointCount; j++)
                {
                    LineViewModel lv = new LineViewModel(lineCount++, game.GamePoints[i], game.GamePoints[j]);
                    lv.CalculateLinePoints();
                    lv.Line.StrokeThickness = 2;
                    lv.Line.Stroke = new SolidColorBrush(Colors.Black);
                    this._gameCanvas.Children.Add(lv.Line);
                    game.GameLines.Add(lv);
                }
            }

            LineViewModel tail = new LineViewModel(lineCount++, game.GamePoints[pointCount - 1], game.GamePoints[0]);
            tail.CalculateLinePoints();
            tail.Line.StrokeThickness = 2;
            tail.Line.Stroke = new SolidColorBrush(Colors.Black);
            this._gameCanvas.Children.Add(tail.Line);
            game.GameLines.Add(tail);
        }

        #endregion

        private void ButtonDebug_OnClick(object sender, RoutedEventArgs e)
        {
            GameViewModel game = new GameViewModel();
            Random r = new Random();
            int lvl;
            int pointCount;
            if (int.TryParse(this._textbox.Text, out lvl) == false)
            {
                pointCount = 6;
            }
            else
            {
                pointCount = lvl * (lvl - 1) / 2;
            }
            this._gameCanvas.Children.Clear();
            game.GenerateGameLines(lvl);

            for (int i = 0; i < pointCount; i++)
            {
                GamePoint point = new GamePoint();
                point.OnPointMove += game.CheckState;
                PointViewModel pv = new PointViewModel(i);

                point.ViewModel = pv;
                pv.View = point;

                double left = (game._intersections[i].Coordinate.X + 400);
                double height = (game._intersections[i].Coordinate.Y + 600);
                pv.CurPosition = new Point(left, height);

                this._gameCanvas.Children.Add(point);
                Canvas.SetLeft(point, pv.CurPosition.X);
                Canvas.SetTop(point, pv.CurPosition.Y);
                Canvas.SetZIndex(point, 10);
                game.GamePoints.Add(pv);
            }

            int lineCount = 0;

            foreach (Tuple<int, int> pair in game._connectedPairs)
            {
                int a = pair.Item1;
                int b = pair.Item2;
                LineViewModel lv = new LineViewModel(lineCount++, game.GamePoints[a], game.GamePoints[b]);
                lv.CalculateLinePoints();
                lv.Line.StrokeThickness = 2;
                lv.Line.Stroke = new SolidColorBrush(Colors.Black);
                this._gameCanvas.Children.Add(lv.Line);
                game.GameLines.Add(lv);
            }
        }


    }
}