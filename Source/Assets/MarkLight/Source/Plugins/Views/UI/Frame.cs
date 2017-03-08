using UnityEngine;

namespace MarkLight.Views.UI
{
    /// <summary>
    /// Frame view.
    /// </summary>
    /// <d>The frame resizes itself to its content by default.</d>
    [HideInPresenter]
    public class Frame : UIView
    {
        #region Fields

        /// <summary>
        /// Indicates if the view should resize to content.
        /// </summary>
        /// <d>Resizes the view to the size of its children.</d>
        [ChangeHandler("LayoutChanged")]
        public _bool ResizeToContent;

        /// <summary>
        /// Content margin.
        /// </summary>
        /// <d>The margin of the content of this view.</d>
        [ChangeHandler("LayoutChanged")]
        public _ElementMargin ContentMargin;

        #endregion

        #region Methods

        public override void InitializeInternalDefaultValues() {
            base.InitializeInternalDefaultValues();

            LayoutCalculator = new FrameLayoutCalculator();
        }

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();
            ContentMargin.DirectValue = new ElementMargin();
            ResizeToContent.DirectValue = true;
        }

        public override bool CalculateLayoutChanges(LayoutChangeContext context) {

            if (!ResizeToContent)
                return Layout.IsDirty;

            var layout = LayoutCalculator as FrameLayoutCalculator;
            if (layout != null)
            {
                layout.ContentMargin = ContentMargin.Value;
            }

            return base.CalculateLayoutChanges(context);
        }

        #endregion
    }
}
