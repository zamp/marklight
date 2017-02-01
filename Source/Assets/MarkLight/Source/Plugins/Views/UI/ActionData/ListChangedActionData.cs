#region Using Statements
using MarkLight.Views.UI;
using System;
using UnityEngine.EventSystems;
#endregion

namespace MarkLight.Views.UI
{
    /// <summary>
    /// Items changed action data.
    /// </summary>
    public class ListChangedActionData : ActionData
    {
        #region Fields

        public readonly ListChangeAction ListChangeAction;
        public readonly int StartIndex;
        public readonly int EndIndex;
        public readonly string FieldPath;

        #endregion

        #region Constructor

        public ListChangedActionData(ListChangeAction action,
                                     int startIndex = 0, int endIndex = 0, string fieldPath = null) {
            ListChangeAction = action;
            StartIndex = startIndex;
            EndIndex = endIndex;
            FieldPath = fieldPath;
        }

        #endregion
    }
}
