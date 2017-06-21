using System.Collections.Generic;

namespace MarkLight.Views.UI
{
    /// <summary>
    /// Stack Panel view.
    /// </summary>
    /// <d>The StackPanel arranges child views next to each other either horizontally or vertically with no spacing.
    /// The child elements are sized to fill the StackPanel.</d>
    [HideInPresenter]
    public class StackPanel : UIView
    {
        #region Fields

        /// <summary>
        /// Orientation of the group.
        /// </summary>
        /// <d>The orientation of the group that determines how the child views will be arranged.</d>
        [ChangeHandler("LayoutChanged")]
        public _ElementOrientation Orientation;

        /// <summary>
        /// Sort direction.
        /// </summary>
        /// <d>If children has SortIndex set the sort direction determines which order the views should be arranged
        /// in the group.</d>
        [ChangeHandler("LayoutChanged")]
        public _ElementSortDirection SortDirection;

        /// <summary>
        /// Sets the visibility of children so they are made visible after they are arranged.
        /// </summary>
        /// <d>Boolean indicating that the group should set the visibility of children so they are only made visible
        /// after they are arranged.</d>
        public _bool SetChildVisibility;

        protected View _groupContentContainer;

        #endregion

        #region Methods

        public override void InitializeInternalDefaultValues()
        {
            base.InitializeInternalDefaultValues();

            LayoutCalculator = new UniformStackLayoutCalculator();
        }

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();

            Orientation.DirectValue = ElementOrientation.Vertical;
            SortDirection.DirectValue = ElementSortDirection.Ascending;
        }

        public override bool CalculateLayoutChanges(LayoutChangeContext context) {

            var layoutCalc = LayoutCalculator as UniformStackLayoutCalculator;
            if (layoutCalc != null)
            {
                layoutCalc.Orientation = Orientation;
                layoutCalc.SetChildVisibility = SetChildVisibility;
            }

            return base.CalculateLayoutChanges(context);
        }

        public override void Initialize()
        {
            if (SetChildVisibility)
            {
                // set inactive children as not visible
                Content.ForEachChild<UIView>(x =>
                {
                    if (!x.IsActive)
                    {
                        x.IsVisible.Value = false;
                    }
                }, ViewSearchArgs.NonRecursive);
            }

            base.Initialize();
        }

        protected override List<UIView> GetContentChildren(View content, List<UIView> output)
        {
            return GetContentChildren(content, SortDirection, output, _sortBuffer);
        }

        #endregion
    }
}