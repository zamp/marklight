
namespace MarkLight.Views.UI
{
    /// <summary>
    /// DataGrid view.
    /// </summary>
    /// <d>The data grid is used to arrange dynamic or static content in a grid.</d>
    [MapViewField("ItemSelected", "DataGridList.ItemSelected")]
    [MapViewField("ItemDeselected", "DataGridList.ItemDeselected")]
    [MapViewField("ListChanged", "DataGridList.ListChanged")]
    [HideInPresenter]
    public class DataGrid : UIView
    {
        #region Fields

        /// <summary>
        /// Row header view.
        /// </summary>
        /// <d>The row view that is displayed as header for all the data grid rows.</d>
        public RowHeader RowHeader;

        #region DataGridList

        /// <summary>
        /// User-defined data grid data.
        /// </summary>
        /// <d>Can be bound to an generic ObservableList to dynamically generate data grid items based on a
        /// template.</d>
        [MapTo("DataGridList.Items")]
        public _IObservableList Items;

        /// <summary>
        /// Selected list item.
        /// </summary>
        /// <d>Set when the selected data grid item changes and points to the user-defined item data.</d>
        [MapTo("DataGridList.SelectedItem")]
        public _object SelectedItem;

        /// <summary>
        /// Horizontal scrollbar visibility of scrollable list content.
        /// </summary>
        /// <d>Horizontal scrollbar visibility of scrollable list content.</d>
        [MapTo("DataGridList.HorizontalScrollbarVisibility")]
        public _PanelScrollbarVisibility HorizontalScrollbarVisibility;

        /// <summary>
        /// Vertical scrollbar visibility of scrollable list content.
        /// </summary>
        /// <d>Vertical scrollbar visibility of scrollable list content.</d>
        [MapTo("DataGridList.VerticalScrollbarVisibility")]
        public _PanelScrollbarVisibility VerticalScrollbarVisibility;

        /// <summary>
        /// Alignment of scrollable list content.
        /// </summary>
        /// <d>Sets the alignment of the scrollable list content.</d>
        [MapTo("DataGridList.ScrollableContentAlignment")]
        public _ElementAlignment ScrollableContentAlignment;

        /// <summary>
        /// Indicates if the list of rows are scrollable.
        /// </summary>
        /// <d>Boolean indicating if the list of rows is to be scrollable.</d>
        [MapTo("DataGridList.IsScrollable")]
        public _bool IsScrollable;

        /// <summary>
        /// Indicates if items can be deselected by clicking.
        /// </summary>
        /// <d>A boolean indicating if items in the data grid can be deselected by clicking. Items can always be
        /// deselected programmatically.</d>
        [MapTo("DataGridList.CanDeselect")]
        public _bool CanDeselect;

        /// <summary>
        /// Indicates if more than one list item can be selected.
        /// </summary>
        /// <d>A boolean indicating if more than one data grid item can be selected by clicking or
        /// programmatically.</d>
        [MapTo("DataGridList.CanMultiSelect")]
        public _bool CanMultiSelect;

        /// <summary>
        /// Indicates if items can be selected by clicking.
        /// </summary>
        /// <d>A boolean indicating if items can be selected by clicking. Items can always be selected
        /// programmatically.</d>
        [MapTo("DataGridList.CanSelect")]
        public _bool CanSelect;

        /// <summary>
        /// Indicates if the rows should alternate in style.
        /// </summary>
        /// <d>Boolean indicating if the Row style should alternate between "Default" and "Alternate".</d>
        [MapTo("DataGridList.AlternateItems")]
        public _bool AlternateRows;

        /// <summary>
        /// The alignment of data grid list items.
        /// </summary>
        /// <d>If the data grid list items varies in size the content alignment specifies how the data grid list
        /// items should be arranged in relation to each other.</d>
        [MapTo("DataGridList.ContentAlignment")]
        public _ElementAlignment ContentAlignment;

        /// <summary>
        /// Data grid list content margin.
        /// </summary>
        /// <d>Sets the margin of the data grid list mask view that contains the contents of the data grid list.</d>
        [MapTo("DataGridList.ContentMargin")]
        public _ElementMargin ContentMargin;

        /// <summary>
        /// Sort direction.
        /// </summary>
        /// <d>If data grid list items has SortIndex set they can be sorted in the direction specified.</d>
        [MapTo("DataGridList.SortDirection")]
        public _ElementSortDirection SortDirection;

