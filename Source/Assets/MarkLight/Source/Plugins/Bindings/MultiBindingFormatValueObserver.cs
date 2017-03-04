using System;
using System.Collections.Generic;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Binding value observer for a Multi-binding format string.
    /// </summary>
    public class MultiBindingFormatValueObserver : BindingValueObserver
    {
        #region Fields

        private readonly string _formatString;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiBindingFormatValueObserver(ViewFieldData target, string formatString) : base(target)
        {
            _formatString = formatString;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Notifies the binding value observer that value has changed.
        /// </summary>
        public override bool Notify(HashSet<ViewFieldData> callstack)
        {
            try
            {
                base.Notify(callstack);

                // check if target has been destroyed
                if (Target.SourceView == null)
                {
                    return false;
                }

                //Debug.Log(String.Format("Source(s) updated. Updating target field: {0}", Target.ViewFieldPath));
                var formatPars = Sources.Count > 0
                    ? new object[Sources.Count]
                    : null;

                if (formatPars == null)
                    return false;

                for (var i = 0; i < formatPars.Length; ++i)
                {
                    bool hasValue;
                    formatPars[i] = Sources[i].GetValue(out hasValue);
                }

                // set format string value
                Target.SetValue(new ViewFieldValue(String.Format(_formatString, formatPars), callstack));
            }
            catch (Exception e)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] Exception thrown when propagating multi format binding value from "+
                    "sources \"{0}\" to target \"{1}.{2}\": {3}",
                    JoinSources(), Target.SourceView.ViewTypeName, Target.Path, Utils.GetError(e)));
            }

            return true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the binding type.
        /// </summary>
        public override BindingType BindingType
        {
            get { return BindingType.MultiBindingTransform; }
        }

        /// <summary>
        /// Determine if the observer can only handle one-way bindings.
        /// </summary>
        public override bool IsOneWayOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Get the format string used to format the binding values.
        /// </summary>
        public string FormatString
        {
            get { return _formatString; }
        }

        #endregion
    }
}