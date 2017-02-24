using System;
using System.Collections.Generic;
using System.Linq;

namespace MarkLight.Views.UI
{
    /// <summary>
    /// Row view.
    /// </summary>
    /// <d>The row view displays the content of a row in the data grid.</d>
    [HideInPresenter]
    public class Row : ListItem
    {
        #region Fields

        private DataGrid _parentDataGrid;

        #endregion

        #region Methods

        public override bool CalculateLayoutChanges(LayoutChangeContext context) {

            if (ParentDataGrid == null)
            {
                return false;
            }

            // arrange columns according to the settings in the parent datagrid
            var columns = this.GetChildren<Column>(false);
            var columnHeaders = ParentDataGrid.RowHeader != null
                ? ParentDataGrid.RowHeader.GetChildren<ColumnHeader>(false)
                : new List<ColumnHeader>();

            if (columnHeaders.Count > 0 && columns.Count > columnHeaders.Count)
            {
                Utils.LogWarning("[MarkLight] {0}: Row contains more columns ({1}) than there are column headers ({2}).", name,
                    columns.Count, columnHeaders.Count);

                // remove columns outside the bounds
                columns = new List<Column>(columns.Take(columnHeaders.Count));
            }

            // if no headers exist arrange columns to fit row with equal widths
            if (columnHeaders.Count <= 0)
            {
                // adjust columns to row width
                var columnWidth = (ActualWidth - (columns.Count - 1)
                                                  * ParentDataGrid.ColumnSpacing.Value.Pixels) / columns.Count;
                foreach (var column in columns)
                {
                    column.Layout.Width = ElementSize.FromPixels(columnWidth);
                    context.NotifyLayoutUpdated(column);
                }
            }
            else
            {
                // adjust width of columns based on headers
                var columnSpacing = (columns.Count - 1) * ParentDataGrid.ColumnSpacing.Value.Pixels / columns.Count;
                var columnsToFill = new List<Column>();
                float totalWidth = 0;

                for (var i = 0; i < columns.Count; ++i)
                {
                    var defWidth = columnHeaders[i].Layout.Width;
                    if (!columnHeaders[i].Width.IsSet || defWidth.Fill)
                    {
                        columnsToFill.Add(columns[i]);
                        continue;
                    }

                    columns[i].Layout.Width = defWidth.Unit == ElementSizeUnit.Percents
                        ? new ElementSize(defWidth.Percent * ActualWidth - columnSpacing, ElementSizeUnit.Pixels)
                        : new ElementSize(defWidth.Pixels - columnSpacing, ElementSizeUnit.Pixels);

                    totalWidth += columns[i].Width.Value.Pixels;
                }

                // adjust width of fill columns
                if (columnsToFill.Count > 0)
                {
                    var columnWidth = Math.Max(columnSpacing, (ActualWidth - totalWidth) / columnsToFill.Count);
                    foreach (var column in columnsToFill)
                    {
                        column.Layout.Width = new ElementSize(columnWidth - columnSpacing, ElementSizeUnit.Pixels);
                    }
                }
            }

            // adjust column offsets and settings
            float offset = 0;
            foreach (var column in columns)
            {
                if (!column.TextAlignment.IsSet)
                {
                    column.TextAlignment.Value = ParentDataGrid.ColumnTextAlignment.Value;
                }

                if (!column.TextMargin.IsSet)
                {
                    column.TextMargin.Value = ParentDataGrid.ColumnTextMargin.Value;
                }

                column.Layout.OffsetFromParent = new ElementMargin(offset, 0, 0, 0);
                offset += column.Width.Value.Pixels + ParentDataGrid.ColumnSpacing.Value.Pixels;
                context.NotifyLayoutUpdated(column);
            }

            return base.CalculateLayoutChanges(context);
        }

        /// <summary>
        /// Sets the state of the view.
        /// </summary>
        public override void SetState(string stateName)
        {
            base.SetState(stateName);
            this.ForEachChild<Column>(x => x.SetState(stateName), false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets parent datagrid.
        /// </summary>
        public DataGrid ParentDataGrid
        {
            get
            {
                if (_parentDataGrid == null)
                {
                    _parentDataGrid = this.FindParent<DataGrid>();
                }

                return _parentDataGrid;
            }
        }

        #endregion
    }
}
