using UnityEngine;

namespace MarkLight
{   
    /// <summary>
    /// Generic base class for dependency view fields.
    /// </summary>
    public class ViewField<T> : ViewFieldBase
    {
        #region Fields

        [SerializeField]
        protected T _internalValue;

        #endregion

        #region Methods

        /// <summary>
        /// Notify that current value has been modified without being set.
        /// </summary>
        public virtual void NotifyModified() {
            TriggerValueSet();
            if (OwnerView != null)
                OwnerView.Fields.NotifyDependentValueObservers(Path, true);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets view field notifying observers if the value has changed.
        /// </summary>
        public virtual T Value
        {
            get
            {
                if (OwnerView != null && IsMapped)
                {
                    return (T)OwnerView.Fields.GetValue(Path);
                }

                return _internalValue;
            }
            set
            {
                if (OwnerView != null)
                {
                    OwnerView.Fields.SetValue(Path, value);
                }
                else
                {
                    InternalValue = value;
                    _isSet = true;
                }                
            }
        }

        /// <summary>
        /// Gets or sets view field notifying observers if the value has changed.
        /// </summary>
        public virtual object ObjectValue
        {
            get
            {
                if (OwnerView != null && IsMapped)
                {
                    return OwnerView.Fields.GetValue(Path);
                }

                return _internalValue;
            }
            set
            {
                if (OwnerView != null)
                {
                    OwnerView.Fields.SetValue(Path, value);
                }
                else
                {
                    InternalValue = (T)value;
                    _isSet = true;
                }
            }
        }

        /// <summary>
        /// Sets view field directly without notifying observers that the value has changed.
        /// </summary>
        public virtual T DirectValue
        {
            set
            {
                if (OwnerView != null && IsMapped)
                {
                    OwnerView.Fields.SetValue(Path, value, true, null, null, false);
                }
                else
                {
                    _internalValue = value;
                    _isSet = true;
                }
            }
        }

        /// <summary>
        /// Sets view field directly without notifying observers that the value has changed.
        /// </summary>
        public virtual object DirectObjectValue
        {
            set
            {
                if (OwnerView != null && IsMapped)
                {
                    OwnerView.Fields.SetValue(Path, value, true, null, null, false);
                }
                else
                {
                    _internalValue = (T)value;
                    _isSet = true;
                }
            }
        }

        /// <summary>
        /// Gets boolean indicating if the value has been set. 
        /// </summary>
        public virtual bool IsSet
        {
            get
            {
                return OwnerView != null ? OwnerView.Fields.IsSet(Path) : _isSet;
            }
        }

        /// <summary>
        /// Gets or sets internal value without considering mappings and without notifying observers.
        /// </summary>
        public virtual T InternalValue
        {
            get
            {
                return _internalValue;
            }
            set
            {
                _internalValue = value;
                TriggerValueSet();
            }
        }

        #endregion
    }
}
