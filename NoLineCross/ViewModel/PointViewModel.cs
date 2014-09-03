namespace NoLineCross.ViewModel
{
    using System.Collections.Generic;
    using System.Windows;

    using NoLineCross.Controls;

    using WpfCommon.ModelBase;

    /// <summary>
    ///     The point view model.
    /// </summary>
    public class PointViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// The radio.
        /// </summary>
        public const int Radio = 10;

        /// <summary>
        ///     The _cur position.
        /// </summary>
        private Point _curPosition;

        /// <summary>
        ///     The _id.
        /// </summary>
        private int _id = -1;

        /// <summary>
        ///     The _lines.
        /// </summary>
        private List<LineViewModel> _lines = new List<LineViewModel>();

        /// <summary>
        ///     The _neighbors.
        /// </summary>
        private List<PointViewModel> _neighbors = new List<PointViewModel>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PointViewModel"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public PointViewModel(int id)
        {
            this._id = id;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the cur position.
        /// </summary>
        public Point CurPosition
        {
            get
            {
                return this._curPosition;
            }

            set
            {
                this._curPosition = value;
            }
        }

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
        ///     Gets or sets the lines.
        /// </summary>
        public List<LineViewModel> Lines
        {
            get
            {
                return this._lines;
            }

            set
            {
                this._lines = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the neighbors.
        /// </summary>
        public List<PointViewModel> Neighbors
        {
            get
            {
                return this._neighbors;
            }

            set
            {
                this._neighbors = value;
                this.NotifyPropertyChanged();
            }
        }

        #endregion
    }
}