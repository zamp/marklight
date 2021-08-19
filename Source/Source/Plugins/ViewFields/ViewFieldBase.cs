using System;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Base class for dependency view fields.
    /// </summary>
    public class ViewFieldBase
    {
        #region Fields

        public event EventHandler ValueSet;

        [SerializeField]
        protected bool _isSet;

        [SerializeField]
        private View _ownerView;

        [SerializeField]
        private string _path;

        [SerializeField]
        private bool _isMapped;

        [NonSerialized]
        private bool _isMappedSet;

        #endregion

        #region Methods

        /// <summary>
        /// Triggers the ValueSet event.
        /// </summary>
        public void TriggerValueSet()
        {
            if (ValueSet != null)
            {
                ValueSet(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the fields parent View.
        /// </summary>
        /// <exception cref="InvalidOperationException">When trying to set more than once.</exception>
        public View OwnerView
        {
            get { return _ownerView; }
            set
            {
                if (_ownerView != null)
                    throw new InvalidOperationException("ParentView can only be set once.");

                _ownerView = value;
            }
        }

        /// <summary>
        /// Get the field path.
        /// </summary>
        /// <exception cref="InvalidOperationException">When trying to set more than once.</exception>
        public string Path
        {
            get { return _path; }
            set
            {
                if (_path != null)
                    throw new InvalidOperationException("Path can only be set once.");

                _path = value;
            }
        }

        /// <summary>
        /// Determine if the field is mapped to another path.
        /// </summary>
        /// <exception cref="InvalidOperationException">When trying to set more than once.</exception>
        public bool IsMapped
        {
            get { return _isMapped; }
            set
            {
                if (_isMappedSet)
                    throw new InvalidOperationException("IsMapped can only be set once.");

                _isMapped = value;
                _isMappedSet = true;
            }
        }

        #endregion

    }
}
