using System;
using System.Collections.Generic;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// View search extension methods.
    /// </summary>
    public static class ViewSearchExtensions
    {
        #region Fields

        private static readonly List<View[]> ViewArrayPool = new List<View[]>(50);
        private static readonly List<View[]> DirtyViewArrayPool = new List<View[]>(50);
        private const int PooledViewBufferSize = 15;

        #endregion

        #region Methods

        /// <summary>
        /// Traverses the view object tree and performs an action on each child until the action returns false.
        /// </summary>
        public static void ForEachChild<T>(this View view, Action<T> action, ViewSearchArgs args) where T : View
        {
            DoUntil<T>(view, x =>
            {
                action(x);
                return true;
            }, args);
        }

        /// <summary>
        /// Traverses the view object tree and performs an action on each child until the action returns false.
        /// </summary>
        public static void ForEachChild<T>(this View view,
                                           Predicate<T> predicate, ViewSearchArgs args) where T : View
        {
            DoUntil(view, predicate, args);
        }

        /// <summary>
        /// Traverses the view object tree and performs an action on each child until the action returns false.
        /// </summary>
        public static void ForEachChild<T>(this GameObject gameObject,
                                           Action<T> action, ViewSearchArgs args) where T : View
        {
            var view = gameObject.GetComponent<View>();
            if (view == null)
                return;

            ForEachChild(view, action, args);
        }

        /// <summary>
        /// Traverses the view object tree and performs an action on each child until the action returns false.
        /// </summary>
        public static void ForEachChild<T>(this GameObject gameObject,
                                           Predicate<T> predicate, ViewSearchArgs args) where T : View
        {
            var view = gameObject.GetComponent<View>();
            if (view == null)
                return;

            ForEachChild(view, predicate, args);
        }

        /// <summary>
        /// Traverses the view object tree and performs an action on this view and its children until the action returns false.
        /// </summary>
        public static void ForThisAndEachChild<T>(this View view,
                                                  Action<T> action, ViewSearchArgs args) where T : View
        {
            ForThisAndEachChild<T>(view, x =>
            {
                action(x);
                return true;
            }, args);
        }

        /// <summary>
        /// Traverses the view object tree and performs an action on this view and its children until the action returns false.
        /// </summary>
        public static void ForThisAndEachChild<T>(this View view,
            Predicate<T> predicate, ViewSearchArgs args) where T : View
        {
            var thisView = view as T;
            if (thisView != null)
            {
                if (!args.SkipInactive || thisView.IsActive)
                {
                    if (!predicate(thisView))
                    {
                        return;
                    }
                }
            }
            ForEachChild(view, predicate, args);
        }

        /// <summary>
        /// Traverses the view object tree and performs an action on each child until the predicate returns false.
        /// </summary>
        public static void ForThisAndEachChild<T>(this GameObject gameObject,
                                                  Action<T> action, ViewSearchArgs args) where T : View
        {
            ForThisAndEachChild<T>(gameObject, x =>
            {
                action(x);
                return true;
            }, args);
        }

        /// <summary>
        /// Traverses the view object tree and performs an action on each child until the predicate returns false.
        /// </summary>
        public static void ForThisAndEachChild<T>(this GameObject gameObject,
                                                  Predicate<T> predicate, ViewSearchArgs args) where T : View
        {
            var view = gameObject.GetComponent<T>();
            if (view == null)
                return;

            if (args.SkipInactive && !view.IsActive)
                return;

            if (!predicate(view))
                return;

            ForEachChild(view, predicate, args);
        }

        /// <summary>
        /// Traverses the view object tree and returns the first view that matches the predicate.
        /// </summary>
        public static T Find<T>(this View view, ViewSearchArgs args) where T : View
        {
            return Find<T>(view, x => true, args);
        }

        /// <summary>
        /// Returns first view of type T found.
        /// </summary>
        public static T Find<T>(this View view, Predicate<T> predicate, ViewSearchArgs args) where T : View
        {
            T result = null;

            DoUntil<T>(view, x =>
            {
                if (!predicate(x))
                    return true;

                result = x;
                return false;
            }, args);

            return result;
        }

        /// <summary>
        /// Returns first view of type T found.
        /// </summary>
        public static T Find<T>(this GameObject gameObject, ViewSearchArgs args) where T : View
        {
            var view = gameObject.GetComponent<View>();
            return view == null ? null : Find<T>(view, args);
        }

        /// <summary>
        /// Returns first view of type T found.
        /// </summary>
        public static T Find<T>(this GameObject gameObject,
                                Predicate<T> predicate, ViewSearchArgs args) where T : View
        {
            var view = gameObject.GetComponent<View>();
            return view == null ? null : Find(view, predicate, args);
        }

        /// <summary>
        /// Returns first view of type T with the specified ID.
        /// </summary>
        public static T Find<T>(this View view, string id, ViewSearchArgs args) where T : View
        {
            return Find<T>(view, x => String.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase), args);
        }

        /// <summary>
        /// Returns first ascendant of type T found that matches the predicate.
        /// </summary>
        public static T FindParent<T>(this View view, Predicate<T> predicate) where T : View
        {
            while (true)
            {
                var parent = view.LayoutParent;
                if (parent == null)
                {
                    return null;
                }

                if (parent is T && predicate(parent as T))
                {
                    return parent as T;
                }

                view = parent;
            }
        }

        /// <summary>
        /// Returns first ascendant of type T found.
        /// </summary>
        public static T FindParent<T>(this View view) where T : View
        {
            return view.FindParent<T>(x => true);
        }

        /// <summary>
        /// Performs an action on all ascendants of a view.
        /// </summary>
        public static void ForEachParent<T>(this View view, Action<T> action) where T : View
        {
            var parent = view.transform.parent;
            if (parent == null)
                return;

            var component = parent.GetComponent<T>();
            if (component != null)
            {
                action(component);
            }

            parent.gameObject.ForEachParent(action);
        }

        /// <summary>
        /// Performs an action on all ascendants of a view.
        /// </summary>
        public static void ForEachParent<T>(this GameObject gameObject, Action<T> action) where T : View
        {
            var view = gameObject.GetComponent<View>();
            if (view != null)
            {
                ForEachParent(view, action);
            }
        }

        /// <summary>
        /// Performs an action on this view and all its ascendants.
        /// </summary>
        public static void ForThisAndEachParent<T>(this GameObject gameObject, Action<T> action) where T : View
        {
            var view = gameObject.GetComponent<T>();
            if (view != null)
            {
                action(view);
            }

            ForEachParent(view, action);
        }

        /// <summary>
        /// Performs an action on this view and all its ascendants.
        /// </summary>
        public static void ForThisAndEachParent<T>(this View view, Action<T> action) where T : View
        {
            var thisView = view as T;
            if (thisView != null)
            {
                action(thisView);
            }
            ForEachParent(view, action);
        }

        /// <summary>
        /// Gets a list of all descendants matching the predicate.
        /// </summary>
        public static List<T> GetChildren<T>(this View view, Predicate<T> predicate, ViewSearchArgs args)
                                             where T : View
        {
            var children = new List<T>();

            ForEachChild<T>(view, x =>
                {
                    if (predicate(x))
                    {
                        children.Add(x);
                    }
                }, args);

            return children;
        }

        /// <summary>
        /// Gets a list of all descendants.
        /// </summary>
        public static List<T> GetChildren<T>(this View view, ViewSearchArgs args)
            where T : View
        {
            var children = new List<T>();

            ForEachChild<T>(view, x =>
            {
               children.Add(x);
            }, args);

            return children;
        }

        /// <summary>
        /// Gets a list of all descendants matching the predicate.
        /// </summary>
        public static List<T> GetChildren<T>(this GameObject gameObject,
                                             Predicate<T> predicate, ViewSearchArgs args) where T : View
        {
            var view = gameObject.GetComponent<View>();

            return view == null
                ? new List<T>()
                : GetChildren(view, predicate, args);
        }

        /// <summary>
        /// Gets child at index.
        /// </summary>
        public static View GetChild(this View view, int index, bool countOnlyActive = false)
        {
            if (!countOnlyActive)
                return view.LayoutChildren[index];

            var i = 0;
            foreach (var childView in view.LayoutChildren)
            {
                if (childView == null || !childView.IsActive)
                    continue;

                if (i == index)
                    return childView;

                ++i;
            }

            return null;
        }

        /// <summary>
        /// Traverses the view object tree and performs an action on each child until the action returns false.
        /// </summary>
        public static void DoUntil<T>(this View view, Predicate<T> predicate, ViewSearchArgs args)
                                    where T : View
        {
            switch (args.TraversalAlgorithm)
            {
                case TraversalAlgorithm.DepthFirst:
                    DoUntilDepth(view, predicate, args);
                    break;

                case TraversalAlgorithm.BreadthFirst:
                    DoUntilBreadth(view, predicate, args);
                    break;

                case TraversalAlgorithm.ReverseDepthFirst:
                    DoUntilReverseDepth(view, predicate, args);
                    break;

                case TraversalAlgorithm.ReverseBreadthFirst:
                    DoUntilReverseBreadth(view, predicate, args);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CleanViewBuffers();
        }

        private static void DoUntilDepth<T>(View view, Predicate<T> predicate, ViewSearchArgs args) where T : View
        {
            var count = view.LayoutChildren.Count;
            for (var i=0; i < count; i++)
            {
                var childView = view.LayoutChildren[i];
                if (childView == null || args.SkipInactive && !childView.IsActive)
                    continue;

                var skipChild = false;

                if (args.Parent != null)
                {
                    if (childView.Parent != args.Parent)
                        skipChild = true;
                }

                if (!skipChild)
                {
                    var component = childView as T;
                    if (component != null)
                    {
                        var result = predicate(component);
                        if (!result)
                        {
                            // done traversing
                            if (args.StopOnFalsePredicate)
                                return;

                            continue;
                        }
                    }
                }

                if (args.IsRecursive)
                {
                    DoUntilDepth(childView, predicate, args);
                }
            }
        }

        private static void DoUntilBreadth<T>(View view, Predicate<T> predicate, ViewSearchArgs args) where T : View
        {
            var count = view.LayoutChildren.Count;
            var children = GetViewBuffer(count);
            for (var i=0; i < count; i++)
            {
                var childView = view.LayoutChildren[i];
                if (childView == null || args.SkipInactive && !childView.IsActive)
                    continue;

                var skipChild = false;

                if (args.Parent != null)
                {
                    if (childView.Parent != args.Parent.gameObject)
                        skipChild = true;
                }

                if (!skipChild)
                {
                    var component = childView as T;
                    if (component != null)
                    {
                        var result = predicate(component);
                        if (!result)
                        {
                            // done traversing
                            if (args.StopOnFalsePredicate)
                            {
                                RecycleViewBuffer(children);
                                return;
                            }

                            continue;
                        }
                    }
                }

                if (args.IsRecursive)
                {
                    // add children to queue
                    children[i] = childView;
                }
            }

            for (var i=0; i < count; i++)
            {
                var child = children[i];
                if (child == null)
                    continue;

                DoUntilBreadth(child, predicate, args);
            }

            RecycleViewBuffer(children);
        }

        private static void DoUntilReverseDepth<T>(View view, Predicate<T> predicate, ViewSearchArgs args)
                                                   where T : View
        {
            var count = view.LayoutChildren.Count;
            for (var i=0; i < count; i++)
            {
                var childView = view.LayoutChildren[i];
                if (childView == null || args.SkipInactive && !childView.IsActive)
                    continue;

                if (args.IsRecursive)
                {
                    DoUntilReverseDepth(childView, predicate, args);
                }

                if (args.Parent != null)
                {
                    if (childView.Parent != args.Parent.gameObject)
                        continue;
                }

                var component = childView as T;
                if (component != null)
                {
                    var result = predicate(component);
                    if (!result)
                    {
                        // done traversing
                        if (args.StopOnFalsePredicate)
                            return;
                    }
                }
            }
        }

        private static void DoUntilReverseBreadth<T>(View view, Predicate<T> predicate, ViewSearchArgs args)
                                                     where T : View
        {
            var count = view.LayoutChildren.Count;
            var componentStack = GetViewBuffer(count);
            var childStack = GetViewBuffer(count);

            for (var i = 0; i < count; i++)
            {
                var childView = view.LayoutChildren[i];
                if (childView == null || args.SkipInactive && !childView.IsActive)
                    continue;

                if (args.IsRecursive)
                {
                    childStack[i] = childView;
                }

                if (args.Parent != null)
                {
                    if (childView.Parent != args.Parent.gameObject)
                        continue;
                }

                var component = childView as T;
                if (component != null)
                {
                    componentStack[i] = component;
                }
            }

            for (var i = 0; i < count; i++)
            {
                var child = childStack[i];
                if (child == null)
                    continue;

                DoUntilReverseBreadth(child, predicate, args);
            }

            for (var i = 0; i < count; i++)
            {
                var component = componentStack[i] as T;
                if (component == null)
                    continue;

                var result = predicate(component);
                if (!result)
                {
                    // done traversing
                    if (args.StopOnFalsePredicate)
                        return;
                }
            }

            RecycleViewBuffer(componentStack);
            RecycleViewBuffer(childStack);
        }

        private static View[] GetViewBuffer(int elementCount)
        {
            if (elementCount > PooledViewBufferSize)
                return new View[elementCount];

            if (DirtyViewArrayPool.Count > 0)
            {
                var index = DirtyViewArrayPool.Count - 1;
                var result = DirtyViewArrayPool[index];
                DirtyViewArrayPool.RemoveAt(index);
                return result;
            }
            if (ViewArrayPool.Count > 0)
            {
                var index = ViewArrayPool.Count - 1;
                var result = ViewArrayPool[index];
                ViewArrayPool.RemoveAt(index);
                return result;
            }

            return new View[PooledViewBufferSize];
        }

        private static void RecycleViewBuffer(View[] buffer)
        {
            if (buffer.Length != PooledViewBufferSize)
                return;

            DirtyViewArrayPool.Add(buffer);
        }

        private static void CleanViewBuffers()
        {
            for (var i = DirtyViewArrayPool.Count - 1; i >= 0; i--)
            {
                var buffer = DirtyViewArrayPool[i];
                Array.Clear(buffer, 0, buffer.Length);
                DirtyViewArrayPool.RemoveAt(i);
                ViewArrayPool.Add(buffer);
            }
        }

        #endregion
    }
}