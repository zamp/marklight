
namespace MarkLight.Views.UI
{
    /// <summary>
    /// RowHeader view.
    /// </summary>
    /// <d>The row header view displays the content of a header row in the data grid.</d>
    [HideInPresenter]
    public class RowHeader : UIView
    {
        #region Fields

        private DataGrid _parentDataGrid;

        #endregion

        #region Methods

        public override void InitializeInternalDefaultValues() {
            base.InitializeInternalDefaultValues();

            LayoutCalculator = new DataGridRowLayoutCalculator();
        }

        public override bool CalculateLayoutChanges(LayoutChangeContext context)
        {
            var layout = LayoutCalculator as DataGridRowLayoutCalculator;
            if (layout != null)
            {
                layout.IsHeader = true;
                layout.ParentDataGrid = ParentDataGrid;
            }

            return LayoutCalculator.CalculateLayoutChanges(this, context);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets parent datagrid.
        /// </summary>
        public DataGrid ParentDataGrid
        {
            get {
                return _parentDataGrid
                         ?? (_parentDataGrid = this.FindParent<DataGrid>());
            }
        }

        #endregion
    }
}
