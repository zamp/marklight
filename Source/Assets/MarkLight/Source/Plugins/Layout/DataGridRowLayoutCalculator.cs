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

            context.CalculateRequired(ParentDataGrid);

            // require row header must already be calculated.
            if (!IsHeader && ParentDataGrid.RowHeader != null)
                context.CalculateRequired(ParentDataGrid.RowHeader);

            // arrange columns according to the settings in the parent datagrid
            var columns = view.GetChildren<Column>(
                column => column.PositionType.Value != ElementPositionType.Absolute,
                ViewSearchArgs.NonRecursive);

            var columnsToFill = new List<Column>();

            var rowWidth = IsHeader || ParentDataGrid.RowHeader == null
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

                if (IsHeader || columnHeaders == null)
                {
                    if (!column.Margin.IsSet && !IsHeader && columnHeaders != null)
                    {
                        var header = columnHeaders[i];
                        column.Layout.Margin = header.Layout.Margin;
                    }

                    // set aside fill width columns
                    if (defWidth.Fill)
                    {
                        columnsToFill.Add(column);
                        maxHeight = Mathf.Max(maxHeight, column.Layout.AspectPixelHeight);
                        continue;
                    }

                    column.Layout.TargetWidth = defWidth.Unit == ElementSizeUnit.Percents
                        ? ElementSize.FromPixels(defWidth.Percent * rowWidth - column.Layout.HorizontalMarginPixels)
                        : ElementSize.FromPixels(defWidth.Pixels - column.Layout.HorizontalMarginPixels);
                }
                else
                {
                    // copy width of header column
                    var header = columnHeaders[i];
                    column.Layout.TargetWidth = header.Layout.Width;

                    if (!column.Margin.IsSet)
                    {
                        column.Layout.Margin = header.Layout.Margin;
                    }
                }

                maxHeight = Mathf.Max(maxHeight, column.Layout.AspectPixelHeight);
                totalWidth += column.Layout.Width.Pixels + column.Layout.HorizontalMarginPixels;
            }

            // adjust width of fill columns
            if (columnsToFill.Count > 0)
            {
                var fillWidth = rowWidth - totalWidth;
                var columnWidth = fillWidth / columnsToFill.Count;

                foreach (var column in columnsToFill)
                {
                    column.Layout.TargetWidth = ElementSize.FromPixels(
                        columnWidth - column.Layout.HorizontalMarginPixels);

                    maxHeight = Mathf.Max(maxHeight, column.Layout.AspectPixelHeight);
                }
            }

            if (!view.Height.IsSet)
            {
                view.Layout.TargetHeight = ElementSize.FromPixels(maxHeight);
            }

            if (!IsHeader && !view.Width.IsSet && ParentDataGrid.RowHeader != null)
            {
                var headerLayout = ParentDataGrid.RowHeader.Layout;
                view.Layout.TargetWidth = headerLayout.Width;
            }

            // adjust column offsets and settings
            float offset = 0;
            foreach (var column in columns)
            {
                column.Layout.IsDirty = true;
                column.Layout.TargetHeight = ElementSize.FromPixels(maxHeight);
                column.Layout.OffsetFromParent = new ElementMargin(offset, 0f, 0f, 0f);

                offset += column.Layout.Width.Pixels + column.Layout.HorizontalMarginPixels;
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