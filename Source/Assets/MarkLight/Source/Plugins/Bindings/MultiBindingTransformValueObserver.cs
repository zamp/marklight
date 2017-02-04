using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Binding value observer for a Multi-transform binding.
    /// </summary>
    public class MultiBindingTransformValueObserver : BindingValueObserver
    {

        #region Fields

        private readonly MethodInfo _transformMethod;
        private readonly View _parentView;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public MultiBindingTransformValueObserver(ViewFieldData target, MethodInfo transformMethod, View parentView)
                                                  : base(target)
        {
            _transformMethod = transformMethod;
            _parentView = parentView;
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

                var pars = Sources.Count > 0 ? new object[Sources.Count] : null;
                if (pars == null)
                    return false;

                for (var i = 0; i < pars.Length; ++i)
                {
                    bool hasValue;
                    pars[i] = Sources[i].GetValue(out hasValue);
                }

                // set transformed value
                Target.SetValue(
                    _transformMethod.IsStatic
                        ? _transformMethod.Invoke(null, pars)
                        : _transformMethod.Invoke(_parentView, pars), callstack);
            }
            catch (Exception e)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] Exception thrown when propagating multi transform binding value from "+
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
        /// Get the transform method info.
        /// </summary>
        public MethodInfo TransformMethod
        {
            get { return _transformMethod; }
        }

        /// <summary>
        /// Get the parent view that the transform method is from.
        /// </summary>
        public View ParentView
        {
            get { return _parentView; }
        }

        #endregion
    }
}