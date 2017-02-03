using System;
using System.Collections.Generic;
using System.Linq;
using MarkLight.Views;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Manages View field states.
    /// </summary>
    [Serializable]
    public class ViewFieldStates
    {
        #region Fields

        [NonSerialized]
        public readonly View OwnerView;

        [SerializeField]
        private List<ViewFieldStateValue> _stateValuesList;

        private StateAnimation _stateAnimation;
        private Dictionary<string, Dictionary<string, ViewFieldStateValue>> _stateValuesDictionary;
        private Dictionary<string, Dictionary<string, StateAnimation>> _stateAnimations;
        private bool _isDefaultState;
        private string _previousState;

        // local caching
        private Dictionary<string, ViewFieldStateValue> _latestStates;
        private string _latestStatesName;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ViewFieldStates(View view)
        {
            OwnerView = view;

            _stateValuesList = new List<ViewFieldStateValue>(10);
            _stateValuesDictionary = new Dictionary<string, Dictionary<string, ViewFieldStateValue>>();
            _stateAnimations = new Dictionary<string, Dictionary<string, StateAnimation>>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the value of a field for a specified state.
        /// </summary>
        public ViewFieldStateValue Get(string stateName, string fieldPath)
        {
            if (_latestStatesName == stateName)
                return _latestStates.Get(fieldPath);

            _latestStates = null;
            _latestStatesName = null;

            var states = _stateValuesDictionary.Get(stateName);
            if (states == null)
                return null;

            _latestStates = states;
            _latestStatesName = stateName;

            return states.Get(fieldPath);
        }

        /// <summary>
        /// Adds field field state value.
        /// </summary>
        public void AddValue(string stateName, string fieldPath, string value,
                             ValueConverterContext context, bool isSubState)
        {
            // convert value to ensure that assets that can only be loaded in the editor is cached by
            // the view-presenter, the caching is done by the value converter
            var viewFieldData = OwnerView.Fields.GetData(fieldPath);
            if (viewFieldData == null)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to set state value \"{1}-{2}\". View field \"{2}\" not found.",
                    OwnerView.GameObjectName, stateName, fieldPath));

                return;
            }

            if (viewFieldData.ValueConverter != null)
            {
                viewFieldData.ValueConverter.Convert(value, context != null ? context : ValueConverterContext.Default);
            }

            // set state value
            if (isSubState)
            {
                // only go down one level
                var subViewFieldData = OwnerView.Fields.GetData(fieldPath, 1);
                subViewFieldData.SourceView.States.AddValue(stateName, subViewFieldData.Path, value, context, false);
            }
            else
            {
                // get mapped view-field path
                var viewTypeData = ViewData.GetViewTypeData(OwnerView.ViewTypeName);
                var mappedPath = viewTypeData.GetMappedFieldPath(fieldPath);

                // overwrite state value if it exist otherwise create a new one
                var stateValue = _stateValuesList.FirstOrDefault(x => x.State == stateName && x.Path == mappedPath);
                if (stateValue != null)
                {
                    stateValue.SetValue(value, context);
                }
                else
                {
                    _stateValuesList.Add(new ViewFieldStateValue(mappedPath, stateName, value, context, true));

                    // for every state value we need to store the default value so we can revert back to it
                    var defaultStateValue =
                        _stateValuesList.FirstOrDefault(
                            x => x.State == View.DefaultStateName && x.Path == mappedPath);

                    if (defaultStateValue == null)
                    {
                        _stateValuesList.Add(
                            new ViewFieldStateValue(mappedPath, View.DefaultStateName, null,
                                ValueConverterContext.Empty, false));
                    }
                }
            }
        }

        /// <summary>
        /// Called when view state has been changed.
        /// </summary>
        internal void NotifyStateChanged()
        {
            // stop previous animation if active and get new state animation
            if (_stateAnimation != null && !_stateAnimation.IsAnimationCompleted)
            {
                _stateAnimation.StopAnimation();
            }
            _stateAnimation = GetAnimation(_previousState, OwnerView.State);

            // if we are changing from default state we need to make sure all default state values has been set
            if (_isDefaultState)
            {
                var defaultStateValues = _stateValuesDictionary.Get(View.DefaultStateName);
                if (defaultStateValues != null) // no state values set nothing to do
                {
                    foreach (var stateValue in defaultStateValues.Values)
                    {
                        if (stateValue.IsDefaultValueSet)
                            continue;

                        // get view field data
                        var viewFieldData = OwnerView.Fields.GetData(stateValue.Path);
                        if (viewFieldData == null)
                        {
                            Debug.LogError(String.Format(
                                "[MarkLight] {0}: Unable to assign default state value to view field \"{1}\". "+
                                "View field not found.",
                                OwnerView.GameObjectName, stateValue.Path));
                        }
                        else
                        {
                            // set default value
                            var value = OwnerView.Fields.GetValue(stateValue.Path);
                            if (viewFieldData.ValueConverter != null)
                            {
                                stateValue.SetDefaultValue(value, viewFieldData.ValueConverter.ConvertToString(value));
                            }
                        }
                    }
                }
            }

            _isDefaultState = OwnerView.State == View.DefaultStateName;
            _previousState = OwnerView.State;

            // get state values
            var currentVals = _stateValuesDictionary.Get(OwnerView.State);
            if (currentVals == null)
                return; // no state values set, nothing to do

            // go through state values and update view values
            foreach (var stateValue in currentVals.Values)
            {
                if (_stateAnimation != null)
                {
                    // is this view field animated?
                    var animators = _stateAnimation.GetFieldAnimators(stateValue.Path);
                    if (animators != null)
                    {
                        var animateTo = stateValue.Value;

                        // yes. set target value of animations
                        foreach (var animator in animators)
                        {
                            var animateToStr = animateTo as string;
                            if (animateToStr != null)
                            {
                                animator.ToStringValue = animateToStr;
                            }
                            else
                            {
                                animator.To = animateTo;
                            }

                            animator.UpdateViewFieldAnimator();
                        }

                        continue;
                    }
                }

                // set view value to state value
                var value = OwnerView.Fields.SetValue(stateValue.Path, stateValue.Value,
                                            false, null, stateValue.ConverterContext, true);

                stateValue.SetCachedValue(value); // store converted value to save conversion time
            }

            if (_stateAnimation != null)
            {
                // start state animation
                _stateAnimation.StartAnimation();
            }
        }

        /// <summary>
        /// Adds a state animation to the view.
        /// </summary>
        internal void AddAnimation(StateAnimation animation)
        {
            if (String.IsNullOrEmpty(animation.From))
                return;

            if (!_stateAnimations.ContainsKey(animation.From))
            {
                _stateAnimations.Add(animation.From, new Dictionary<string, StateAnimation>());
            }

            _stateAnimations[animation.From].Add(animation.To, animation);
        }

        /// <summary>
        /// Gets state animation.
        /// </summary>
        internal StateAnimation GetAnimation(string from, string to)
        {
            // check if animation exist between states From -> To
            var fromAnimation = _stateAnimations.Get(from);
            if (fromAnimation != null)
            {
                var toAnimation = fromAnimation.Get(to);
                if (toAnimation != null)
                    return toAnimation;
            }

            // check if animation exist between states Any -> To
            fromAnimation = _stateAnimations.Get(View.AnyStateName);
            return fromAnimation == null
                ? null
                : fromAnimation.Get(to);
        }

        /// <summary>
        /// Initializes internal values to default values. Called once before InitializeInternal().
        /// Called in depth-first order.
        /// </summary>
        internal void InitializeInternalDefaultValues()
        {
            OwnerView.Fields.InitializeInternalDefaultValues();
            // initialize lists and dictionaries
            _stateValuesDictionary = new Dictionary<string, Dictionary<string, ViewFieldStateValue>>();
            _stateAnimations = new Dictionary<string, Dictionary<string, StateAnimation>>();
            _previousState = OwnerView.State;
            _isDefaultState = OwnerView.State == View.DefaultStateName;
        }

        /// <summary>
        /// Initializes the field manager internally.
        /// Called once before Initialize(). Called in depth-first order.
        /// </summary>
        internal void InitializeInternal()
        {
            // initialize state values
            foreach (var stateValue in _stateValuesList)
            {
                SetStateValue(stateValue);
            }
        }

        /// <summary>
        /// Sets state value.
        /// </summary>
        private void SetStateValue(ViewFieldStateValue stateValue)
        {
            if (!_stateValuesDictionary.ContainsKey(stateValue.State))
            {
                _stateValuesDictionary.Add(stateValue.State, new Dictionary<string, ViewFieldStateValue>());
            }

            var currentVals = _stateValuesDictionary[stateValue.State];
            if (!currentVals.ContainsKey(stateValue.Path))
            {
                currentVals.Add(stateValue.Path, stateValue);
            }
            else
            {
                currentVals[stateValue.Path] = stateValue;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determine if the current view state is the default state.
        /// </summary>
        public bool IsDefaultState
        {
            get { return _isDefaultState; }
        }

        #endregion
    }
}