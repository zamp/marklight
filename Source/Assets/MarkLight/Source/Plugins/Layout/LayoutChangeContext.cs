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

        private readonly HashSet<View> _dirtyViews = new HashSet<View>();

        #endregion

        #region Methods

        /// <summary>
        /// Calculate layout for the specified view.
        /// </summary>
        public bool Calculate(View view)
        {
            return CalculateLayout(view, ViewRelation.Source);
        }

        /// <summary>
        /// Ensures that a view whose layout calculations are required by another are already calculated.
        /// </summary>
        public bool CalculateRequired(View view)
        {
            // check if the view is already calculated
            if (_dirtyViews.Contains(view))
                return false;

            var isCalculated = false;
            view.ForThisAndEachChild<View>(x =>
            {
                isCalculated = CalculateLayout(view, ViewRelation.Required) || isCalculated;
                return true;
            }, new ViewSearchArgs
            {
                TraversalAlgorithm = TraversalAlgorithm.ReverseBreadthFirst,
                SkipInactive = true
            });
            return isCalculated;
        }

        /// <summary>
        /// Initiate layout rendering on all views whose recalculation in this context
        /// resulted in layout changes.
        /// </summary>
        public void RenderLayout()
        {
            foreach (var view in _dirtyViews)
            {
                if (view == null || view.IsDestroyed)
                    continue;

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
            var uiView = view as UIView;
            if (uiView == null)
                return false;

            var isCalculated = _dirtyViews.Contains(view);

            if (relation == ViewRelation.Required && isCalculated && !uiView.Layout.IsDirty)
                return false;

            // refresh the layout data the first time the views layout is calculated in this context.
            if (!isCalculated)
                uiView.RefreshLayoutData();

            var totalDirty = _dirtyViews.Count;

            if (!view.CalculateLayoutChanges(this) && totalDirty == _dirtyViews.Count) {
                return false; // return false, unless new dirty views were added
            }
            _dirtyViews.Add(view);
            return true;
        }

        #endregion

        #region Enum

        private enum ViewRelation
        {
            Source,
            Required
        }

        #endregion
    }
}