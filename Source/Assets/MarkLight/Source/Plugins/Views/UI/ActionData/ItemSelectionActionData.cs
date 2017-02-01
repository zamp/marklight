#region Using Statements
using MarkLight.Views.UI;
using System;
using UnityEngine.EventSystems;
#endregion

namespace MarkLight.Views.UI
{
    /// <summary>
    /// Item selection action data.
    /// </summary>
    public class ItemSelectionActionData : ActionData
    {
        #region Fields

        public readonly IObservableItem Item;
        public readonly bool IsSelected;

        #endregion

        #region Constructor

        public ItemSelectionActionData(IObservableItem item) {
            Item = item;
            IsSelected = item.IsSelected;
        }

        #endregion
    }
}
