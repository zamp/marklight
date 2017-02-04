using System;
using System.Collections.Generic;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Binding value observer for a single binding.
    /// </summary>
    public class SingleBindingValueObserver : BindingValueObserver
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SingleBindingValueObserver(ViewFieldData target) : base(target)
        {
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
                bool hasValue;

                // check if target has been destroyed
                if (Target.SourceView == null)
                {
                    return false;
                }

                //Debug.Log(String.Format("Source(s) updated. Updating target field: {0}", Target.ViewFieldPath));

                var value = Sources[0].GetValue(out hasValue);
                if (hasValue)
                {
                    // use to debug
                    //Debug.Log(String.Format("Propagating Value \"{4}\": {0}.{1} -> {2}.{3}", Sources[0].ViewFieldData.TargetView.ViewTypeName, Sources[0].ViewFieldData.TargetViewFieldPath,
                    //    Target.TargetView.ViewTypeName, Target.TargetViewFieldPath, value.ToString()));

                    // set value
                    Target.SetValue(value, callstack);
                }

            }
            catch (Exception e)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] Exception thrown when propagating single binding value from "+
                    "source \"{0}\" to target \"{1}.{2}\": {3}",
                    Sources[0].BindingSourceString, Target.SourceView.ViewTypeName, Target.Path,
                    Utils.GetError(e)));
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
            get { return BindingType.SingleBinding; }
        }

        /// <summary>
        /// Determine if the observer can only handle one-way bindings.
        /// </summary>
        public override bool IsOneWayOnly
        {
            get { return false; }
        }

        #endregion
    }
}