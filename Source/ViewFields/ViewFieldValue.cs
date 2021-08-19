using System.Collections.Generic;

namespace MarkLight
{
    /// <summary>
    /// Field field value setter context.
    /// </summary>
    public struct ViewFieldValue
    {
        #region Fields

        /// <summary>
        /// The view field value.
        /// </summary>
        public readonly object Value;

        /// <summary>
        /// The view field call stack.
        /// </summary>
        public readonly HashSet<ViewFieldData> Callstack;

        /// <summary>
        /// True if the default state should be updated with value.
        /// </summary>
        public bool UpdateDefaultState;

        /// <summary>
        /// True if view field observers should be notified of change.
        /// </summary>
        public bool NotifyObservers;

        /// <summary>
        /// True if errors while assigning value to view field should be suppreessed. Used for applying theme settings.
        /// </summary>
        public bool SuppressAssignErrors;

        /// <summary>
        /// The value converter context.
        /// </summary>
        public ValueConverterContext ConverterContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="callstack">View field call stack.</param>
        public ViewFieldValue(object value, HashSet<ViewFieldData> callstack = null) : this() {
            UpdateDefaultState = true;
            NotifyObservers = true;
            Value = value;
            Callstack = callstack ?? new HashSet<ViewFieldData>();
        }

        #endregion
    }
}