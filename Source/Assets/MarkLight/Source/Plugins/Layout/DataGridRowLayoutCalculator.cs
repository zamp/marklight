using System;
using System.Collections.Generic;
using MarkLight.Views.UI;
using UnityEngine;

namespace MarkLight
{
    public class DataGridRowLayoutCalculator : LayoutCalculator
    {
        #region Fields

        /// <summary>
        /// The parent data grid of the row.
        /// </summary>
        public DataGrid ParentDataGrid;

        /// <summary>
        /// Determine if the data grid row is the header row.
        /// </summary>
        public bool IsHeader;

        #endregion

        #region Methods

        public override bool CalculateLayoutChanges(UIView view, IList<UIView> children, LayoutChangeContext context)
        {
            if (ParentDataGrid == null)
                return view.Layout.IsDirty;

            // require row header is already calculated
            if (!IsHeader && ParentDataGrid.RowHeader != null)
                context.CalculateAsParent(ParentDataGrid.RowHeader);

            // arrange columns according to the settings in the parent datagrid
            var columns = view.GetChildren<Column>(
                column => column.PositionType.Value != ElementPositionType.Absolute,
                ViewSearchArgs.NonRecursive);

            var columnSpacing = (columns.Count - 1) *
                                ParentDataGrid.Layout.WidthToPixels(ParentDataGrid.ColumnSpacing.Value.Pixels) /
                                columns.Count;

            var columnsToFill = new List<Column>();
            var pixelWidth = IsHeader || ParentDataGrid.RowHeader == null
                ? view.Layout.AspectPixelWidth
                : ParentDataGrid.RowHeader.Layout.AspectPixelWidth;

            var totalWidth = 0f;
            var maxHeight = 0f;

            // get column headers if the view is not the header row
            var columnHeaders = !IsHeader && ParentDataGrid.RowHeader != null
                ? ParentDataGrid.RowHeader.GetChildren<ColumnHeader>(ViewSearchArgs.NonRecursive)
                : null;

            for (var i=0; i < columns.Count; i++)
            {
                var column = columns[i];
                var defWidth = column.Width.Value;

                maxHeight = Mathf.Max(maxHeight, column.Layout.PixelHeight);

                if (IsHeader || columnHeaders == null)
                {
                    // set aside fill width columns
                    if (defWidth.Fill)
                    {
                        columnsToFill.Add(column);
                        continue;
                    }

                    column.Layout.Width = defWidth.Unit == ElementSizeUnit.Percents
                        ? ElementSize.FromPixels(defWidth.Percent * pixelWidth - columnSpacing)
                        : ElementSize.FromPixels(defWidth.Pixels - columnSpacing);
                }
                else
                {
                    var header = columnHeaders[i];
                    LayoutData.Copy(header.Layout.Width, column.Layout.Width);
                }

                totalWidth += column.Layout.Width.Pixels;
            }

            if (!view.Height.IsSet)
            {
                view.Layout.Height = ElementSize.FromPixels(maxHeight);
            }

            if (!IsHeader && !view.Width.IsSet && ParentDataGrid.RowHeader != null)
            {
                view.Layout.Width = ElementSize.FromPixels(ParentDataGrid.RowHeader.Layout.PixelWidth);
            }

            // adjust width of fill columns
            if (columnsToFill.Count > 0)
            {
                var fillWidth = pixelWidth - totalWidth;
                var columnWidth = Math.Max(columnSpacing, fillWidth / columnsToFill.Count);
                foreach (var column in columnsToFill)
                {
                    column.Layout.Width = ElementSize.FromPixels(columnWidth - columnSpacing);
                }
            }

            // adjust column offsets and settings
            float offset = 0;
            foreach (var column in columns)
            {
                if (!column.TextAlignment.IsSet)
                {
                    var textAlignment = ParentDataGrid.ColumnHeaderTextAlignment.IsSet
                        ? ParentDataGrid.ColumnHeaderTextAlignment
                        : ParentDataGrid.ColumnTextAlignment;

                    column.TextAlignment.Value = textAlignment.Value;
                }

                if (!column.TextMargin.IsSet)
                {
                    var textMargin = ParentDataGrid.ColumnHeaderTextMargin.IsSet
                        ? ParentDataGrid.ColumnHeaderTextMargin
                        : ParentDataGrid.ColumnTextMargin;

                    column.TextMargin.Value = textMargin.Value;
                }

                column.Layout.Height = ElementSize.FromPixels(maxHeight);
                column.Layout.OffsetFromParent = new ElementMargin(offset, 0, 0, 0);
                offset += column.Layout.Width.Pixels + columnSpacing;
                context.NotifyLayoutUpdated(column, true);
            }

            return true;
        }

        #endregion

        #region Properties

        public override bool IsChildLayout
        {
            get { return true; }
        }

        public override bool IsAffectedByChildren
        {
            get { return true; }
        }

        #endregion
    }
}