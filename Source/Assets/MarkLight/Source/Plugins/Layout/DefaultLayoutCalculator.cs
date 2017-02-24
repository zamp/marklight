using System.Collections.Generic;
using MarkLight.Views.UI;

namespace MarkLight
{
    /// <summary>
    /// Layout calculator that is used by default.
    /// </summary>
    public class DefaultLayoutCalculator : LayoutCalculator
    {
        #region Fields

        /// <summary>
        /// Singletone instance.
        /// </summary>
        public static readonly DefaultLayoutCalculator Instance = new DefaultLayoutCalculator();

        #endregion

        #region Methods

        public override bool CalculateLayoutChanges(UIView parent, IList<UIView> children, LayoutChangeContext context)
        {
            return parent.Layout.IsDirty;
        }

        #endregion

        #region Properties

        public override bool IsChildLayout
        {
            get { return false; }
        }

        #endregion
    }
}