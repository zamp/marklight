
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

        public override void InitializeInternalDefaultValues() {
            base.InitializeInternalDefaultValues();

            LayoutCalculator = new DataGridRowLayoutCalculator();
        }

        public override bool CalculateLayoutChanges(LayoutChangeContext context)
        {
            var layout = LayoutCalculator as DataGridRowLayoutCalculator;
            if (layout != null)
            {
                layout.IsHeader = false;
                layout.ParentDataGrid = ParentDataGrid;
            }

            return LayoutCalculator.CalculateLayoutChanges(this, context);
        }

        public override void SetState(string stateName)
        {
            base.SetState(stateName);
            this.ForEachChild<Column>(x => x.SetState(stateName), ViewSearchArgs.NonRecursive);
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
