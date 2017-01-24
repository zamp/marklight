using System;
using MarkLight;
using UnityEngine;

namespace Marklight.Themes
{
    [Serializable]
    public struct StyleProperty
    {

        #region Private Fields

        [SerializeField]
        private string _name;

        [SerializeField]
        private string _value;

        #endregion

        #region Constructor

        public StyleProperty(string name, string value)
        {
            _name = name;
            _value = value;
        }

        #endregion

        #region Methods

        public void ApplyTo(View view, ValueConverterContext context)
        {
            if (context == null)
                context = new ValueConverterContext();

            // check if value contains a binding
            if (ViewFieldBinding.ValueHasBindings(_value))
            {
                view.AddBinding(_name, _value);
                return;
            }

            // check if the field value is allowed to be be set from xuml
            var viewTypeData = view.ViewTypeData;
            var isNotAllowed = viewTypeData.FieldsNotSetFromXuml.Contains(_name);
            if (isNotAllowed)
            {
                Debug.LogError(string.Format("[MarkLight] {0}: Unable to assign value \"{1}\" to view "+
                                             "field \"{2}.{3}\". Field not allowed to be set from XUML.",
                                             view.GameObjectName, _value, view.ViewXumlName, _name));
                return;
            }

            // check if we are setting a state-value
            var stateIndex = _name.IndexOf('-', 0);
            if (stateIndex > 0)
            {
                // check if we are setting a sub-state, i.e. the state of the target view
                var stateViewField = _name.Substring(stateIndex + 1);
                var state = _name.Substring(0, stateIndex);

                var isSubState = stateViewField.StartsWith("-");
                if (isSubState)
                {
                    stateViewField = stateViewField.Substring(1);
                }

                // setting the state of the source view
                view.AddStateValue(state, stateViewField, _value, context, isSubState);
                return;
            }

            // get view field data
            var viewFieldData = view.GetViewFieldData(_name);
            if (viewFieldData == null)
                return; // ignore if view does not have field

            // check if we are setting a view action handler
            if (viewFieldData.ViewFieldTypeName == "ViewAction")
            {
                viewFieldData.SourceView.AddViewActionEntry(viewFieldData.ViewFieldPath, _value, view.Parent);
            }
            else
            {
                // we are setting a normal view field
                view.SetValue(_name, _value, true, null, context, true, true);
            }
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is StyleProperty))
                return false;

            var other = (StyleProperty) obj;
            return other._name == _name;
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return _name; }
        }

        public string Value
        {
            get { return _value; }
        }

        #endregion
    }
}