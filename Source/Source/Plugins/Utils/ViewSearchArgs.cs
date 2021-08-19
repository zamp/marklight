using System;

namespace MarkLight
{
    /// <summary>
    /// Argument object for use when performing View searches via ViewSearchExtension methods.
    /// </summary>
    public struct ViewSearchArgs
    {
        #region Constants

        private const byte ReadonlyFlag = 1 << 0;
        private const byte SkipInactiveFlag = 1 << 1;
        private const byte IsRecursiveFlag = 1 << 2;
        private const byte StopOnFalsePredicateFlag = 1 << 3;

        #endregion

        #region Fields

        /// <summary>
        /// Default search arguments.
        /// </summary>
        public static readonly ViewSearchArgs Default = new ViewSearchArgs(true).MakeReadonly();

        /// <summary>
        /// Depth first traversal args.
        /// </summary>
        public static readonly ViewSearchArgs DepthFirst = new ViewSearchArgs(true).MakeReadonly();

        /// <summary>
        /// Breadth first traversal args.
        /// </summary>
        public static readonly ViewSearchArgs BreadthFirst =
            new ViewSearchArgs(true, true, TraversalAlgorithm.BreadthFirst).MakeReadonly();

        /// <summary>
        /// Reverse depth first traversal args.
        /// </summary>
        public static readonly ViewSearchArgs ReverseDepthFirst =
            new ViewSearchArgs(true, true, TraversalAlgorithm.ReverseDepthFirst).MakeReadonly();

        /// <summary>
        /// Reverse breadth first traversal args.
        /// </summary>
        public static readonly ViewSearchArgs ReverseBreadthFirst =
            new ViewSearchArgs(true, true, TraversalAlgorithm.ReverseBreadthFirst).MakeReadonly();

        /// <summary>
        /// No recursion (first level only) args.
        /// </summary>
        public static readonly ViewSearchArgs NonRecursive =
            new ViewSearchArgs(true) { IsRecursive = false }.MakeReadonly();

        /// <summary>
        /// False predicate in traversal causes a child branch to be ignored (non reversed traversal), but
        /// traversal continues.
        /// </summary>
        public static readonly ViewSearchArgs ContinueOnFalsePredicate =
            new ViewSearchArgs(true) { StopOnFalsePredicate = false };

        private View _parent;
        private TraversalAlgorithm _traversalAlgorithm;
        private byte _flags;

        #endregion

        #region Constructors

        public ViewSearchArgs(bool isRecursive = true, bool stopOnFalsePredicate = true,
                              TraversalAlgorithm traversalAlgorithm = TraversalAlgorithm.DepthFirst) : this()
        {
            if (isRecursive)
                _flags |= IsRecursiveFlag;

            if (stopOnFalsePredicate)
                _flags |= StopOnFalsePredicateFlag;

            _traversalAlgorithm = traversalAlgorithm;
        }

        public ViewSearchArgs(ViewSearchArgs args) : this()
        {
            IsRecursive = args.IsRecursive;
            Parent = args.Parent;
            SkipInactive = args.SkipInactive;
            TraversalAlgorithm = args.TraversalAlgorithm;
        }

        #endregion

        #region Methods

        public ViewSearchArgs MakeReadonly()
        {
            _flags |= ReadonlyFlag;
            return this;
        }

        #endregion

        #region Properties

        public bool IsReadonly
        {
            get { return (_flags & ReadonlyFlag) == ReadonlyFlag; }
        }

        /// <summary>
        /// Determine if the search is recursive. Default is true.
        /// </summary>
        public bool IsRecursive
        {
            get { return (_flags & IsRecursiveFlag) == IsRecursiveFlag; }
            set
            {
                if (IsReadonly)
                {
                    throw new InvalidOperationException(
                        "The ViewSearchArgs cannot be modified because it is readonly.");
                }

                SetFlag(IsRecursiveFlag, value);
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
                if (IsReadonly)
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
            get { return (_flags & SkipInactiveFlag) == SkipInactiveFlag; }
            set
            {
                if (IsReadonly)
                {
                    throw new InvalidOperationException(
                        "The ViewSearchArgs cannot be modified because it is readonly.");
                }

                SetFlag(SkipInactiveFlag, value);
            }
        }

        /// <summary>
        /// Stop traversing when predicate returns false. Default is true.
        /// </summary>
        /// <d>False causes branch to be skipped on
        /// false predicate for non-reversed traversal. Reversed traversal simply ignores predicate result.</d>
        public bool StopOnFalsePredicate
        {
            get { return (_flags & StopOnFalsePredicateFlag) == StopOnFalsePredicateFlag; }
            set
            {
                if (IsReadonly)
                {
                    throw new InvalidOperationException(
                        "The ViewSearchArgs cannot be modified because it is readonly.");
                }

                SetFlag(StopOnFalsePredicateFlag, value);
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
                if (IsReadonly)
                {
                    throw new InvalidOperationException(
                        "The ViewSearchArgs cannot be modified because it is readonly.");
                }

                _traversalAlgorithm = value;
            }
        }

        private void SetFlag(byte flag, bool isSet)
        {
            if (isSet)
            {
                _flags |= flag;
            }
            else
            {
                _flags = (byte)(_flags & ~flag);
            }
        }

        #endregion
    }
}