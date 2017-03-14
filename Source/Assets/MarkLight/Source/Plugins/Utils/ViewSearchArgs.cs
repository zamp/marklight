using System;

namespace MarkLight
{
    /// <summary>
    /// Argument object for use when performing View searches via ViewSearchExtension methods.
    /// </summary>
    public class ViewSearchArgs
    {
        #region Fields

        /// <summary>
        /// Depth first traversal args.
        /// </summary>
        public static readonly ViewSearchArgs DepthFirst =
            new ViewSearchArgs { TraversalAlgorithm = TraversalAlgorithm.DepthFirst }.MakeReadonly();

        /// <summary>
        /// Breadth first traversal args.
        /// </summary>
        public static readonly ViewSearchArgs BreadthFirst =
            new ViewSearchArgs { TraversalAlgorithm = TraversalAlgorithm.BreadthFirst }.MakeReadonly();

        /// <summary>
        /// Reverse depth first traversal args.
        /// </summary>
        public static readonly ViewSearchArgs ReverseDepthFirst =
            new ViewSearchArgs { TraversalAlgorithm = TraversalAlgorithm.ReverseDepthFirst }.MakeReadonly();

        /// <summary>
        /// Reverse breadth first traversal args.
        /// </summary>
        public static readonly ViewSearchArgs ReverseBreadthFirst =
            new ViewSearchArgs { TraversalAlgorithm = TraversalAlgorithm.ReverseBreadthFirst }.MakeReadonly();

        /// <summary>
        /// No recursion (first level only) args.
        /// </summary>
        public static readonly ViewSearchArgs NonRecursive =
            new ViewSearchArgs { IsRecursive = false }.MakeReadonly();

        /// <summary>
        /// False predicate in traversal causes a child branch to be ignored (non reversed traversal), but
        /// traversal continues.
        /// </summary>
        public static readonly ViewSearchArgs ContinueOnFalsePredicate =
            new ViewSearchArgs { StopOnFalsePredicate = false };

        private bool _isReadonly;
        private TraversalAlgorithm _traversalAlgorithm = TraversalAlgorithm.DepthFirst;
        private bool _skipInactive;
        private View _parent;
        private bool _isRecursive = true;
        private bool _stopOnFalsePredicate = true;

        #endregion

        #region Constructors

        public ViewSearchArgs()
        {
        }

        public ViewSearchArgs(ViewSearchArgs args)
        {
            if (args == null)
                return;

            _isRecursive = args.IsRecursive;
            _parent = args.Parent;
            _skipInactive = args.SkipInactive;
            TraversalAlgorithm = args.TraversalAlgorithm;
        }

        #endregion

        #region Methods

        public ViewSearchArgs MakeReadonly() {
            _isReadonly = true;
            return this;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determine if the search is recursive. Default is true.
        /// </summary>
        public bool IsRecursive
        {
            get { return _isRecursive; }
            set
            {
                if (_isReadonly)
                {
                    throw new InvalidOperationException(
                        "The ViewSearchArgs cannot be modified because it is readonly.");
                }

                _isRecursive = value;
            }
        }

        /// <summary>
        /// When set, specifies that views should have the specified parent.
        /// </summary>
        public View Parent
        {
            get { return _parent; }
            set
            {
                if (_isReadonly)
                {
                    throw new InvalidOperationException(
                        "The ViewSearchArgs cannot be modified because it is readonly.");
                }

                _parent = value;
            }
        }

        /// <summary>
        /// Determine if Inactive views should be skipped. Default is false.
        /// </summary>
        public bool SkipInactive
        {
            get { return _skipInactive; }
            set
            {
                if (_isReadonly)
                {
                    throw new InvalidOperationException(
                        "The ViewSearchArgs cannot be modified because it is readonly.");
                }

                _skipInactive = value;
            }
        }

        /// <summary>
        /// Stop traversing when predicate returns false. Default is true.
        /// </summary>
        /// <d>False causes branch to be skipped on
        /// false predicate for non-reversed traversal. Reversed traversal simply ignores predicate result.</d>
        public bool StopOnFalsePredicate
        {
            get { return _stopOnFalsePredicate; }
            set
            {
                if (_isReadonly)
                {
                    throw new InvalidOperationException(
                        "The ViewSearchArgs cannot be modified because it is readonly.");
                }

                _stopOnFalsePredicate = value;
            }
        }

        /// <summary>
        /// The type of recursive traversal.
        /// </summary>
        public TraversalAlgorithm TraversalAlgorithm
        {
            get { return _traversalAlgorithm; }
            set
            {
                if (_isReadonly)
                {
                    throw new InvalidOperationException(
                        "The ViewSearchArgs cannot be modified because it is readonly.");
                }

                _traversalAlgorithm = value;
            }
        }

        #endregion
    }
}