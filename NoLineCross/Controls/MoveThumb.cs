namespace NoLineCross.Controls
{
    using System;
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
    public class MoveThumb : Thumb
    {
        public static readonly DependencyProperty IsHighlight = DependencyProperty.Register(
            "Highlight",
            typeof(bool),
            typeof(MoveThumb),
            new FrameworkPropertyMetadata(false));

        public bool Highlight
        {
            get
            {
                return (bool)this.GetValue(IsHighlight);
            }
            set
            {
                this.SetValue(IsHighlight, value);
            }
        }

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MoveThumb" /> class.
        /// </summary>
        public MoveThumb()
        {
            ResourceDictionary dict = new ResourceDictionary();
            Uri uri = new Uri("../Assets/styles.xaml", UriKind.Relative);
            dict.Source = uri;
            Application.Current.Resources.MergedDictionaries.Add(dict);
            ControlTemplate ct = (ControlTemplate)Application.Current.Resources["MoveThumbTemplate"];

            this.DragDelta += this.MoveThumb_DragDelta;
            this.DragCompleted += this.MoveThumb_DragCompleted;
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
            UIElement curElement = sender as UIElement;

            GamePoint point = FindParent<GamePoint>(curElement);

            if (point != null)
            {
                PointViewModel pv = point.ViewModel as PointViewModel;
                foreach (PointViewModel nei in pv.Neighbors)
                {
                    GamePoint p = nei.View as GamePoint;
                    p.DisHighlight();
                }

                point.OnPositionChangedOver();
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
            UIElement curElement = sender as UIElement;

            GamePoint point = FindParent<GamePoint>(curElement);

            if (point != null)
            {
                PointViewModel pv = point.ViewModel as PointViewModel;
                foreach (PointViewModel nei in pv.Neighbors)
                {
                    GamePoint p = nei.View as GamePoint;
                    p.Highlight();
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

                point.OnPositionChanged(new Point(newLeft, newTop));
            }
        }

        #endregion
    }
}