        #region ListMask

        /// <summary>
        /// Indicates if a list mask is to be used.
        /// </summary>
        /// <d>Boolean indicating if a list mask is to be used.</d>
        [MapTo("DataGridList.UseListMask")]
        public _bool UseListMask;

        /// <summary>
        /// The width of the list mask image.
        /// </summary>
        /// <d>Specifies the width of the list mask image either in pixels or percents.</d>
        [MapTo("DataGridList.ListMaskWidth")]
        public _ElementSize ListMaskWidth;

        /// <summary>
        /// The height of the list mask image.
        /// </summary>
        /// <d>Specifies the height of the list mask image either in pixels or percents.</d>
        [MapTo("DataGridList.ListMaskHeight")]
        public _ElementSize ListMaskHeight;

        /// <summary>
        /// The offset of the list mask image.
        /// </summary>
        /// <d>Specifies the offset of the list mask image.</d>
        [MapTo("DataGridList.ListMaskOffset")]
        public _ElementMargin ListMaskOffset;

        /// <summary>
        /// List max image sprite.
        /// </summary>
        /// <d>The sprite that will be rendered as the list max.</d>
        [MapTo("DataGridList.ListMaskImage")]
        public _SpriteAsset ListMaskImage;

        /// <summary>
        /// List max image type.
        /// </summary>
        /// <d>The type of the image sprite that is to be rendered as the list max.</d>
        [MapTo("DataGridList.ListMaskImageType")]
        public _ImageType ListMaskImageType;

        /// <summary>
        /// List max image material.
        /// </summary>
        /// <d>The material of the list max image.</d>
        [MapTo("DataGridList.ListMaskMaterial")]
        public _Material ListMaskMaterial;

        /// <summary>
        /// List max image color.
        /// </summary>
        /// <d>The color of the list max image.</d>
        [MapTo("DataGridList.ListMaskColor")]
        public _Color ListMaskColor;

        /// <summary>
        /// List mask alignment.
        /// </summary>
        /// <d>Specifies the alignment of the list mask.</d>
        [MapTo("DataGridList.ListMaskAlignment")]
        public _ElementAlignment ListMaskAlignment;

        /// <summary>
        /// Indicates if list mask should be rendered.
        /// </summary>
        /// <d>Indicates if the list mask, i.e. the list mask background image sprite and color should be rendered.</d>
        [MapTo("DataGridList.ListMaskShowGraphic")]
        public _bool ListMaskShowGraphic;

        #endregion

        #region HorizontalScrollbar

        /// <summary>
        /// Orientation of the horizontal scrollbar.
        /// </summary>
        /// <d>Orientation of the horizontal scrollbar.</d>
        [MapTo("DataGridList.HorizontalScrollbarOrientation")]
        public _ElementOrientation HorizontalScrollbarOrientation;

        /// <summary>
        /// Breadth of the horizontal scrollbar.
        /// </summary>
        /// <d>Breadth of the horizontal scrollbar.</d>
        [MapTo("DataGridList.HorizontalScrollbarBreadth")]
        public _ElementSize HorizontalScrollbarBreadth;

        /// <summary>
        /// Scrollbar scroll direction.
        /// </summary>
        /// <d>Scrollbar scroll direction.</d>
        [MapTo("DataGridList.HorizontalScrollbarScrollDirection")]
        public _ScrollbarDirection HorizontalScrollbarScrollDirection;

        /// <summary>
        /// Scroll steps.
        /// </summary>
        /// <d>The number of steps to use for the value. A value of 0 disables use of steps.</d>
        [MapTo("DataGridList.HorizontalScrollbarNumberOfSteps")]
        public _int HorizontalScrollbarNumberOfSteps;

        /// <summary>
        /// Handle size.
        /// </summary>
        /// <d> The size of the horizontal scrollbar handle where 1 means it fills the entire horizontal scrollbar.</d>
        [MapTo("DataGridList.HorizontalScrollbarHandleSize")]
        public _float HorizontalScrollbarHandleSize;

        /// <summary>
        /// Scrollbar value.
        /// </summary>
        /// <d>The current value of the horizontal scrollbar, between 0 and 1.</d>
        [MapTo("DataGridList.HorizontalScrollbarValue")]
        public _float HorizontalScrollbarValue;

