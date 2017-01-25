using System.Collections.Generic;

namespace MarkLight.Views
{
    /// <summary>
    /// View tree node allows for optimized for recursion.
    /// </summary>
    public class ViewRecursionNode
    {
        public readonly View View;
        public readonly ViewRecursionNode Parent;
        public readonly List<ViewRecursionNode> Children;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="view">The view that the node represents.</param>
        /// <param name="parent">The views parent view as a node.</param>
        public ViewRecursionNode(View view, ViewRecursionNode parent) {
            View = view;
            Parent = parent;
            Children = new List<ViewRecursionNode>();
            if (parent != null)
                parent.Children.Add(this);
        }

        /// <summary>
        /// Find a node using a view. The parent, self, and all child nodes (recursive) are searched.
        /// </summary>
        /// <param name="view">The view to find</param>
        /// <returns>The node that represents the view or null if not found.</returns>
        public ViewRecursionNode Find(View view) {
            if (View == view)
                return this;

            if (Parent != null && Parent.View == view)
                return Parent;

            foreach (var child in Children)
            {
                if (child.View == view)
                    return child;

                child.Find(view);
            }
            return null;
        }
    }
}