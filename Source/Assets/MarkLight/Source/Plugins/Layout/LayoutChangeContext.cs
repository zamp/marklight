using System.Collections.Generic;
using MarkLight.Views.UI;

namespace MarkLight
{
    /// <summary>
    /// Context responsible for tracking and managing layout recalculation  propagation and rendering.
    /// </summary>
    public class LayoutChangeContext
    {
        #region Fields

        private readonly View _source;
        private readonly int _maxTries;
        private readonly int _maxCalculate;
        private readonly HashSet<View> _dirtyViews = new HashSet<View>();
        private int _tries;
        private int _calculates;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public LayoutChangeContext(View source, int maxTries = 10, int maxCalculate = 500)
        {
            _source = source;
            _maxTries = maxTries;
            _maxCalculate = maxCalculate;
        }

        #region Methods

        /// <summary>
        /// Calculate layout for the specified view.
        /// </summary>
        public bool Calculate(View view)
        {
            // increment tries if recalculating the view that initiated the layout change.
            if (Equals(view, _source))
            {
                _tries++;
                if (_tries > _maxTries)
                    return false;
            }

            // limit the total number of recalculations.
            _calculates++;
            if (_calculates > _maxCalculate)
                return false;

            // refresh the layout data the first time the views layout is calculated in this context.
            var uiView = view as UIView;
            if (uiView != null && !_dirtyViews.Contains(view))
            {
                uiView.RefreshLayoutData();
            }

            var totalDirty = _dirtyViews.Count;
            if (!view.CalculateLayoutChanges(this) && totalDirty == _dirtyViews.Count) {
                return false; // return false, unless new dirty views were added
            }

            _dirtyViews.Add(view);

            // notify parent of child recalculation.
            var parent = view.LayoutParent;
            if (parent != null) {
                parent.NotifyChildLayoutCalculated(view, this);
            }

            if (!view.PropagateChildLayoutChanges)
                return true;

            // notify children about recalculation.
            view.ForEachChild<View>(x =>
            {
                x.NotifyParentLayoutCalculated(view, this);
            }, false);

            return true;
        }

        /// <summary>
        /// Initiate layout rendering on all views whose recalculation in this context
        /// resulted in layout changes.
        /// </summary>
        public void RenderLayout()
        {
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

        #endregion
    }
}