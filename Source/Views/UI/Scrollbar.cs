using UnityEngine;

namespace MarkLight.Views.UI
{
    /// <summary>
    /// Scrollbar view.
    /// </summary>
    /// <d>A scrollbar with a draggable handle.</d>
    [HideInPresenter]
    public class Scrollbar : UIView
    {
        #region Fields

        /// <summary>
        /// Orientation of the scrollbar.
        /// </summary>
        /// <d>Orientation of the scrollbar.</d>
        [ChangeHandler("LayoutChanged")]
        public _ElementOrientation Orientation;

        /// <summary>
        /// Breadth of the scrollbar.
        /// </summary>
        /// <d>Breadth of the scrollbar.</d>
        [ChangeHandler("LayoutChanged")]
        public _ElementSize Breadth;

        #region ScrollbarComponent

        /// <summary>
        /// Scrollbar scroll direction.
        /// </summary>
        /// <d>Scrollbar scroll direction.</d>
        [MapTo("ScrollbarComponent.direction")]
        public _ScrollbarDirection ScrollDirection;

        /// <summary>
        /// Scroll steps.
        /// </summary>
        /// <d>The number of steps to use for the value. A value of 0 disables use of steps.</d>
        [MapTo("ScrollbarComponent.numberOfSteps")]
        public _int NumberOfSteps;

        /// <summary>
        /// Handle size.
        /// </summary>
        /// <d> The size of the scrollbar handle where 1 means it fills the entire scrollbar.</d>
        [MapTo("ScrollbarComponent.size")]
        public _float HandleSize;

        /// <summary>
        /// Scrollbar value.
        /// </summary>
        /// <d>The current value of the scrollbar, between 0 and 1.</d>
        [MapTo("ScrollbarComponent.value")]
        public _float Value;

        /// <summary>
        /// Scrollbar component.
        /// </summary>
        /// <d>Manages a scrollbar and handle.</d>
        public UnityEngine.UI.Scrollbar ScrollbarComponent;

        #endregion

        #region Handle

        /// <summary>
        /// Handle image.
        /// </summary>
        /// <d>Handle image sprite.</d>
        [MapTo("Handle.Sprite")]
        public _SpriteAsset HandleImage;

        /// <summary>
        /// Handle image type.
        /// </summary>
        /// <d>Handle image sprite type.</d>
        [MapTo("Handle.Type")]
        public _ImageType HandleImageType;

        /// <summary>
        /// Handle image material.
        /// </summary>
        /// <d>Handle image material.</d>
        [MapTo("Handle.Material")]
        public _Material HandleMaterial;

        /// <summary>
        /// Handle image color.
        /// </summary>
        /// <d>Handle image color.</d>
        [MapTo("Handle.Color")]
        public _Color HandleColor;

        /// <summary>
        /// Handle image.
        /// </summary>
        /// <d>Image used to display scrollbar handle.</d>
        public Image Handle;

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Sets default values of the view
        /// </summary>
        public override void SetDefaultValues()
        {
            base.SetDefaultValues();

            Orientation.DirectValue = ElementOrientation.Horizontal;
            Handle.ImageComponent.color = Color.white;
            Breadth.DirectValue = ElementSize.FromPixels(20);

            Handle.UpdateRectTransform.DirectValue = false;

            ScrollbarComponent.targetGraphic = Handle.ImageComponent;
            ScrollbarComponent.handleRect = Handle.RectTransform;
            Handle.RectTransform.Reset();
        }

        public override bool CalculateLayoutChanges(LayoutChangeContext context) {

            // adjust scrollbar to orientation
            if (Orientation == ElementOrientation.Horizontal)
            {
                Layout.TargetWidth = ElementSize.FromPercents(1f);
                Layout.TargetHeight = ElementSize.FromPixels(Breadth.Value.Pixels);
                Layout.Alignment = ElementAlignment.Bottom;

                ScrollbarComponent.direction = UnityEngine.UI.Scrollbar.Direction.LeftToRight;
            }
            else
            {
                Layout.TargetWidth = ElementSize.FromPixels(Breadth.Value.Pixels);
                Layout.TargetHeight = ElementSize.FromPercents(1f);
                Layout.Alignment = ElementAlignment.Right;

                ScrollbarComponent.direction = UnityEngine.UI.Scrollbar.Direction.BottomToTop;
            }

            return base.CalculateLayoutChanges(context);
        }

        #endregion
    }
}
