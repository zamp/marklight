using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MarkLight.Views.UI
{
    /// <summary>
    /// List view.
    /// </summary>
    /// <d>The list view presents a selectable list of items. It can either contain a static list of ListItem views or
    /// one ListItem with IsTemplate="True". If bound to list data through the Items field the list uses the template
    /// to generate a dynamic list of ListItems.</d>
    [HideInPresenter]
    public class List : UIView
    {
        #region Fields

        #region ListMask

        /// <summary>
        /// Indicates if a list mask is to be used.
        /// </summary>
        /// <d>Boolean indicating if a list mask is to be used.</d>
        public _bool UseListMask;

        /// <summary>
        /// The width of the list mask image.
        /// </summary>
        /// <d>Specifies the width of the list mask image either in pixels or percents.</d>
        [MapTo("ListMask.Width")]
        public _ElementSize ListMaskWidth;

        /// <summary>
        /// The height of the list mask image.
        /// </summary>
        /// <d>Specifies the height of the list mask image either in pixels or percents.</d>
        [MapTo("ListMask.Height")]
        public _ElementSize ListMaskHeight;

        /// <summary>
        /// The offset of the list mask image.
        /// </summary>
        /// <d>Specifies the offset of the list mask image.</d>
        [MapTo("ListMask.Offset")]
        public _ElementMargin ListMaskOffset;

        /// <summary>
        /// List max image sprite.
        /// </summary>
        /// <d>The sprite that will be rendered as the list max.</d>
        [MapTo("ListMask.BackgroundImage")]
        public _SpriteAsset ListMaskImage;

        /// <summary>
        /// List max image type.
        /// </summary>
        /// <d>The type of the image sprite that is to be rendered as the list max.</d>
        [MapTo("ListMask.BackgroundImageType")]
        public _ImageType ListMaskImageType;

        /// <summary>
        /// List max image material.
        /// </summary>
        /// <d>The material of the list max image.</d>
        [MapTo("ListMask.BackgroundMaterial")]
        public _Material ListMaskMaterial;

        /// <summary>
        /// List max image color.
        /// </summary>
        /// <d>The color of the list max image.</d>
        [MapTo("ListMask.BackgroundColor")]
        public _Color ListMaskColor;

        /// <summary>
        /// List mask alignment.
        /// </summary>
        /// <d>Specifies the alignment of the list mask.</d>
        [MapTo("ListMask.Alignment")]
        public _ElementAlignment ListMaskAlignment;

        /// <summary>
        /// Indicates if list mask should be rendered.
        /// </summary>
        /// <d>Indicates if the list mask, i.e. the list mask background image sprite and color should be rendered.</d>
        [MapTo("ListMask.ShowMaskGraphic")]
        public _bool ListMaskShowGraphic;

        /// <summary>
        /// Content margin of the list.
        /// </summary>
        /// <d>Sets the margin of the list mask view that contains the contents of the list.</d>
        [MapTo("ListMask.Margin")]
        public _ElementMargin ContentMargin;

        /// <summary>
        /// Content padding.
        /// </summary>
        /// <d>The amount of margin around all list items within the list.</d>
        [ChangeHandler("LayoutChanged")]
        public _ElementMargin Padding;

        /// <summary>
        /// List mask.
        /// </summary>
        /// <d>The list mask can be used to mask the list and its items using a mask graphic.</d>
        public Mask ListMask;

        #endregion

        #region ListPanel

        #region HorizontalScrollbar

        /// <summary>
        /// Orientation of the horizontal scrollbar.
        /// </summary>
        /// <d>Orientation of the horizontal scrollbar.</d>
        [MapTo("ListPanel.HorizontalScrollbarOrientation")]
        public _ElementOrientation HorizontalScrollbarOrientation;

        /// <summary>
        /// Breadth of the horizontal scrollbar.
        /// </summary>
        /// <d>Breadth of the horizontal scrollbar.</d>
        [MapTo("ListPanel.HorizontalScrollbarBreadth")]
        public _ElementSize HorizontalScrollbarBreadth;

        /// <summary>
        /// Scrollbar scroll direction.
        /// </summary>
        /// <d>Scrollbar scroll direction.</d>
        [MapTo("ListPanel.HorizontalScrollbarScrollDirection")]
        public _ScrollbarDirection HorizontalScrollbarScrollDirection;

        /// <summary>
        /// Scroll steps.
        /// </summary>
        /// <d>The number of steps to use for the value. A value of 0 disables use of steps.</d>
        [MapTo("ListPanel.HorizontalScrollbarNumberOfSteps")]
        public _int HorizontalScrollbarNumberOfSteps;

        /// <summary>
        /// Handle size.
        /// </summary>
        /// <d> The size of the horizontal scrollbar handle where 1 means it fills the entire horizontal scrollbar.</d>
        [MapTo("ListPanel.HorizontalScrollbarHandleSize")]
        public _float HorizontalScrollbarHandleSize;

        /// <summary>
        /// Scrollbar value.
        /// </summary>
        /// <d>The current value of the horizontal scrollbar, between 0 and 1.</d>
        [MapTo("ListPanel.HorizontalScrollbarValue")]
        public _float HorizontalScrollbarValue;

        /// <summary>
        /// Horizontal scrollbar image.
        /// </summary>
        /// <d>Horizontal scrollbar image sprite.</d>
        [MapTo("ListPanel.HorizontalScrollbarImage")]
        public _SpriteAsset HorizontalScrollbarImage;

        /// <summary>
        /// Horizontal scrollbar image type.
        /// </summary>
        /// <d>Horizontal scrollbar image sprite type.</d>
        [MapTo("ListPanel.HorizontalScrollbarImageType")]
        public _ImageType HorizontalScrollbarImageType;

        /// <summary>
        /// Horizontal scrollbar image material.
        /// </summary>
        /// <d>Horizontal scrollbar image material.</d>
        [MapTo("ListPanel.HorizontalScrollbarMaterial")]
        public _Material HorizontalScrollbarMaterial;

        /// <summary>
        /// Horizontal scrollbar image color.
        /// </summary>
        /// <d>Horizontal scrollbar image color.</d>
        [MapTo("ListPanel.HorizontalScrollbarColor")]
        public _Color HorizontalScrollbarColor;

        /// <summary>
        /// Horizontal scrollbar handle image.
        /// </summary>
        /// <d>Horizontal scrollbar handle image sprite.</d>
        [MapTo("ListPanel.HorizontalScrollbarHandleImage")]
        public _SpriteAsset HorizontalScrollbarHandleImage;

        /// <summary>
        /// Horizontal scrollbar handle image type.
        /// </summary>
        /// <d>Horizontal scrollbar handle image sprite type.</d>
        [MapTo("ListPanel.HorizontalScrollbarHandleImageType")]
        public _ImageType HorizontalScrollbarHandleImageType;

        /// <summary>
        /// Horizontal scrollbar handle image material.
        /// </summary>
        /// <d>Horizontal scrollbar handle image material.</d>
        [MapTo("ListPanel.HorizontalScrollbarHandleMaterial")]
        public _Material HorizontalScrollbarHandleMaterial;

        /// <summary>
        /// Horizontal scrollbar handle image color.
        /// </summary>
        /// <d>Horizontal scrollbar handle image color.</d>
        [MapTo("ListPanel.HorizontalScrollbarHandleColor")]
        public _Color HorizontalScrollbarHandleColor;

        #endregion

        #region VerticalScrollbar

        /// <summary>
        /// Orientation of the vertical scrollbar.
        /// </summary>
        /// <d>Orientation of the vertical scrollbar.</d>
        [MapTo("ListPanel.VerticalScrollbarOrientation")]
        public _ElementOrientation VerticalScrollbarOrientation;

        /// <summary>
        /// Breadth of the vertical scrollbar.
        /// </summary>
        /// <d>Breadth of the vertical scrollbar.</d>
        [MapTo("ListPanel.VerticalScrollbarBreadth")]
        public _ElementSize VerticalScrollbarBreadth;

        /// <summary>
        /// Scrollbar scroll direction.
        /// </summary>
        /// <d>Scrollbar scroll direction.</d>
        [MapTo("ListPanel.VerticalScrollbarScrollDirection")]
        public _ScrollbarDirection VerticalScrollbarScrollDirection;

        /// <summary>
        /// Scroll steps.
        /// </summary>
        /// <d>The number of steps to use for the value. A value of 0 disables use of steps.</d>
        [MapTo("ListPanel.VerticalScrollbarNumberOfSteps")]
        public _int VerticalScrollbarNumberOfSteps;

        /// <summary>
        /// Vertical scrollbar handle size.
        /// </summary>
        /// <d> The size of the vertical scrollbar handle where 1 means it fills the entire vertical scrollbar.</d>
        [MapTo("ListPanel.VerticalScrollbarHandleSize")]
        public _float VerticalScrollbarHandleSize;

        /// <summary>
        /// Scrollbar value.
        /// </summary>
        /// <d>The current value of the vertical scrollbar, between 0 and 1.</d>
        [MapTo("ListPanel.VerticalScrollbarValue")]
        public _float VerticalScrollbarValue;

        /// <summary>
        /// Vertical scrollbar image.
        /// </summary>
        /// <d>Vertical scrollbar image sprite.</d>
        [MapTo("ListPanel.VerticalScrollbarImage")]
        public _SpriteAsset VerticalScrollbarImage;

        /// <summary>
        /// Vertical scrollbar image type.
        /// </summary>
        /// <d>Vertical scrollbar image sprite type.</d>
        [MapTo("ListPanel.VerticalScrollbarImageType")]
        public _ImageType VerticalScrollbarImageType;

        /// <summary>
        /// Vertical scrollbar image material.
        /// </summary>
        /// <d>Vertical scrollbar image material.</d>
        [MapTo("ListPanel.VerticalScrollbarMaterial")]
        public _Material VerticalScrollbarMaterial;

        /// <summary>
        /// Vertical scrollbar image color.
        /// </summary>
        /// <d>Vertical scrollbar image color.</d>
        [MapTo("ListPanel.VerticalScrollbarColor")]
        public _Color VerticalScrollbarColor;

        /// <summary>
        /// Vertical scrollbar handle image.
        /// </summary>
        /// <d>Vertical scrollbar handle image sprite.</d>
        [MapTo("ListPanel.VerticalScrollbarHandleImage")]
        public _SpriteAsset VerticalScrollbarHandleImage;

        /// <summary>
        /// Vertical scrollbar handle image type.
        /// </summary>
        /// <d>Vertical scrollbar handle image sprite type.</d>
        [MapTo("ListPanel.VerticalScrollbarHandleImageType")]
        public _ImageType VerticalScrollbarHandleImageType;

        /// <summary>
        /// Vertical scrollbar handle image material.
        /// </summary>
        /// <d>Vertical scrollbar handle image material.</d>
        [MapTo("ListPanel.VerticalScrollbarHandleMaterial")]
        public _Material VerticalScrollbarHandleMaterial;

        /// <summary>
        /// Vertical scrollbar handle image color.
        /// </summary>
        /// <d>Vertical scrollbar handle image color.</d>
        [MapTo("ListPanel.VerticalScrollbarHandleColor")]
        public _Color VerticalScrollbarHandleColor;

        #endregion

        #region ScrollRect

        /// <summary>
        /// Indicates if the content can scroll horizontally.
        /// </summary>
        /// <d>Boolean indicating if the content can be scrolled horizontally.</d>
        [MapTo("ListPanel.CanScrollHorizontally")]
        public _bool CanScrollHorizontally;

        /// <summary>
        /// Indicates if the content can scroll vertically.
        /// </summary>
        /// <d>Boolean indicating if the content can be scrolled vertically.</d>
        [MapTo("ListPanel.CanScrollVertically")]
        public _bool CanScrollVertically;

        /// <summary>
        /// Scroll deceleration rate.
        /// </summary>
        /// <d>Value indicating the rate of which the scroll stops moving.</d>
        [MapTo("ListPanel.DecelerationRate")]
        public _float DecelerationRate;

        /// <summary>
        /// Scroll elasticity.
        /// </summary>
        /// <d>Value indicating how elastic the scrolling is when moved beyond the bounds of the scrollable content.</d>
        [MapTo("ListPanel.Elasticity")]
        public _float Elasticity;

        /// <summary>
        /// Suppress scroll elasticity when rendering view.
        /// </summary>
        /// <d>True prevents scrolling movement due to elasticity when the layout is rendered.</d>
        [MapTo("ListPanel.DisableRenderElasticity")]
        public _bool DisableRenderElasticity;

        /// <summary>
        /// Horizontal normalized position.
        /// </summary>
        /// <d>Value between 0-1 indicating the position of the scrollable content.</d>
        [MapTo("ListPanel.HorizontalNormalizedPosition")]
        public _float HorizontalNormalizedPosition;

        /// <summary>
        /// Space between scrollbar and scrollable content.
        /// </summary>
        /// <d>Space between scrollbar and scrollable content.</d>
        [MapTo("ListPanel.HorizontalScrollbarSpacing")]
        public _float HorizontalScrollbarSpacing;

        /// <summary>
        /// Indicates if scroll has intertia.
        /// </summary>
        /// <d>Boolean indicating if the scroll has inertia.</d>
        [MapTo("ListPanel.HasInertia")]
        public _bool HasInertia;

        /// <summary>
        /// Behavior when scrolled beyond bounds.
        /// </summary>
        /// <d>Enum specifying the behavior to use when the content moves beyond the scroll rect.</d>
        [MapTo("ListPanel.MovementType")]
        public _ScrollRectMovementType MovementType;

        /// <summary>
        /// Normalized position of the scroll.
        /// </summary>
        /// <d>The scroll position as a Vector2 between (0,0) and (1,1) with (0,0) being the lower left corner.</d>
        [MapTo("ListPanel.NormalizedPosition")]
        public _Vector2 NormalizedPosition;

        /// <summary>
        /// Scroll sensitivity.
        /// </summary>
        /// <d>Value indicating how sensitive the scrolling is to scroll wheel and track pad movement.</d>
        [MapTo("ListPanel.ScrollSensitivity")]
        public _float ScrollSensitivity;

        /// <summary>
        /// Current velocity of the content.
        /// </summary>
        /// <d>Indicates the current velocity of the scrolled content.</d>
        [MapTo("ListPanel.ScrollVelocity")]
        public _Vector2 ScrollVelocity;

        /// <summary>
        /// Vertical normalized position.
        /// </summary>
        /// <d>Value between 0-1 indicating the position of the scrollable content.</d>
        [MapTo("ListPanel.VerticalNormalizedPosition")]
        public _float VerticalNormalizedPosition;

        /// <summary>
        /// Space between scrollbar and scrollable content.
        /// </summary>
        /// <d>Space between scrollbar and scrollable content.</d>
        [MapTo("ListPanel.VerticalScrollbarSpacing")]
        public _float VerticalScrollbarSpacing;

        /// <summary>
        /// Scroll delta distance for disabling interaction.
        /// </summary>
        /// <d>If set any interaction with child views (clicks, etc) is disabled when the specified distance has been
        /// scrolled. This is used e.g. to disable clicks while scrolling a selectable list of items.</d>
        [MapTo("ListPanel.DisableInteractionScrollDelta")]
        public _float DisableInteractionScrollDelta;

        #endregion

        /// <summary>
        /// Indicates if mask margin should be added.
        /// </summary>
        /// <d>Boolean indicating if margin should be added to the content mask to make room for the scrollbars.</d>
        [MapTo("ListPanel.AddMaskMargin")]
        public _bool AddMaskMargin;

        /// <summary>
        /// Horizontal scrollbar visibility of scrollable list content.
        /// </summary>
        /// <d>Horizontal scrollbar visibility of scrollable list content.</d>
        [MapTo("ListPanel.HorizontalScrollbarVisibility")]
        public _PanelScrollbarVisibility HorizontalScrollbarVisibility;

        /// <summary>
        /// Vertical scrollbar visibility of scrollable list content.
        /// </summary>
        /// <d>Vertical scrollbar visibility of scrollable list content.</d>
        [MapTo("ListPanel.VerticalScrollbarVisibility")]
        public _PanelScrollbarVisibility VerticalScrollbarVisibility;

        /// <summary>
        /// Alignment of scrollable list content.
        /// </summary>
        /// <d>Sets the alignment of the scrollable list content.</d>
        [MapTo("ListPanel.ContentAlignment")]
        public _ElementAlignment ScrollableContentAlignment;

        /// <summary>
        /// Indicates if the items should alternate in style.
        /// </summary>
        /// <d>Boolean indicating if the ListItem style should alternate between "Default" and "Alternate".</d>
        public _bool AlternateItems;

        /// <summary>
        /// Indicates if the list is scrollable.
        /// </summary>
        /// <d>Boolean indicating if the list is to be scrollable.</d>
        public _bool IsScrollable;

        /// <summary>
        /// Scrollable region of the list that contains the list items.
        /// </summary>
        /// <d>Scrollable region of the list that contains the list items. Set to null if the list isn't scrollable.</d>
        public Region ScrollContent;

        /// <summary>
        /// Panel containing scrollable list content.
        /// </summary>
        /// <d>Panel containing scrollable list content. Will be null if IsScrollable is set to False.</d>
        public Panel ListPanel;

        #endregion

        /// <summary>
        /// User-defined data list.
        /// </summary>
        /// <d>Can be bound to an generic ObservableList to dynamically generate ListItems based on a template.</d>
        [ChangeHandler("ItemsChanged")]
        public _IObservableList Items;

        /// <summary>
        /// Orientation of the list.
        /// </summary>
        /// <d>Defines how the list items should be arranged.</d>
        [ChangeHandler("LayoutChanged")]
        public _ElementOrientation Orientation;

        /// <summary>
        /// Boolean indicating if list item arrangement should be disabled.
        /// </summary>
        /// <d>If set to true the list doesn't automatically arrange one item after another. Used when item arrangement
        /// is done elsewhere.</d>
        public _bool DisableItemArrangement;

        /// <summary>
        /// Indicates if an item is selected.
        /// </summary>
        /// <d>Set to true when a list item is selected.</d>
        public _bool IsItemSelected;

        /// <summary>
        /// Indicates if items can be deselected by clicking.
        /// </summary>
        /// <d>A boolean indicating if items in the list can be deselected by clicking. Items can always be deselected
        /// programmatically.</d>
        public _bool CanDeselect;

        /// <summary>
        /// Indicates if more than one list item can be selected.
        /// </summary>
        /// <d>A boolean indicating if more than one list items can be selected by clicking or programmatically.</d>
        public _bool CanMultiSelect;

        /// <summary>
        /// Indicates if items can be selected by clicking.
        /// </summary>
        /// <d>A boolean indicating if items can be selected by clicking. Items can always be selected
        /// programmatically.</d>
        public _bool CanSelect;

        /// <summary>
        /// Indicates if item can be selected again if it's already selected.
        /// </summary>
        /// <d>Boolean indicating if the item can be selected again if it is already selected. This setting is ignored
        /// if CanDeselect is True.</d>
        public _bool CanReselect;

        /// <summary>
        /// Indicates if items are deselected immediately after being selected.
        /// </summary>
        /// <d>A boolean indicating if items are deselected immediately after being selected. Useful if you want to
        /// trigger selection action but don't want the item to remain selected.</d>
        public _bool DeselectAfterSelect;

        /// <summary>
        /// Indicates how items overflow.
        /// </summary>
        /// <d>Enum indicating how items should overflow as they reach the boundaries of the list.</d>
        public _OverflowMode Overflow;

        /// <summary>
        /// The alignment of list items.
        /// </summary>
        /// <d>If the list items varies in size the content alignment specifies how the list items should be arranged
        /// in relation to each other.</d>
        [ChangeHandler("LayoutChanged")]
        public _ElementAlignment ContentAlignment;

        /// <summary>
        /// Sort direction.
        /// </summary>
        /// <d>If list items has SortIndex set they can be sorted in the direction specified.</d>
        [ChangeHandler("LayoutChanged")]
        public _ElementSortDirection SortDirection;

        /// <summary>
        /// Indicates if items are selected on mouse up.
        /// </summary>
        /// <d>Boolean indicating if items are selected on mouse up rather than mouse down (default).</d>
        public _bool SelectOnMouseUp;

        /// <summary>
        /// Indicates if template is to be shown in the editor.
        /// </summary>
        /// <d>Boolean indicating if template should be shown in the editor.</d>
        public _bool ShowTemplateInEditor;

        /// <summary>
        /// List item pool size.
        /// </summary>
        /// <d>Indicates how many list items should be pooled. Pooled items are already created and ready to be used
        /// rather than being created and destroyed on demand. Can be used to increase the performance of dynamic
        /// lists.</d>
        public _int PoolSize;

        /// <summary>
        /// Max list item pool size.
        /// </summary>
        /// <d>Indicates maximum number of list items that should be pooled. If not set it uses initial PoolSize is
        /// used as max. Pooled items are already created and ready to be used rather than being created and
        /// destroyed on demand. Can be used to increase the performance of dynamic lists.</d>
        public _int MaxPoolSize;

        /// <summary>
        /// Indicates if list should use virtualization.
        /// </summary>
        /// <d>Boolean indicating if list should use virtualization where only visible list items are presented in
        /// the visual hierarchy.</d>
        public _bool UseVirtualization;

        /// <summary>
        /// Indicates how much margin should be added to the realization viewport.
        /// </summary>
        /// <d>Boolean indicating how much margin should be added to the realization viewport. If zero the
        /// realization viewport will be the same size as the scrollable viewport. Used when UseVirtualization
        /// is True.</d>
        public _float RealizationMargin;

        /// <summary>
        /// Indicates how many pixels should be scrolled before virtualization updates.
        /// </summary>
        /// <d>Boolean indicating how many pixels should be scrolled before virtualization updates.</d>
        public _float VirtualizationUpdateThreshold;

        /// <summary>
        /// Selected data list item.
        /// </summary>
        /// <d>Set when the selected list item changes and points to the user-defined data item.</d>
        [ChangeHandler("SelectedItemChanged")]
        public _object SelectedItem;

        /// <summary>
        /// Selected items in the data list.
        /// </summary>
        /// <d>Contains selected items in the user-defined list data. Can contain more than one item if IsMultiSelect
        /// is true.</d>
        [ChangeHandler("SelectedItemsChanged")]
        public _IObservableList SelectedItems;

        /// <summary>
        /// Item selected view action.
        /// </summary>
        /// <d>Triggered when a list item is selected either by user interaction or programmatically.</d>
        /// <actionData>ItemSelectionActionData</actionData>
        public ViewAction ItemSelected;

        /// <summary>
        /// Item deselected view action.
        /// </summary>
        /// <d>Triggered when a list item is deselected either by user interaction or programmatically. An item is
        /// deselected if another item is selected and CanMultiSelect is false. If CanMultiSelect is true an item is
        /// deselected when the user clicks on an selected item.</d>
        /// <actionData>ItemSelectionActionData</actionData>
        public ViewAction ItemDeselected;

        /// <summary>
        /// List changed view action.
        /// </summary>
        /// <d>Triggered when the list changes (items added, removed or moved).</d>
        /// <actionData>ListChangedActionData</actionData>
        public ViewAction ListChanged;

        private IObservableList _items;
        private IObservableList _selectedItems;
        private List<ListItem> _presentedListItems;
        private List<ListItem> _listItemTemplates;
        private Dictionary<View, ViewPool> _viewPools;
        private VirtualizedItems _virtualizedItems;
        private float _previousViewportMin;
        private bool _updateVirtualization;

        #endregion

        #region Methods

        /// <summary>
        /// Checks if ActualWidth changes and updates the layout.
        /// </summary>
        public virtual void Update()
        {
            // adjust virtualized/realized items
            if (UseVirtualization)
            {
                UpdateVirtualizedItems();
            }
        }

        public override void InitializeInternalDefaultValues()
        {
            base.InitializeInternalDefaultValues();

            LayoutCalculator = new ListGroupLayout(this);
        }

        public override void SetDefaultValues()
        {
            base.SetDefaultValues();
            CanSelect.DirectValue = true;
            CanDeselect.DirectValue = false;
            CanMultiSelect.DirectValue = false;
            RealizationMargin.DirectValue = 50;
            VirtualizationUpdateThreshold.DirectValue = 25;
        }

        /// <summary>
        /// Called whenever the list is scrolled or items are added, removed or rearranged.
        /// </summary>
        private void UpdateVirtualizedItems()
        {
            float vpMin;
            float vpMax;

            if (Orientation.Value == ElementOrientation.Vertical)
            {
                var viewportHeight = ListPanel.ScrollRect.ActualHeight;
                var scrollHeight = ScrollContent.ActualHeight - viewportHeight;
                vpMin = (1.0f - VerticalNormalizedPosition.Value) * scrollHeight - RealizationMargin.Value;
                vpMax = vpMin + viewportHeight + RealizationMargin.Value;
            }
            else
            {
                var viewportWidth = ListPanel.ScrollRect.ActualWidth;
                var scrollWidth = ScrollContent.ActualWidth - viewportWidth;
                vpMin = HorizontalNormalizedPosition.Value * scrollWidth - RealizationMargin.Value;
                vpMax = vpMin + viewportWidth + RealizationMargin.Value;
            }

            // only update when we have scrolled further than the threshold since last update
            if (Mathf.Abs(_previousViewportMin - vpMin) <= VirtualizationUpdateThreshold.Value && !_updateVirtualization)
                return;

            _updateVirtualization = false;
            _previousViewportMin = vpMin;
            var newItems = _virtualizedItems.GetItemsInRange(vpMin, vpMax);

            // remove any items not in new list from viewport to virtualized list
            var previousItems = Content.GetChildren<ListItem>(
                x => x.IsLive && !x.IsTemplate,
                ViewSearchArgs.NonRecursive);

            foreach (var item in previousItems)
            {
                if (!_virtualizedItems.IsInRange(item, vpMin, vpMax))
                {
                    item.MoveTo(_virtualizedItems.VirtualizedItemsContainer, -1, false);
                }
            }

            // add new items to viewport
            foreach (var item in newItems)
            {
                item.MoveTo(Content, -1, false);
                ListPanel.ScrollRect.UnblockDragEvents(item);
            }
        }

        public override void RefreshLayoutData()
        {
            Layout.Orientation = Orientation.Value;
            base.RefreshLayoutData();
        }

        public override bool CalculateLayoutChanges(LayoutChangeContext context)
        {
            if (DisableItemArrangement)
            {
                return false;
            }

            if (ListPanel != null)
            {
                AdjustScrollableLayout();
            }

            var layoutCalc = LayoutCalculator as ListGroupLayout;
            if (layoutCalc != null)
            {
                layoutCalc.AdjustToWidth = !Width.IsSet;
                layoutCalc.AdjustToHeight = !Height.IsSet;

                layoutCalc.Alignment = ContentAlignment.IsSet ? ContentAlignment.Value : ElementAlignment.TopLeft;
                layoutCalc.Padding = Padding.Value;
                layoutCalc.Orientation = Orientation.Value;
                layoutCalc.Overflow = Overflow.Value;
                layoutCalc.ScrollContent = ScrollContent;
            }

            GetContentChildren(ScrollContent ?? Content, _childBuffer);
            LayoutCalculator.CalculateLayoutChanges(this, _childBuffer, context);
            _childBuffer.Clear();

            if (UseVirtualization)
            {
                _updateVirtualization = true;
            }

            return Layout.IsDirty;
        }

        public override void RenderLayout()
        {
            base.RenderLayout();

            var lastScroll = Items.Value != null ? Items.Value.LastScroll : null;
            if (lastScroll != null)
            {
               ScrollTo(lastScroll.Index, lastScroll.Alignment, lastScroll.Offset);
            }
        }

        /// <summary>
        /// Gets all list items (realized and virtualized) that are active in the list.
        /// </summary>
        public List<ListItem> GetActiveListItems()
        {
            var listItems = new List<ListItem>();
            if (UseVirtualization)
            {
                listItems.AddRange(
                    _virtualizedItems.VirtualizedItemsContainer.GetChildren<ListItem>(
                        x => x.IsLive,
                        ViewSearchArgs.NonRecursive));
            }

            listItems.AddRange(Content.GetChildren<ListItem>(
                x => x.IsLive && !x.IsTemplate,
                ViewSearchArgs.NonRecursive));

            return listItems;
        }

        /// <summary>
        /// Adjusts scrollable layout.
        /// </summary>
        public void AdjustScrollableLayout()
        {
            // set default scrollable content alignment based on orientation
            if (!ScrollableContentAlignment.IsSet)
            {
                if (Overflow.Value == OverflowMode.Overflow)
                {
                    ScrollableContentAlignment.Value = Orientation.Value == ElementOrientation.Vertical
                        ? ElementAlignment.Top
                        : ElementAlignment.Left;
                }
                else
                {
                    ScrollableContentAlignment.Value = Orientation.Value == ElementOrientation.Vertical
                        ? ElementAlignment.Left
                        : ElementAlignment.Top;
                }
            }

            // set default scrollbar visibility based on orientation
            if (!HorizontalScrollbarVisibility.IsSet)
            {
                if (Overflow.Value == OverflowMode.Overflow)
                {
                    HorizontalScrollbarVisibility.Value = Orientation.Value == ElementOrientation.Horizontal
                        ? PanelScrollbarVisibility.AutoHideAndExpandViewport
                        : PanelScrollbarVisibility.Hidden;
                }
                else
                {
                    HorizontalScrollbarVisibility.Value = Orientation.Value == ElementOrientation.Vertical
                        ? PanelScrollbarVisibility.AutoHideAndExpandViewport
                        : PanelScrollbarVisibility.Hidden;
                }

            }

            if (!VerticalScrollbarVisibility.IsSet)
            {
                if (Overflow.Value == OverflowMode.Overflow)
                {
                    VerticalScrollbarVisibility.Value = Orientation.Value == ElementOrientation.Vertical
                        ? PanelScrollbarVisibility.AutoHideAndExpandViewport
                        : PanelScrollbarVisibility.Hidden;
                }
                else
                {
                    VerticalScrollbarVisibility.Value = Orientation.Value == ElementOrientation.Horizontal
                        ? PanelScrollbarVisibility.AutoHideAndExpandViewport
                        : PanelScrollbarVisibility.Hidden;
                }
            }

            // set default allowed scroll direction
            if (!CanScrollHorizontally.IsSet)
            {
                CanScrollHorizontally.Value = Overflow.Value == OverflowMode.Overflow
                    ? Orientation.Value == ElementOrientation.Horizontal
                    : Orientation.Value == ElementOrientation.Vertical;
            }

            if (!CanScrollVertically.IsSet)
            {
                CanScrollVertically.Value = Overflow.Value == OverflowMode.Overflow
                    ? Orientation.Value == ElementOrientation.Vertical
                    : Orientation.Value == ElementOrientation.Horizontal;
            }
        }

        protected override List<UIView> GetContentChildren(View content, List<UIView> output)
        {
            return GetContentChildren(content, SortDirection, output, _sortBuffer);
        }

        /// <summary>
        /// Called when the selected item of the list has been changed.
        /// </summary>
        protected virtual void SelectedItemChanged()
        {
            if (_items == null)
                return;

            if (SelectedItem.IsSet)
                _items.SelectedItem = SelectedItem.Value;
        }

        /// <summary>
        /// Called when the SselectedItems field value is changed.
        /// </summary>
        protected virtual void SelectedItemsChanged() {
            if (SelectedItems.IsSet)
                SetSelectedItems(SelectedItems.Value);
        }

        /// <summary>
        /// Set the internal selected items observable list.
        /// </summary>
        private void SetSelectedItems(IObservableList selectedItems) {

            if (_selectedItems != null)
            {
                if (_items != null)
                {
                    var selected = new List<IObservableItem>(_selectedItems.Observables);
                    foreach (var item in selected)
                    {
                        var observable = _items.GetObservable(item.Value);
                        if (observable != null)
                            observable.IsSelected = false;
                    }
                }

                ListenSelectedItems(false);
            }

            _selectedItems = selectedItems ?? new ObservableList<object>();

            ListenSelectedItems(true);
        }

        /// <summary>
        /// Called to setup or teardown SelectedItems event listeners.
        /// </summary>
        protected virtual void ListenSelectedItems(bool listen)
        {
            if (listen)
            {
                _selectedItems.ItemsAdded += OnSelectedItemsAdded;
                _selectedItems.ItemsRemoved += OnSelectedItemsRemoved;
            }
            else
            {
                _selectedItems.ItemsAdded -= OnSelectedItemsAdded;
                _selectedItems.ItemsRemoved -= OnSelectedItemsRemoved;
            }
        }

        /// <summary>
        /// Called when items are added to the SelectedItems observable list
        /// </summary>
        protected virtual void OnSelectedItemsAdded(object sender, DataItemsAddedEventArgs args)
        {
            if (_items == null)
                return;

            // selected items in the Items observable list
            foreach (var selected in args.Added)
            {
                var observable = _items.GetObservable(selected.Value);
                if (observable != null)
                    observable.IsSelected = true;
            }
        }

        /// <summary>
        /// Called when items are removed from the SelectedItems observable list
        /// </summary>
        protected virtual void OnSelectedItemsRemoved(object sender, DataItemsRemovedEventArgs args)
        {
            if (_items == null)
                return;

            // deselect items in Items observable list
            foreach (var deselected in args.Removed)
            {
                var observable = _items.GetObservable(deselected.Value);
                if (observable != null)
                    observable.IsSelected = false;
            }
        }

        /// <summary>
        /// Called when the list of items has been changed.
        /// </summary>
        protected virtual void ItemsChanged()
        {
            if (ListItemTemplates.Count <= 0)
                return; // static list

            Rebuild();
            LayoutChanged();
        }

        /// <summary>
        /// Updates the sort index on the list items.
        /// </summary>
        private void UpdateSortIndex()
        {
            // update not needed for observable list items
            if (_items != null)
                return;

            var index = 0;

            _presentedListItems.ForEach(x =>
            {
                if (!x.IsLive)
                    return;

                x.SortIndex.DirectValue = index + 1;
                x.IsAlternate.Value = AlternateItems.Value && Utils.IsOdd(index + 1);
                ++index;
            });
        }

        /// <summary>
        /// Rebuilds the entire list.
        /// </summary>
        protected virtual void Rebuild()
        {
            // assume a completely new list has been set
            if (_items != null)
            {
                // unsubscribe from change events in the old list
                _items.ItemsAdded -= OnItemsAdded;
                _items.ItemsRemoved -= OnItemsRemoved;
                _items.ItemsModified -= OnItemsModified;
                _items.ItemsMoved -= OnItemsMoved;
                _items.ItemSelectChanged -= OnItemSelectChanged;
                _items.ScrolledTo -= OnScrolledTo;
            }

            _items = Items.Value;

            // clear list
            ClearPresentedItems();

            // add new list
            if (_items != null)
            {
                // subscribe to change events in the new list
                _items.ItemsAdded += OnItemsAdded;
                _items.ItemsRemoved += OnItemsRemoved;
                _items.ItemsModified += OnItemsModified;
                _items.ItemsMoved += OnItemsMoved;
                _items.ItemSelectChanged += OnItemSelectChanged;
                _items.ScrolledTo += OnScrolledTo;

                // add list items
                if (_items.Count > 0)
                {
                    AddRange(0, _items.Count - 1);
                }

                // silently clear selected items
                ListenSelectedItems(false);
                _selectedItems.Clear();
                ListenSelectedItems(true);

                // update selected items to match new Items data model
                var selectedItems = new List<IObservableItem>(_items.SelectedObservables);
                foreach (var item in selectedItems)
                {
                    item.ForceSelected(true);
                }
                SelectedItem.Value = _items.SelectedItem;
            }
        }

        /// <summary>
        /// Event handler for when items are added
        /// </summary>
        protected virtual void OnItemsAdded(object sender, DataItemsAddedEventArgs args)
        {
            switch (args.AddReason)
            {
                case DataItemAddReason.Add:
                case DataItemAddReason.Insert:
                    AddRange(args.StartIndex, args.EndIndex);
                    break;
                case DataItemAddReason.Replace:
                    ItemsReplaced(args.StartIndex, args.EndIndex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // inform parents of update
            NotifyListChanged(true, args.ActionData);
        }

        /// <summary>
        /// Event handler for when items are removed
        /// </summary>
        protected virtual void OnItemsRemoved(object sender, DataItemsRemovedEventArgs args)
        {
            switch (args.RemoveReason)
            {
                case DataItemsRemovedReason.Remove:
                    RemoveRange(args.StartIndex, args.EndIndex);
                    break;
                case DataItemsRemovedReason.Clear:
                    ClearPresentedItems();
                    break;
                case DataItemsRemovedReason.Replace:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // inform parents of update
            NotifyListChanged(true, args.ActionData);
        }

        /// <summary>
        /// Event handler for when items are modified
        /// </summary>
        protected virtual void OnItemsModified(object sender, DataItemsModifiedEventArgs args)
        {
            NotifyListChanged(false, args.ActionData);
        }

        /// <summary>
        /// Event handler for when items are moved
        /// </summary>
        protected virtual void OnItemsMoved(object sender, DataItemsMovedEventArgs args)
        {
            // inform parents of update
            NotifyListChanged(true, args.ActionData);
        }

        /// <summary>
        /// Event handler for when items are selected or deselected
        /// </summary>
        protected virtual void OnItemSelectChanged(object sender, DataItemSelectChangedEventArgs args)
        {
            var observable = sender as IObservableItem;
            if (observable == null)
                return;

            if (args.IsSelected)
            {
                if (!_selectedItems.Contains(observable.Value))
                {
                    SelectedItem.Value = observable.Value;
                    _selectedItems.Add(observable.Value);
                    if (!CanMultiSelect)
                    {
                        var selected = new List<IObservableItem>(_items.SelectedObservables);
                        foreach (var selItem in selected)
                        {
                            if (!Equals(selItem, observable))
                                selItem.IsSelected = false;
                        }
                    }

                    // trigger item selected action
                    if (ItemSelected.HasEntries)
                        ItemSelected.Trigger(new ItemSelectionActionData(observable));
                }
            }
            else
            {
                if (_selectedItems.Remove(observable.Value))
                {
                    if (Equals(SelectedItem.Value, observable.Value))
                        SelectedItem.Value = _selectedItems.Values.FirstOrDefault();

                    // trigger item deselected action
                    if (ItemDeselected.HasEntries)
                        ItemDeselected.Trigger(new ItemSelectionActionData(observable));
                }
            }

            if (Parent != null)
                Parent.Fields.NotifyDependentValueObservers(Id, true, false);
        }

        /// <summary>
        /// Event handler for when list is scrolled to a specific item.
        /// </summary>
        protected virtual void OnScrolledTo(object sender, DataScrollToEventArgs args)
        {
            ScrollTo(args.Index, args.Alignment, args.Offset);
        }

        /// <summary>
        /// Scroll to an item specified by index position.
        /// </summary>
        public virtual void ScrollTo(int index)
        {
            ScrollTo(index, ElementAlignment.Center, new ElementMargin());
        }

        /// <summary>
        /// Scroll to an item specified by index position.
        /// </summary>
        public virtual void ScrollTo(int index, ElementAlignment alignment, ElementMargin offset)
        {
            if (ListPanel == null)
                return;

            if (index >= _presentedListItems.Count || index < 0)
                return;

            var listItem = _presentedListItems[index];
            var itemLayout = listItem.Layout;
            var parentLayout = ((UIView)listItem.LayoutParent).Layout;

            var isVerticalScroll = Overflow.Value == OverflowMode.Overflow
                                   && Orientation.Value == ElementOrientation.Vertical
                                   ||
                                   Overflow.Value == OverflowMode.Wrap
                                   && Orientation.Value == ElementOrientation.Horizontal;

            if (isVerticalScroll)
            {
                // set vertical scroll distance
                var viewportHeight = ListPanel.ScrollRect.ActualHeight;
                var scrollRegionHeight = ScrollContent.Layout.Height.Pixels;

                var scrollHeight = scrollRegionHeight - viewportHeight;
                if (scrollHeight <= 0)
                    return;

                // calculate the scroll position based on alignment and offset
                var parentHeight = parentLayout.AspectPixelHeight;
                var itemHeight = listItem.Layout.AspectPixelHeight;
                var itemPosition = -(parentHeight -
                                     itemLayout.AnchorMin.y * parentHeight + itemLayout.OffsetMax.y);

                if (alignment.HasFlag(ElementAlignment.Bottom))
                {
                    // scroll so item is at bottom of viewport
                    var scrollOffset = itemPosition - (viewportHeight - itemHeight)
                                       + offset.Top.Pixels + offset.Bottom.Pixels;

                    VerticalNormalizedPosition.Value = (1 - scrollOffset / scrollHeight).Clamp(0, 1);
                }
                else if (alignment.HasFlag(ElementAlignment.Left)
                         || alignment.HasFlag(ElementAlignment.Right)
                         || alignment == ElementAlignment.Center)
                {
                    // scroll so item is at center of viewport
                    var scrollOffset = itemPosition - viewportHeight / 2 + itemHeight / 2
                                       + offset.Top.Pixels + offset.Bottom.Pixels;

                    VerticalNormalizedPosition.Value = (1 - scrollOffset / scrollHeight).Clamp(0, 1);
                }
                else
                {
                    // scroll so item is at top of viewport
                    var scrollOffset = itemPosition + offset.Top.Pixels + offset.Bottom.Pixels;
                    VerticalNormalizedPosition.Value = (1 - scrollOffset / scrollHeight).Clamp(0, 1);
                }
            }
            else
            {
                // set horizontal scroll distance
                var viewportWidth = ListPanel.ScrollRect.ActualWidth;
                var scrollRegionWidth = ScrollContent.Layout.Width.Pixels;

                var scrollWidth = scrollRegionWidth - viewportWidth;
                if (scrollWidth <= 0)
                    return;

                // calculate the scroll position based on alignment and offset
                var parentWidth = parentLayout.AspectPixelWidth;
                var itemWidth = listItem.Layout.AspectPixelWidth;
                var itemPosition = -(parentWidth -
                                     itemLayout.AnchorMin.x * parentWidth + itemLayout.OffsetMax.x);

                if (alignment.HasFlag(ElementAlignment.Right))
                {
                    // scroll so item is the right side of viewport
                    var scrollOffset = itemPosition - (viewportWidth - itemWidth)
                                       + offset.Left.Pixels + offset.Right.Pixels;

                    HorizontalNormalizedPosition.Value = (scrollOffset / scrollWidth).Clamp(0, 1);
                }
                else if (alignment.HasFlag(ElementAlignment.Top)
                         || alignment.HasFlag(ElementAlignment.Bottom)
                         || alignment == ElementAlignment.Center)
                {
                    // scroll so item is at center of viewport
                    var scrollOffset = itemPosition - viewportWidth / 2 + itemWidth / 2
                                       + offset.Left.Pixels + offset.Right.Pixels;

                    HorizontalNormalizedPosition.Value = (scrollOffset / scrollWidth).Clamp(0, 1);
                }
                else
                {
                    // scroll so item is at left side of viewport
                    var scrollOffset = itemPosition + offset.Left.Pixels + offset.Right.Pixels;
                    HorizontalNormalizedPosition.Value = (scrollOffset / scrollWidth).Clamp(0, 1);
                }
            }

            ListPanel.ScrollRect.UpdateNormalizedPosition.Value = true;

            NotifyListChanged(false, new ListChangedActionData(ListChangeAction.ScrollTo, index, index));
        }

        protected virtual void NotifyListChanged(bool layoutUpdated, ActionData actionData) {

            if (ListChanged.HasEntries)
                ListChanged.Trigger(actionData);

            // inform parents of update
            if (layoutUpdated)
                NotifyLayoutChanged();

            if (Parent != null)
                Parent.Fields.NotifyDependentValueObservers(Id, true, false);
        }

        public override void ValueObserversNotified(ViewFieldData viewFieldData, HashSet<ViewFieldData> callstack)
        {
            base.ValueObserversNotified(viewFieldData, callstack);

            if (_items == null)
                return;

            var path = viewFieldData.Path;

            if (path != "SelectedItem" && path != "SelectedItems" &&
                !viewFieldData.Path.StartsWith("SelectedItem.") &&
                !viewFieldData.Path.StartsWith("SelectedItems."))
            {
                return;
            }

            var selectionCount = _items.SelectedItems.Count;

            // ensure changes to selected item are propogated to equivalent list item.
            foreach (var listItem in _presentedListItems)
            {
                if (listItem == null)
                    continue;

                if (selectionCount == 1)
                {
                    if (!_items.SelectedItem.Equals(listItem.Item.Value))
                        continue;

                    listItem.Fields.NotifyDependentValueObservers("Item", true);
                    break;
                }

                foreach (var item in _items.SelectedItems)
                {
                    if (!item.Equals(listItem.Item.Value))
                        continue;

                    listItem.Fields.NotifyDependentValueObservers("Item", true);
                    break;
                }
            }
        }

        /// <summary>
        /// Clears the list items if there is no data source.
        /// </summary>
        public bool Clear()
        {
            // clear through data model if present
            if (Items.Value != null)
                return false;

            ClearPresentedItems();
            return true;
        }

        /// <summary>
        /// Clears the list items.
        /// </summary>
        private void ClearPresentedItems() {
            foreach (var presentedItem in _presentedListItems)
            {
                DestroyListItem(presentedItem);
            }

            _presentedListItems.Clear();
        }

        /// <summary>
        /// Adds a range of list items.
        /// </summary>
        private void AddRange(int startIndex, int endIndex)
        {
            if (_items == null)
                return;

            // make sure we have a template
            if (ListItemTemplates.Count <= 0)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to generate list from items. Template missing. Add a template by adding "+
                    "a view with IsTemplate=\"True\" to the list.",
                    GameObjectName));
                return;
            }

            // validate input
            var lastIndex = _items.Count - 1;
            var insertCount = endIndex - startIndex + 1;
            var listMatch = _presentedListItems.Count == _items.Count - insertCount;
            if (startIndex < 0 || startIndex > lastIndex ||
                endIndex < startIndex || endIndex > lastIndex || !listMatch)
            {
                Debug.LogWarning(String.Format("[MarkLight] {0}: List mismatch. Rebuilding list.", GameObjectName));
                Rebuild();
                return;
            }

            // insert items
            //Utils.StartTimer();
            for (var i = startIndex; i <= endIndex; ++i)
            {
                CreateListItem(i);
            }
            //Utils.LogTimer();
        }

        /// <summary>
        /// Removes a range of list items.
        /// </summary>
        private void RemoveRange(int startIndex, int endIndex)
        {
            // validate input
            var lastIndex = _presentedListItems.Count - 1;
            var removeCount = endIndex - startIndex + 1;
            var listMatch = _presentedListItems.Count == Items.Value.Count + removeCount;
            if (startIndex < 0 || startIndex > lastIndex ||
                endIndex < startIndex || endIndex > lastIndex || !listMatch)
            {
                Debug.LogWarning(String.Format("[MarkLight] {0}: List mismatch. Rebuilding list.", GameObjectName));
                Rebuild();
                return;
            }

            // remove items
            for (var i = endIndex; i >= startIndex; --i)
            {
                DestroyListItem(i);
            }
        }

        /// <summary>
        /// Called when item data in the list have been replaced.
        /// </summary>
        private void ItemsReplaced(int startIndex, int endIndex)
        {
            // validate input
            var lastIndex = _presentedListItems.Count - 1;
            var listMatch = _presentedListItems.Count ==_items.Count;
            if (startIndex < 0 || startIndex > lastIndex || endIndex < startIndex || endIndex > lastIndex || !listMatch)
            {
                Debug.LogWarning(String.Format("[MarkLight] {0}: List mismatch. Rebuilding list.", GameObjectName));
                Rebuild();
                return;
            }

            // replace items
            for (var i = startIndex; i <= endIndex; ++i)
            {
                ItemReplaced(i);
            }
        }

        /// <summary>
        /// Called when item data in list has been replaced.
        /// </summary>
        private void ItemReplaced(int index)
        {
            var observableItem = _items.Observables[index];
            var listItem = _presentedListItems[index];
            listItem.Item.Value = observableItem;
            listItem.Fields.NotifyDependentValueObservers("Item", true);
        }

        /// <summary>
        /// Creates and initializes a new list item.
        /// </summary>
        private void CreateListItem(int index)
        {
            var observableItem = _items.Observables[index];
            var template = GetListItemTemplate(observableItem.Value);

            var content = UseVirtualization
                ? _virtualizedItems.VirtualizedItemsContainer
                : Content;

            var newItemView = content.CreateView(template, -1, _viewPools.Get(template));
            newItemView.Template = template;

            _presentedListItems.Insert(index, newItemView);

            newItemView.SortIndex.Value = index + 1;
            newItemView.Item.Value = observableItem;
            newItemView.Activate();

            // initialize view
            newItemView.InitializeViews();

            if (ListPanel != null)
                ListPanel.ScrollRect.UnblockDragEvents(newItemView);
        }

        /// <summary>
        /// Gets template based on item data.
        /// </summary>
        private ListItem GetListItemTemplate(object itemData)
        {
            if (ListItemTemplates.Count <= 0)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to generate list from items. Template missing. Add a template by adding "+
                    "a view with IsTemplate=\"True\" to the list.",
                    GameObjectName));
                return null;
            }

            if (ListItemTemplates.Count == 1 || itemData == null)
            {
                return ListItemTemplates[0];
            }

            // get method GetTemplateId from list item
            var type = itemData.GetType();
            var method = type.GetMethod("GetTemplateId",
                BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Static);
            if (method == null)
            {
                return ListItemTemplates[0];
            }

            var templateId = method.IsStatic
                ? method.Invoke(null, null) as string
                : method.Invoke(itemData, null) as string;

            return ListItemTemplates.FirstOrDefault(
                       x => String.Equals(x.Id, templateId, StringComparison.OrdinalIgnoreCase)) ??
                   ListItemTemplates[0];
        }

        /// <summary>
        /// Destroys a list item.
        /// </summary>
        private void DestroyListItem(int index)
        {
            var itemView = _presentedListItems[index];
            DestroyListItem(itemView);
            _presentedListItems.RemoveAt(index);
        }

        /// <summary>
        /// Destroys a list item.
        /// </summary>
        private void DestroyListItem(ListItem presentedItem)
        {
            // deselect the item first
            presentedItem.Item.Value = null;

            var viewPool = presentedItem.Template != null
                ? _viewPools.Get(presentedItem.Template)
                : null;

            presentedItem.Destroy(viewPool);
        }

        /// <summary>
        /// Creates a container for virtualized items which will be presented on demand. Used to improve performance.
        /// </summary>
        public VirtualizedItems GetVirtualizedItems()
        {
            if (LayoutRoot == null)
                return null;

            // does a virtualized items container exist for this view?
            var virtualizedItemsContainer =
                LayoutRoot.Find<VirtualizedItemsContainer>(
                    x => ReferenceEquals(x.Owner, this),
                    ViewSearchArgs.NonRecursive);

            if (virtualizedItemsContainer == null)
            {
                // no. create a new one
                virtualizedItemsContainer = LayoutRoot.CreateView<VirtualizedItemsContainer>();
                virtualizedItemsContainer.IsActive.DirectValue = false;
                virtualizedItemsContainer.Id = GameObjectName;
                virtualizedItemsContainer.Owner = this;
                //virtualizedItemsContainer.HideFlags.Value = UnityEngine.HideFlags.DontSave;
                virtualizedItemsContainer.HideFlags.Value = UnityEngine.HideFlags.HideAndDontSave;
                virtualizedItemsContainer.InitializeViews();
            }

            return new VirtualizedItems(virtualizedItemsContainer);
        }

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            SetSelectedItems(new ObservableList<object>());

            // remove panel if not used
            if (ListPanel != null && !IsScrollable)
            {
                Content = ListMask != null ? ListMask.Content : this;
                ListPanel.DestroyAndMoveContent(Content);
                ScrollContent.DestroyAndMoveContent(Content);
                ListPanel = null;
                ScrollContent = null;
            }

            // remove list mask if not used
            if (ListMask != null && !UseListMask)
            {
                if (Content == ListMask.Content)
                {
                    Content = this;
                }

                ListMask.DestroyAndMoveContent(this);
                ListMask = null;
            }

            _presentedListItems = new List<ListItem>();

            // set up virtualization
            UseVirtualization.DirectValue = InitializeVirtualization();
            UpdatePresentedListItems();

            LoadListItemTemplates();

            if (ListItemTemplates.Count > 0)
            {
                //  get view pools for item templates
                _viewPools = new Dictionary<View, ViewPool>();
                foreach (var template in ListItemTemplates)
                {
                    // should pooling be used for this template?
                    if (!PoolSize.IsSet && !template.PoolSize.IsSet)
                        continue; // no.

                    int poolSize = template.PoolSize.IsSet ? template.PoolSize : PoolSize;
                    int maxPoolSize = template.MaxPoolSize.IsSet ? template.MaxPoolSize : MaxPoolSize;

                    var viewPool = LayoutRoot.GetViewPool(template.GameObjectName, template, poolSize, maxPoolSize);
                    _viewPools.Add(template, viewPool);
                }
            }
        }

        /// <summary>
        /// Called once at initialization to set the list up for virtualization.
        /// </summary>
        private bool InitializeVirtualization()
        {
            if (!UseVirtualization)
                return false;

            // verify things are correctly set up for virtualization
            if (Overflow.Value == OverflowMode.Wrap || IsScrollable == false)
            {
                Debug.LogWarning(String.Format(
                    "[MarkLight] {0}: Can't virtualize list because IsScrollable is false or Overflow is set to Wrap.",
                    GameObjectName));
                return false;
            }

            if (DisableItemArrangement)
            {
                Debug.LogWarning(String.Format(
                    "[MarkLight] {0}: Can't virtualize list because DisableItemArrangement is set to True.",
                    GameObjectName));
                return false;
            }

            // check if templates are set and that they have the same height/width
            if (ListItemTemplates.Count <= 0)
            {
                Debug.LogWarning(String.Format(
                    "[MarkLight] {0}: Can't virtualize list because no item template found. Only dynamic lists "+
                    "can be virtualized.",
                    GameObjectName));
                return false;
            }

            // get virtualized items container
            _virtualizedItems = GetVirtualizedItems();
            _virtualizedItems.Orientation = Orientation.Value;
            return true;
        }

        /// <summary>
        /// Load list item templates.
        /// </summary>
        private void LoadListItemTemplates()
        {
            if (_listItemTemplates != null)
                return;

            _listItemTemplates = Content.GetChildren<ListItem>(
                x => x.IsTemplate,
                ViewSearchArgs.NonRecursive);

            if (!ShowTemplateInEditor || !Application.isEditor || Application.isPlaying)
                _listItemTemplates.ForEach(x => x.Deactivate());
        }

        /// <summary>
        /// Updates list of presented list items. Needs to be called after list items are added manually to the list.
        /// </summary>
        public void UpdatePresentedListItems()
        {
            _presentedListItems.Clear();
            _presentedListItems.AddRange(GetActiveListItems());
            UpdateSortIndex();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns list item template.
        /// </summary>
        public IList<ListItem> ListItemTemplates
        {
            get
            {
                return _listItemTemplates.AsReadOnly();
            }
        }

        /// <summary>
        /// Returns list of presented list items.
        /// </summary>
        public IList<ListItem> PresentedListItems
        {
            get
            {
                return _presentedListItems.AsReadOnly();
            }
        }

        #endregion

        #region Classes

        /// <summary>
        /// Custom GroupLayout for use with List
        /// </summary>
        private class ListGroupLayout : GroupLayoutCalculator
        {
            private static readonly ElementMargin EmptyMargin = new ElementMargin();
            private readonly List _list;

            public ListGroupLayout(List list) {
                _list = list;
            }

            protected override float GetHorizontalPadding(LayoutData layout)
            {
                var listMaskMargin = _list.ListMask != null
                    ? _list.ListMask.Margin.Value
                    : EmptyMargin;

                return base.GetHorizontalPadding(layout) +
                       + listMaskMargin.Left.Pixels + listMaskMargin.Right.Pixels;
            }

            protected override float GetVerticalPadding(LayoutData layout)
            {
                var listMaskMargin = _list.ListMask != null
                    ? _list.ListMask.Margin.Value
                    : EmptyMargin;

                return base.GetVerticalPadding(layout)
                       + listMaskMargin.Top.Pixels + listMaskMargin.Bottom.Pixels;
            }
        }

        #endregion
    }
}
