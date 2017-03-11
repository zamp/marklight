using System.Collections.Generic;
using MarkLight.Views.UI;

namespace MarkLight
{
    /// <summary>
    /// Context responsible for tracking and managing layout recalculation, propagation and rendering.
    /// </summary>
    public class LayoutChangeContext
    {
        #region Fields

        private readonly int _maxCalculate;
        private readonly HashSet<View> _dirtyViews = new HashSet<View>();
        private readonly List<View> _childCalculators = new List<View>(10);
        private int _calculates;
        private bool _isRendering;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public LayoutChangeContext(int maxCalculate = 500)
        {
            _maxCalculate = maxCalculate;
        }

        #region Methods

        /// <summary>
        /// Calculate layout for the specified view.
        /// </summary>
        public bool Calculate(View view)
        {
            return CalculateLayout(view, ViewRelation.Source);
        }

        /// <summary>
        /// Calculate layout for the specified view as a parent or ancestor of the source view.
        /// </summary>
        public bool CalculateAsParent(View view)
        {
            return CalculateLayout(view, ViewRelation.Parent);
        }

        /// <summary>
        /// Calculate layout for the specified view as a child or descendant of the source view.
        /// </summary>
        public bool CalculateAsChild(View view)
        {
            return CalculateLayout(view, ViewRelation.Child);
        }

        /// <summary>
        /// Initiate layout rendering on all views whose recalculation in this context
        /// resulted in layout changes.
        /// </summary>
        public void RenderLayout()
        {
            _isRendering = true;

            for (var i = _childCalculators.Count - 1; i >= 0; i--)
            {
                if (CalculateViewLayout(_childCalculators[i]))
                {
                    CalculateViewParent(_childCalculators[i]);
                }
            }

            foreach (var view in _dirtyViews) {
                view.RenderLayout();
            }
        }

        /// <summary>
        /// Used to notify the context that a view layout information has been updated. Used when
        /// a view modifies another views layout information and needs to ensure the context knows it
        /// has been modified.
        /// </summary>
        public void NotifyLayoutUpdated(View view, bool force = false)
        {
            var uiView = view as UIView;
            if (!force && uiView != null && !uiView.Layout.IsDirty)
                return;

            _dirtyViews.Add(view);
        }

        private bool CalculateLayout(View view, ViewRelation relation)
        {
            if (!CanCalculate())
                return false;

            RefreshLayoutData(view);

            // check if view layout is affected by its child view layouts
            if (!_isRendering && (view.LayoutCalculator.IsAffectedByChildren || view.LayoutCalculator.IsChildLayout))
            {
                // set aside for second and final recalculation
                _childCalculators.Add(view);
            }

            if (!CalculateViewLayout(view))
                return false;

            if (relation != ViewRelation.Child)
                CalculateViewParent(view);

            // notify children about recalculation.
            view.ForEachChild<View>(x =>
            {
                if (relation != ViewRelation.Parent || !_dirtyViews.Contains(x))
                    x.NotifyParentLayoutCalculated(view, this);
            }, false);

            return true;
        }

        private bool CanCalculate()
        {
            // limit the total number of recalculations.
            _calculates++;
            return _calculates <= _maxCalculate;
        }

        private void RefreshLayoutData(View view)
        {
            // refresh the layout data the first time the views layout is calculated in this context.
            var uiView = view as UIView;
            if (uiView != null && !_dirtyViews.Contains(view))
            {
                uiView.RefreshLayoutData();
            }
        }

        private bool CalculateViewLayout(View view)
        {
            var totalDirty = _dirtyViews.Count;

            if (!view.CalculateLayoutChanges(this) && totalDirty == _dirtyViews.Count) {
                return false; // return false, unless new dirty views were added
            }
            _dirtyViews.Add(view);
            return true;
        }

        private void CalculateViewParent(View view)
        {
            // notify parent of child recalculation.
            var parent = view.LayoutParent;
            if (parent != null) {
                parent.NotifyChildLayoutCalculated(view, this);
            }
        }

        #endregion

        #region Enum

        private enum ViewRelation
        {
            Source,
            Parent,
            Child
        }

        #endregion
    }
}