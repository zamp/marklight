
namespace MarkLight.Views.UI
{
    /// <summary>
    /// Tab view.
    /// </summary>
    /// <d>Represents a tab in the tab panel.</d>
    [HideInPresenter]
    public class Tab : UIView
    {
        #region Fields

        /// <summary>
        /// Data model item.
        /// </summary>
        /// <d>Contains the item that the tab represents.</d>
        public ItemViewField Item;

        /// <summary>
        /// Indicates if tab is selected.
        /// </summary>
        /// <d>Boolean indicating if the tab is selected.</d>
        public _bool IsSelected;

        /// <summary>
        /// Tab header text.
        /// </summary>
        /// <d>Tab header text.</d>
        public _string Text;

        #endregion
    }
}