        /// <summary>
        /// Horizontal scrollbar image.
        /// </summary>
        /// <d>Horizontal scrollbar image sprite.</d>
        [MapTo("DataGridList.HorizontalScrollbarImage")]
        public _SpriteAsset HorizontalScrollbarImage;

        /// <summary>
        /// Horizontal scrollbar image type.
        /// </summary>
        /// <d>Horizontal scrollbar image sprite type.</d>
        [MapTo("DataGridList.HorizontalScrollbarImageType")]
        public _ImageType HorizontalScrollbarImageType;

        /// <summary>
        /// Horizontal scrollbar image material.
        /// </summary>
        /// <d>Horizontal scrollbar image material.</d>
        [MapTo("DataGridList.HorizontalScrollbarMaterial")]
        public _Material HorizontalScrollbarMaterial;

        /// <summary>
        /// Horizontal scrollbar image color.
        /// </summary>
        /// <d>Horizontal scrollbar image color.</d>
        [MapTo("DataGridList.HorizontalScrollbarColor")]
        public _Color HorizontalScrollbarColor;

        /// <summary>
        /// Horizontal scrollbar handle image.
        /// </summary>
        /// <d>Horizontal scrollbar handle image sprite.</d>
        [MapTo("DataGridList.HorizontalScrollbarHandleImage")]
        public _SpriteAsset HorizontalScrollbarHandleImage;

        /// <summary>
        /// Horizontal scrollbar handle image type.
        /// </summary>
        /// <d>Horizontal scrollbar handle image sprite type.</d>
        [MapTo("DataGridList.HorizontalScrollbarHandleImageType")]
        public _ImageType HorizontalScrollbarHandleImageType;

        /// <summary>
        /// Horizontal scrollbar handle image material.
        /// </summary>
        /// <d>Horizontal scrollbar handle image material.</d>
        [MapTo("DataGridList.HorizontalScrollbarHandleMaterial")]
        public _Material HorizontalScrollbarHandleMaterial;

        /// <summary>
        /// Horizontal scrollbar handle image color.
        /// </summary>
        /// <d>Horizontal scrollbar handle image color.</d>
        [MapTo("DataGridList.HorizontalScrollbarHandleColor")]
        public _Color HorizontalScrollbarHandleColor;

        #endregion

        #region VerticalScrollbar

        /// <summary>
        /// Orientation of the vertical scrollbar.
        /// </summary>
        /// <d>Orientation of the vertical scrollbar.</d>
        [MapTo("DataGridList.VerticalScrollbarOrientation")]
        public _ElementOrientation VerticalScrollbarOrientation;

        /// <summary>
        /// Breadth of the vertical scrollbar.
        /// </summary>
        /// <d>Breadth of the vertical scrollbar.</d>
        [MapTo("DataGridList.VerticalScrollbarBreadth")]
        public _ElementSize VerticalScrollbarBreadth;

        /// <summary>
        /// Scrollbar scroll direction.
        /// </summary>
        /// <d>Scrollbar scroll direction.</d>
        [MapTo("DataGridList.VerticalScrollbarScrollDirection")]
        public _ScrollbarDirection VerticalScrollbarScrollDirection;

        /// <summary>
        /// Scroll steps.
        /// </summary>
        /// <d>The number of steps to use for the value. A value of 0 disables use of steps.</d>
        [MapTo("DataGridList.VerticalScrollbarNumberOfSteps")]
        public _int VerticalScrollbarNumberOfSteps;

        /// <summary>
        /// Vertical scrollbar handle size.
        /// </summary>
        /// <d> The size of the vertical scrollbar handle where 1 means it fills the entire vertical scrollbar.</d>
        [MapTo("DataGridList.VerticalScrollbarHandleSize")]
        public _float VerticalScrollbarHandleSize;

        /// <summary>
        /// Scrollbar value.
        /// </summary>
        /// <d>The current value of the vertical scrollbar, between 0 and 1.</d>
        [MapTo("DataGridList.VerticalScrollbarValue")]
        public _float VerticalScrollbarValue;

        /// <summary>
        /// Vertical scrollbar image.
        /// </summary>
        /// <d>Vertical scrollbar image sprite.</d>
        [MapTo("DataGridList.VerticalScrollbarImage")]
        public _SpriteAsset VerticalScrollbarImage;

