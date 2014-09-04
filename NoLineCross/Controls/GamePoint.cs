namespace NoLineCross.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    using NoLineCross.ViewModel;

    using WpfCommon.ModelBase;
    using WpfCommon.ViewBase;

    /// <summary>
    ///     The game point.
    /// </summary>
    public class GamePoint : Thumb, IView
    {
        #region Static Fields

        /// <summary>
        ///     The is highlight.
        /// </summary>
        public static readonly DependencyProperty IsHighlightProperty = DependencyProperty.Register(
            "IsHighlight", 
            typeof(bool), 
            typeof(GamePoint), 
            new FrameworkPropertyMetadata(false));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GamePoint" /> class.
        /// </summary>
        public GamePoint()
        {
            this.DragDelta += this.MoveThumb_DragDelta;
            this.DragCompleted += this.MoveThumb_DragCompleted;
        }

        #endregion

        #region Delegates

        /// <summary>
        /// The point move event handler.
        /// </summary>
        public delegate void PointMoveEventHandler();

        #endregion

        #region Public Events

        /// <summary>
        /// The on point move.
        /// </summary>
        public event PointMoveEventHandler OnPointMoveCompleted;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether highlight.
        /// </summary>
        public bool IsHighlight
        {
            get
            {
                return (bool)this.GetValue(IsHighlightProperty);
            }

            set
            {
                this.SetValue(IsHighlightProperty, value);
            }
        }

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
        /// The find parent.
        /// </summary>
        /// <param name="child">
        /// The child.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            // get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // we've reached the end of the tree
            if (parentObject == null)
            {
                return null;
            }

            // check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }

            return FindParent<T>(parentObject);
        }

        /// <summary>
        /// The on position changed.
        /// </summary>
        /// <param name="lastPosition">
        /// The last position.
        /// </param>
        public void OnPointMoving(Point lastPosition)
        {
            PointViewModel viewModel = this.ViewModel as PointViewModel;
            viewModel.CurPosition = lastPosition;
            foreach (LineViewModel line in viewModel.Lines)
            {
                line.RefreshSegmentCoordinate();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The move thumb_ drag completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MoveThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            GamePoint point = sender as GamePoint;

            if (point != null)
            {
                PointViewModel pv = point.ViewModel as PointViewModel;
                foreach (PointViewModel nei in pv.Neighbors)
                {
                    GamePoint p = nei.View as GamePoint;
                    p.IsHighlight = false;
                }

                if (this.OnPointMoveCompleted != null)
                {
                    this.OnPointMoveCompleted();
                }
            }
        }

        /// <summary>
        /// The move thumb_ drag delta.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            GamePoint point = sender as GamePoint;

            // MoveThumb point = FindParent<MoveThumb>(curElement);
            if (point != null)
            {
                PointViewModel pv = point.ViewModel as PointViewModel;
                foreach (PointViewModel nei in pv.Neighbors)
                {
                    GamePoint p = nei.View as GamePoint;
                    p.IsHighlight = true;
                }

                Canvas c = point.Parent as Canvas;
                double minLeft = 0;
                double maxLeft = c.ActualWidth - (PointViewModel.Radio * 2);
                double minTop = 0;
                double maxTop = c.ActualHeight - (PointViewModel.Radio * 2);

                double left = Canvas.GetLeft(point);
                double top = Canvas.GetTop(point);

                double newLeft = left + e.HorizontalChange;
                double newTop = top + e.VerticalChange;

                if (newLeft > maxLeft)
                {
                    newLeft = maxLeft;
                }

                if (newLeft < minLeft)
                {
                    newLeft = minLeft;
                }

                if (newTop > maxTop)
                {
                    newTop = maxTop;
                }

                if (newTop < minTop)
                {
                    newTop = minTop;
                }

                Canvas.SetLeft(point, newLeft);
                Canvas.SetTop(point, newTop);

                point.OnPointMoving(new Point(newLeft, newTop));
            }
        }

        #endregion
    }
}