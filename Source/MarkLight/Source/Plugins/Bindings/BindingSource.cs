
namespace MarkLight
{
    /// <summary>
    /// Binding source.
    /// </summary>
    public abstract class BindingSource
    {
        #region Fields

        private BindingValueObserver _observer;

        #endregion

        #region Methods

        /// <summary>
        /// Gets value from binding source.
        /// </summary>
        public abstract object GetValue(out bool hasValue);

        /// <summary>
        /// Set the source observer.
        /// </summary>
        /// <param name="observer"></param>
        public virtual bool SetObserver(BindingValueObserver observer)
        {
            if (!OnObserverChange(_observer, observer))
                return false;

            if (_observer != null)
                _observer.RemoveBindingSource(this);

            _observer = observer;
            if (_observer != null)
                observer.AddBindingSource(this);

            return true;
        }

        /// <summary>
        /// Called when the observer is changed.
        /// </summary>
        protected virtual bool OnObserverChange(BindingValueObserver oldObserver, BindingValueObserver observer)
        {
            return true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets binding source string.
        /// </summary>
        public abstract string BindingSourceString { get; }

        /// <summary>
        /// Get the source's value observer.
        /// </summary>
        public BindingValueObserver Observer
        {
            get { return _observer; }
        }

        #endregion
    }
}