        /// <summary>
        /// Vertical scrollbar image type.
        /// </summary>
        /// <d>Vertical scrollbar image sprite type.</d>
        [MapTo("DataGridList.VerticalScrollbarImageType")]
        public _ImageType VerticalScrollbarImageType;

        /// <summary>
        /// Vertical scrollbar image material.
        /// </summary>
        /// <d>Vertical scrollbar image material.</d>
        [MapTo("DataGridList.VerticalScrollbarMaterial")]
        public _Material VerticalScrollbarMaterial;

        /// <summary>
        /// Vertical scrollbar image color.
        /// </summary>
        /// <d>Vertical scrollbar image color.</d>
        [MapTo("DataGridList.VerticalScrollbarColor")]
        public _Color VerticalScrollbarColor;

        /// <summary>
        /// Vertical scrollbar handle image.
        /// </summary>
        /// <d>Vertical scrollbar handle image sprite.</d>
        [MapTo("DataGridList.VerticalScrollbarHandleImage")]
        public _SpriteAsset VerticalScrollbarHandleImage;

        /// <summary>
        /// Vertical scrollbar handle image type.
        /// </summary>
        /// <d>Vertical scrollbar handle image sprite type.</d>
        [MapTo("DataGridList.VerticalScrollbarHandleImageType")]
        public _ImageType VerticalScrollbarHandleImageType;

        /// <summary>
        /// Vertical scrollbar handle image material.
        /// </summary>
        /// <d>Vertical scrollbar handle image material.</d>
        [MapTo("DataGridList.VerticalScrollbarHandleMaterial")]
        public _Material VerticalScrollbarHandleMaterial;

        /// <summary>
        /// Vertical scrollbar handle image color.
        /// </summary>
        /// <d>Vertical scrollbar handle image color.</d>
        [MapTo("DataGridList.VerticalScrollbarHandleColor")]
        public _Color VerticalScrollbarHandleColor;

        #endregion

        #region ScrollRect

        /// <summary>
        /// Indicates if the content can scroll horizontally.
        /// </summary>
        /// <d>Boolean indicating if the content can be scrolled horizontally.</d>
        [MapTo("DataGridList.CanScrollHorizontally")]
        public _bool CanScrollHorizontally;

        /// <summary>
        /// Indicates if the content can scroll vertically.
        /// </summary>
        /// <d>Boolean indicating if the content can be scrolled vertically.</d>
        [MapTo("DataGridList.CanScrollVertically")]
        public _bool CanScrollVertically;

        /// <summary>
        /// Scroll deceleration rate.
        /// </summary>
        /// <d>Value indicating the rate of which the scroll stops moving.</d>
        [MapTo("DataGridList.DecelerationRate")]
        public _float DecelerationRate;

        /// <summary>
        /// Scroll elasticity.
        /// </summary>
        /// <d>Value indicating how elastic the scrolling is when moved beyond the bounds of the scrollable content.</d>
        [MapTo("DataGridList.Elasticity")]
        public _float Elasticity;

        /// <summary>
        /// Suppress scroll elasticity when rendering view.
        /// </summary>
        /// <d>True prevents scrolling movement due to elasticity when the layout is rendered.</d>
        [MapTo("DataGridList.DisableRenderElasticity")]
        public _bool DisableRenderElasticity;

        /// <summary>
        /// Horizontal normalized position.
        /// </summary>
        /// <d>Value between 0-1 indicating the position of the scrollable content.</d>
        [MapTo("DataGridList.HorizontalNormalizedPosition")]
        public _float HorizontalNormalizedPosition;

        /// <summary>
        /// Space between scrollbar and scrollable content.
        /// </summary>
        /// <d>Space between scrollbar and scrollable content.</d>
        [MapTo("DataGridList.HorizontalScrollbarSpacing")]
        public _float HorizontalScrollbarSpacing;

        /// <summary>
        /// Indicates if scroll has intertia.
        /// </summary>
        /// <d>Boolean indicating if the scroll has inertia.</d>
        [MapTo("DataGridList.HasInertia")]
        public _bool HasInertia;

        /// <summary>
        /// Behavior when scrolled beyond bounds.
        /// </summary>
        /// <d>Enum specifying the behavior to use when the content moves beyond the scroll rect.</d>
        [MapTo("DataGridList.MovementType")]
        public _ScrollRectMovementType MovementType;

