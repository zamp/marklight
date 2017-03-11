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
        /// Singleton instance.
        /// </summary>
        public static readonly DefaultLayoutCalculator Instance = new DefaultLayoutCalculator(false, false);

        /// <summary>
        /// Singleton instance with IsAffectedByChildren flag set.
        /// </summary>
        public static readonly DefaultLayoutCalculator AffectedByChildren = new DefaultLayoutCalculator(false, true);

        /// <summary>
        /// Singleton instance with IsChildLayout flag set.
        /// </summary>
        public static readonly DefaultLayoutCalculator ChildLayout = new DefaultLayoutCalculator(true, false);

        private readonly bool _isChildLayout;
        private readonly bool _isAffectedByChildren;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public DefaultLayoutCalculator(bool isChildLayout, bool isAffectedByChildren)
        {
            _isChildLayout = isChildLayout;
            _isAffectedByChildren = isAffectedByChildren;
        }

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
            get { return _isChildLayout; }
        }

        public override bool IsAffectedByChildren
        {
            get { return _isAffectedByChildren; }
        }

        #endregion
    }
}