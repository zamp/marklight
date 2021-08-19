using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MarkLight
{
    /// <summary>
    /// Binding value observer.
    /// </summary>
    public abstract class BindingValueObserver : ValueObserver
    {
        #region Fields

        private readonly List<BindingSource> _sources;
        private readonly ViewFieldData _target;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        protected BindingValueObserver(ViewFieldData target)
        {
            _target = target;
            _sources = new List<BindingSource>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a binding source to the observer.
        /// </summary>
        internal void AddBindingSource(BindingSource source) {
            _sources.Add(source);
        }

        /// <summary>
        /// Remove a binding source from the observer.
        /// </summary>
        internal void RemoveBindingSource(BindingSource source) {
            _sources.Remove(source);
        }

        /// <summary>
        /// Join the binding source string in Sources into a comma delimited string.
        /// </summary>
        protected string JoinSources() {

            var sb = BufferPools.StringBuilders.Get();
            foreach (var source in Sources)
            {
                if (source != Sources[0])
                {
                    sb.Append(", ");
                }

                sb.AppendFormat(source.BindingSourceString);
            }

            var result = sb.ToString();
            BufferPools.StringBuilders.Recycle(sb);

            return result;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the binding type.
        /// </summary>
        public abstract BindingType BindingType { get; }

        /// <summary>
        /// Determine if the observer can only do one-way bindings or not.
        /// </summary>
        public abstract bool IsOneWayOnly { get; }

        /// <summary>
        /// Get a readonly list of binding sources.
        /// </summary>
        public ReadOnlyCollection<BindingSource> Sources
        {
            get { return _sources.AsReadOnly(); }
        }

        /// <summary>
        /// Get the view field target that source values will be applied to.
        /// </summary>
        public ViewFieldData Target
        {
            get { return _target; }
        }

        #endregion
    }
}
