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

        /// <summary>
        /// Sets default values of the view.
        /// </summary>
        public override void SetDefaultValues()
        {
            base.SetDefaultValues();
            ContentMargin.DirectValue = new ElementMargin();
            ResizeToContent.DirectValue = true;
            PropagateChildLayoutChanges.DirectValue = true;
        }

        public override bool CalculateLayoutChanges(LayoutChangeContext context) {

            if (!PropagateChildLayoutChanges.IsSet)
            {
                PropagateChildLayoutChanges.DirectValue = ResizeToContent;
            }

            if (!ResizeToContent)
                return Layout.IsDirty;

            var maxWidth = 0f;
            var maxHeight = 0f;
            var childCount = ChildCount;

            // get size of content and set content offsets and alignment
            for (var i = 0; i < childCount; ++i)
            {
                var go = transform.GetChild(i);
                var view = go.GetComponent<UIView>();
                if (view == null)
                    continue;

                // get size of content
                maxWidth = Mathf.Max(maxWidth, view.Layout.PixelWidth, view.Layout.Width.Pixels);
                maxHeight = Mathf.Max(maxHeight, view.Layout.PixelHeight, view.Layout.Height.Pixels);
            }

            // add margins
            maxWidth += ContentMargin.Value.Left.Pixels + ContentMargin.Value.Right.Pixels;
            maxHeight += ContentMargin.Value.Bottom.Pixels + ContentMargin.Value.Top.Pixels;

            // adjust size to content unless it has been set
            if (!Width.IsSet)
            {
                Layout.Width = new ElementSize(maxWidth);
            }

            if (!Height.IsSet)
            {
                Layout.Height = new ElementSize(maxHeight);
            }

            return base.CalculateLayoutChanges(context);
        }

        #endregion
    }
}