        /// <summary>
        /// Normalized position of the scroll.
        /// </summary>
        /// <d>The scroll position as a Vector2 between (0,0) and (1,1) with (0,0) being the lower left corner.</d>
        [MapTo("DataGridList.NormalizedPosition")]
        public _Vector2 NormalizedPosition;

        /// <summary>
        /// Scroll sensitivity.
        /// </summary>
        /// <d>Value indicating how sensitive the scrolling is to scroll wheel and track pad movement.</d>
        [MapTo("DataGridList.ScrollSensitivity")]
        public _float ScrollSensitivity;

        /// <summary>
        /// Current velocity of the content.
        /// </summary>
        /// <d>Indicates the current velocity of the scrolled content.</d>
        [MapTo("DataGridList.ScrollVelocity")]
        public _Vector2 ScrollVelocity;

        /// <summary>
        /// Vertical normalized position.
        /// </summary>
        /// <d>Value between 0-1 indicating the position of the scrollable content.</d>
        [MapTo("DataGridList.VerticalNormalizedPosition")]
        public _float VerticalNormalizedPosition;

        /// <summary>
        /// Space between scrollbar and scrollable content.
        /// </summary>
        /// <d>Space between scrollbar and scrollable content.</d>
        [MapTo("DataGridList.VerticalScrollbarSpacing")]
        public _float VerticalScrollbarSpacing;

        /// <summary>
        /// Scroll delta distance for disabling interaction.
        /// </summary>
        /// <d>If set any interaction with child views (clicks, etc) is disabled when the specified distance has been
        /// scrolled. This is used e.g. to disable clicks while scrolling a selectable list of items.</d>
        [MapTo("DataGridList.DisableInteractionScrollDelta")]
        public _float DisableInteractionScrollDelta;

        #endregion

        /// <summary>
        /// Data grid list.
        /// </summary>
        /// <d>The data grid list renders all the selectable data grid rows.</d>
        public List DataGridList;

        #endregion

        #endregion

        #region Methods

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();

            DataGridList.Fields.SetIsSet("Width");
            DataGridList.Fields.SetIsSet("Height");
            DataGridList.DisableItemArrangement.Value = true;
            DataGridList.UseListMask.Value = false;
        }

        public override bool CalculateLayoutChanges(LayoutChangeContext context)
        {
            var list = DataGridList;

            if (list.ListPanel != null)
            {
                list.AdjustScrollableLayout();
            }

            var rowHeaderHeight = RowHeader != null
                ? RowHeader.Layout.PixelHeight
                : 0f;

            var listMargin = new ElementMargin(0f, rowHeaderHeight, 0f, 0f);
            list.Margin.DirectValue = listMargin;
            list.Layout.Margin = listMargin;

            var layoutCalc = list.LayoutCalculator as GroupLayoutCalculator;
            if (layoutCalc != null)
            {
                layoutCalc.AdjustToWidth = !IsScrollable || !CanScrollHorizontally;
                layoutCalc.AdjustToHeight = !IsScrollable || !CanScrollVertically;

                layoutCalc.Alignment = ContentAlignment.IsSet
                    ? ContentAlignment.Value
                    : ElementAlignment.TopLeft;

                layoutCalc.Orientation = ElementOrientation.Vertical;
                layoutCalc.Overflow = OverflowMode.Overflow;
                layoutCalc.ScrollContent = list.ScrollContent;
            }

            list.RefreshLayoutData();
            context.NotifyLayoutUpdated(list);

            GetContentChildren(list.ScrollContent ?? list.Content, _childBuffer);
            var result = list.LayoutCalculator.CalculateLayoutChanges(list, _childBuffer, context);
            _childBuffer.Clear();

            if (result)
            {
                list.Layout.Width = ElementSize.FromPercents(1f, true);
                list.Layout.Height = ElementSize.FromPercents(1f, true);
                return true;
            }
            return false;
        }

        public override void Initialize()
        {
            base.Initialize();

            // parse header columns
            var header = DataGridList.Content.Find<RowHeader>(new ViewSearchArgs(true)
            {
                IsRecursive = false
            });
            if (header != null)
            {
                RowHeader = header;
                header.MoveTo(this, 0);
                header.Activate();
            }
        }

        #endregion
    }
}
