﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace MarkLight.Views.UI
{
    /// <summary>
    /// Slider view.
    /// </summary>
    /// <d>Slider with a handle that can be moved with the mouse. Can be oriented horizontally or vertically.</d>
    [HideInPresenter]
    public class Slider : UIView
    {
        #region Fields

        #region SliderBackgroundImage

        /// <summary>
        /// Slider image sprite.
        /// </summary>
        /// <d>The sprite that will be rendered as the slider.</d>
        [MapTo("SliderBackgroundImageView.Sprite")]
        public _SpriteAsset SliderBackgroundImage;

        /// <summary>
        /// Slider image type.
        /// </summary>
        /// <d>The type of the image sprite that is to be rendered as the slider.</d>
        [MapTo("SliderBackgroundImageView.Type")]
        public _ImageType SliderBackgroundImageType;

        /// <summary>
        /// Slider image material.
        /// </summary>
        /// <d>The material of the slider image.</d>
        [MapTo("SliderBackgroundImageView.Material")]
        public _Material SliderBackgroundMaterial;

        /// <summary>
        /// Slider image color.
        /// </summary>
        /// <d>The color of the slider image.</d>
        [MapTo("SliderBackgroundImageView.Color")]
        public _Color SliderBackgroundColor;

        /// <summary>
        /// Slider background image.
        /// </summary>
        /// <d>Presents the slider background image.</d>
        public Image SliderBackgroundImageView;

        #endregion

        #region SliderFillImage

        /// <summary>
        /// Slider fill image sprite.
        /// </summary>
        /// <d>The sprite that will be rendered as the slider fill.</d>
        [MapTo("SliderFillImageView.Sprite")]
        public _SpriteAsset SliderFillImage;

        /// <summary>
        /// Slider fill image type.
        /// </summary>
        /// <d>The type of the image sprite that is to be rendered as the slider fill.</d>
        [MapTo("SliderFillImageView.Type")]
        public _ImageType SliderFillImageType;

        /// <summary>
        /// Slider fill image material.
        /// </summary>
        /// <d>The material of the slider fill image.</d>
        [MapTo("SliderFillImageView.Material")]
        public _Material SliderFillMaterial;

        /// <summary>
        /// Slider fill image color.
        /// </summary>
        /// <d>The color of the slider fill image.</d>
        [MapTo("SliderFillImageView.Color")]
        public _Color SliderFillColor;

        /// <summary>
        /// Slider fill image.
        /// </summary>
        /// <d>Presents the slider fill image.</d>
        public Image SliderFillImageView;

        #endregion

        #region SliderHandleImage

        /// <summary>
        /// Slider handle image sprite.
        /// </summary>
        /// <d>The sprite that will be rendered as the slider handle.</d>
        [MapTo("SliderHandleImageView.Sprite")]
        public _SpriteAsset SliderHandleImage;

        /// <summary>
        /// Slider handle image type.
        /// </summary>
        /// <d>The type of the image sprite that is to be rendered as the slider handle.</d>
        [MapTo("SliderHandleImageView.Type")]
        public _ImageType SliderHandleImageType;

        /// <summary>
        /// Slider handle image material.
        /// </summary>
        /// <d>The material of the slider handle image.</d>
        [MapTo("SliderHandleImageView.Material")]
        public _Material SliderHandleMaterial;

        /// <summary>
        /// Slider handle image color.
        /// </summary>
        /// <d>The color of the slider handle image.</d>
        [MapTo("SliderHandleImageView.Color")]
        public _Color SliderHandleColor;

        /// <summary>
        /// Slider handle length.
        /// </summary>
        /// <d>Length of the slider handle.</d>
        [MapTo("SliderHandleImageView.Width")]
        public _ElementSize SliderHandleLength;

        /// <summary>
        /// Slider handle breadth.
        /// </summary>
        /// <d>Breadth of the slider handle.</d>
        [MapTo("SliderHandleImageView.Height")]
        public _ElementSize SliderHandleBreadth;

        /// <summary>
        /// Slider handle image.
        /// </summary>
        /// <d>Presents the slider handle image.</d>
        public Image SliderHandleImageView;

        #endregion

        #region SliderFillRegion

        /// <summary>
        /// Slider fill margin.
        /// </summary>
        /// <d>Margin of the slider fill region.</d>
        [MapTo("SliderFillRegion.Margin")]
        public _ElementMargin SliderFillMargin;

        /// <summary>
        /// Region that contains the fill image.
        /// </summary>
        /// <d>Region that contains the fill image.</d>
        public Region SliderFillRegion;

        #endregion

        /// <summary>
        /// Length of the slider.
        /// </summary>
        /// <d>Specifies the length of the slider. Corresponds to the width or height depending on the orientation
        /// of the slider.</d>
        public _ElementSize Length;

        /// <summary>
        /// Breadth of the slider.
        /// </summary>
        /// <d>Specifies the breadth of the slider. Corresponds to the height or width depending on the orientation
        /// of the slider.</d>
        public _ElementSize Breadth;

        /// <summary>
        /// Orientation of the slider.
        /// </summary>
        /// <d>Enum specifying the orientation of the slider.</d>
        public _ElementOrientation Orientation;

        /// <summary>
        /// Minimum value.
        /// </summary>
        /// <d>Value of the slider when the handle is at the beginning of the slide area.</d>
        [ChangeHandler("SliderValueChanged")]
        public _float Min;

        /// <summary>
        /// Maximum value.
        /// </summary>
        /// <d>Value of the slider when the handle is at the end of the slide area.</d>
        [ChangeHandler("SliderValueChanged")]
        public _float Max;

        /// <summary>
        /// Current value.
        /// </summary>
        /// <d>Current value of the slider. Calculated from the current handle position and the Min/Max value of the
        /// slider.</d>
        [ChangeHandler("SliderValueChanged")]
        public _float Value;

        /// <summary>
        /// Indicates if user can drag the slider handle.
        /// </summary>
        /// <d>Boolean indicating if the user can interact with the slider and drag the handle.</d>
        public _bool CanSlide;

        /// <summary>
        /// Indicates if value set when handle is released.
        /// </summary>
        /// <d>Boolean indicating if the slider value should be set when the user releases the handle instead of
        /// continously while dragging.</d>
        public _bool SetValueOnDragEnded;

        /// <summary>
        /// Indicates if slider should go right to left.
        /// </summary>
        /// <d>Boolean indicating if the slider should go from right to left instead of left to right (default).</d>
        public _bool IsRightToLeft;

        /// <summary>
        /// Specifies how many steps the slider should have.
        /// </summary>
        /// <d>Specifies how many steps the slider should have. If zero or less, infinite steps are used.</d>
        public _float Steps;

        /// <summary>
        /// Region containing slider content.
        /// </summary>
        /// <d>Region containing slider background, fill and handle image.</d>
        public Region SliderRegion;

        /// <summary>
        /// Slider value changed.
        /// </summary>
        /// <d>Triggered when the slider value changes. Triggered once when handle is released if SetValueOnDragEnded
        /// is set.</d>
        public ViewAction ValueChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Sets default values of the view.
        /// </summary>
        public override void SetDefaultValues()
        {
            base.SetDefaultValues();

            Length.DirectValue = ElementSize.FromPixels(160);
            Breadth.DirectValue = ElementSize.FromPixels(40);
            Orientation.DirectValue = ElementOrientation.Horizontal;
            Min.DirectValue = 0;
            Max.DirectValue = 100;
            CanSlide.DirectValue = true;
            SetValueOnDragEnded.DirectValue = false;
            SliderFillImageView.Alignment.DirectValue = ElementAlignment.Left;
            SliderHandleImageView.Alignment.DirectValue = ElementAlignment.Left;
            SliderHandleImageView.Width.DirectValue = ElementSize.FromPixels(20);
            SliderHandleImageView.Height.DirectValue = ElementSize.FromPercents(1f);
        }

        public override bool CalculateLayoutChanges(LayoutChangeContext context) {

            if (!Width.IsSet)
                Layout.TargetWidth = Orientation == ElementOrientation.Horizontal
                    ? Length.Value
                    : Breadth.Value;

            if (!Height.IsSet)
                Layout.TargetHeight = Orientation == ElementOrientation.Horizontal
                    ? Breadth.Value
                    : Length.Value;

            SliderFillImageView.Alignment.Value = IsRightToLeft
                ? ElementAlignment.Right
                : ElementAlignment.Left;

            SliderHandleImageView.Alignment.Value = IsRightToLeft
                ? ElementAlignment.Right
                : ElementAlignment.Left;

            //base.LayoutChanged();

            // if vertical slider rotate slide region 90 degrees
            if (Orientation == ElementOrientation.Vertical)
            {
                SliderRegion.Layout.TargetWidth = ElementSize.FromPixels(RectTransform.rect.height);
                SliderRegion.Layout.TargetHeight = ElementSize.FromPixels(RectTransform.rect.width);
                SliderRegion.Rotation.Value = Quaternion.Euler(new Vector3(0, 0, 90));
                context.NotifyLayoutUpdated(SliderRegion);
            }

            // update slider position
            UpdateSliderPosition(Value);

            return base.CalculateLayoutChanges(context);
        }

        /// <summary>
        /// Called when the value of the slider changes (or any fields affecting the value).
        /// </summary>
        public virtual void SliderValueChanged()
        {
            // clamp value to min/max
            var clampedValue = Value.Value.Clamp(Min, Max);
            Value.DirectValue = Steps > 0 ? Nearest(clampedValue, Min, Max, Steps) : clampedValue;
            UpdateSliderPosition(Value);
        }

        /// <summary>
        /// Called on slider drag begin.
        /// </summary>
        public void SliderBeginDrag(PointerEventData eventData)
        {
            if (!CanSlide)
            {
                return;
            }

            SetSlideTo(eventData.position);
        }

        /// <summary>
        /// Called on slider drag end.
        /// </summary>
        public void SliderEndDrag(PointerEventData eventData)
        {
            if (!CanSlide)
            {
                return;
            }

            SetSlideTo(eventData.position, true);
        }

        /// <summary>
        /// Called on slider drag.
        /// </summary>
        public void SliderDrag(PointerEventData eventData)
        {
            if (!CanSlide)
            {
                return;
            }

            SetSlideTo(eventData.position);
        }

        /// <summary>
        /// Called on potential drag begin (click).
        /// </summary>
        public void SliderInitializePotentialDrag(PointerEventData eventData)
        {
            if (!CanSlide)
            {
                return;
            }

            SetSlideTo(eventData.position, true);
        }

        /// <summary>
        /// Sets slider value.
        /// </summary>
        public void SlideTo(float value)
        {
            float clampedValue = value.Clamp(Min, Max);
            Value.Value = clampedValue;
        }

        /// <summary>
        /// Slides the slider to the given position.
        /// </summary>
        private void SetSlideTo(Vector2 mouseScreenPositionIn, bool isEndDrag = false)
        {
            var fillTransform = SliderFillRegion.RectTransform;

            var pos = GetLocalPoint(mouseScreenPositionIn);

            // calculate slide percentage (transform.position.x/y is center of fill area)
            float p;
            var slideAreaLength = fillTransform.rect.width - SliderHandleImageView.Layout.Width.Pixels;
            p = Orientation == ElementOrientation.Horizontal
                ? ((pos.x - fillTransform.localPosition.x + slideAreaLength / 2f) / slideAreaLength).Clamp(0, 1)
                : ((pos.y - fillTransform.localPosition.y + slideAreaLength / 2f) / slideAreaLength).Clamp(0, 1);

            if (IsRightToLeft)
            {
                // if we slide from left to right then the slide percentage is inverted
                p = 1 - p;
            }

            // set value
            var newValue = (Max - Min) * p + Min;
            newValue = Steps > 0 ? Nearest(newValue, Min, Max, Steps) : newValue;

            if (!SetValueOnDragEnded || (SetValueOnDragEnded && isEndDrag))
            {
                Value.Value = newValue;
                ValueChanged.Trigger();
            }
            else
            {
                UpdateSliderPosition(newValue);
            }
        }

        /// <summary>
        /// Snaps to nearest value based on number of steps.
        /// </summary>
        public static float Nearest(float value, float min, float max, float steps)
        {
            var zerone = Mathf.Round((value - min) * steps / (max - min)) / steps;
            return zerone * (max - min) + min;
        }

        /// <summary>
        /// Sets slider position based on value.
        /// </summary>
        private void UpdateSliderPosition(float value)
        {
            var p = (value - Min) / (Max - Min);
            var fillTransform = SliderFillRegion.RectTransform;

            // set handle offset
            var fillWidth = fillTransform.rect.width;
            var slideAreaWidth = fillWidth - SliderHandleImageView.Layout.Width.Pixels;
            var sliderFillMargin = IsRightToLeft
                ? SliderFillRegion.Layout.Margin.Right.Pixels
                : SliderFillRegion.Layout.Margin.Left.Pixels;
            var handleOffset = p * slideAreaWidth + sliderFillMargin;

            SliderHandleImageView.Layout.OffsetFromParent = IsRightToLeft
                ? ElementMargin.FromRight(ElementSize.FromPixels(handleOffset))
                : ElementMargin.FromLeft(ElementSize.FromPixels(handleOffset));
            SliderHandleImageView.RenderLayout();

            // set fill percentage as to match the offset of the handle
            var fillP = (handleOffset + SliderHandleImageView.Layout.Width.Pixels / 2f) / fillWidth;
            SliderFillImageView.Layout.TargetWidth = ElementSize.FromPercents(fillP);
            SliderFillImageView.RenderLayout();
        }

        #endregion
    }
}
