﻿//#define DISABLE_INIT_TRYCATCH // uncomment if you don't want exceptions to be caught during initialization of views
#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using Marklight;
using MarkLight.Views.UI;
#endregion

namespace MarkLight
{
    /// <summary>
    /// Base class for view models.
    /// </summary>
    /// <d>Base class for all view models in the framework. All view models must be a subclass of this class to
    /// be processed and managed by the framework. </d>
    public class View : MonoBehaviour, IEnumerable<View>
    {
        #region Constants

        public const string DefaultStateName = "Default";
        public const string AnyStateName = "Any";

        #endregion

        #region Fields

        /// <summary>
        /// The ID of the view. 
        /// </summary>
        /// <d>Specifies a unique ID for the view. Used to map the view to reference fields on the parent view
        /// model. Provides a way to reference the view in data bindings. Is used as selectors in styles.</d>
        [ChangeHandler("IdChanged")]
        public string Id;

        /// <summary>
        /// The style of the view.
        /// </summary>
        /// <d>Used as selector by the styles. Specifies the name of the style that is to be applied to the view
        /// and any children that explicitly inherits its style. The style is applied when the view is created
        /// (usually in the editor as the XUML is processed).</d>
        public string Style;

        /// <summary>
        /// Based on style.
        /// </summary>
        /// <d>Used in style definition to specify which style it's based on.</d>
        public string BasedOn;

        /// <summary>
        /// The theme of the view.
        /// </summary>
        /// <d>Specifies the name of the theme that is applied to the view and its children. The theme determines
        ///  which set of styles are to be considered when applying matching styles to the view.</d>
        public string Theme;

        /// <summary>
        /// Base directory.
        /// </summary>
        /// <d>Specifies the base directory to be used by the view and its children. The base directory is used when
        /// loading resources such as sprites, fonts, etc.</d>
        public string BaseDirectory;

        /// <summary>
        /// Unit size.
        /// </summary>
        /// <d>Specifies the user-defined unit size to be used by the view and its children. Used when element size
        ///  is specified in user-defined units to convert it into pixels.</d>
        public Vector3 UnitSize;

        /// <summary>
        /// Color hex type.
        /// </summary>
        /// <d>Specifies the component order to expect color hexidecimals to be provided in.</d>
        public HexColorType HexColorType;

        /// <summary>
        /// Layout parent view.
        /// </summary>
        /// <d>The layout parent view is the direct ascendant of the current view in the scene object hierarchy.</d>
        [NotSetFromXuml]
        public View LayoutParent;

        /// <summary>
        /// Layout child views.
        /// </summary>
        /// <d>The layout child views are direct descendants of the current view in the scene object hierarchy.</d>
        [NotSetFromXuml]
        public List<View> LayoutChildren;

        /// <summary>
        /// Parent view.
        /// </summary>
        /// <d>The parent of the view is the logical parent to which this view belongs. In the XUML any view you
        ///  can see has the current view as its logical parent.</d>
        [NotSetFromXuml]
        public View Parent;

        /// <summary>
        /// Content view.        
        /// </summary>
        /// <d>View that is the parent to the content of this view. Usually it is the current view itself but
        /// when a ContentPlaceholder is used the Content points to the view that contains the ContentPlaceholder.</d>
        [NotSetFromXuml]
        public View Content;

        /// <summary>
        /// View state.
        /// </summary>
        /// <d>View state name. Determines state values to be applied to the view. All views start out in the
        /// "Default" state and when the state changes the values associated with that state are applied to
        /// the view.</d>
        [ChangeHandler("StateChanged", TriggerImmediately = true)]
        [NotSetFromXuml]
        public _string State;

        /// <summary>
        /// Indicates if the view is enabled.
        /// </summary>
        /// <d>Activates/deactivates the view. If set to false in this or in any parent view, all components are
        /// disabled, attached renderers are turned off, etc. Any components attached will no longer have
        /// Update() called.</d>
        [ChangeHandler("IsActiveChanged", TriggerImmediately = true)]
        public _bool IsActive;

        #region GameObject

        /// <summary>
        /// Hide flags for the game object.
        /// </summary>
        /// <d>Bit mask that controls object destruction, saving and visibility in editor.</d>
        [MapTo("GameObject.hideFlags")]
        public _HideFlags HideFlags;

        /// <summary>
        /// GameObject the view is attached to.
        /// </summary>
        /// <d>GameObject that the view is attached to.</d>
        public GameObject GameObject;

        #endregion

        #region Transform

        /// <summary>
        /// Position, rotation and scale of the view.
        /// </summary>
        /// <d>The view transform is used to manipulate the position, rotation and scale of the view in relation
        /// to the layout parent view's transform or in world space. The transform is sometimes manipulated
        /// indirectly through other view fields and through the view model's internal layout logic.</d>
        public Transform Transform;

        /// <summary>
        /// Position of the view.
        /// </summary>
        /// <d>The local position of the view in relation to the layout parent view transform.</d>
        [MapTo("Transform.localPosition")]
        public _Vector3 Position;

        /// <summary>
        /// Rotation of the view.
        /// </summary>
        /// <d>The local rotation of the view in relation to the layout parent view transform. Stored as a
        /// Quaternion but specified in XUML as euler angles.</d>
        [MapTo("Transform.localRotation")]
        public _Quaternion Rotation;

        /// <summary>
        /// Scale of the view.
        /// </summary>
        /// <d>The scale of the view in relation to the layout parent view transform.</d>
        [MapTo("Transform.localScale")]
        public _Vector3 Scale;

        #endregion

        #region View Actions

        /// <summary>
        /// Cancel view action.
        /// </summary>
        /// <d>Triggered by the EventSystem when a cancel event occurs.</d>
        public ViewAction Cancel;

        /// <summary>
        /// Deselect view action.
        /// </summary>
        /// <d>Triggered by the EventSystem when another view is selected.</d>
        public ViewAction Deselect;

        /// <summary>
        /// Drop view action.
        /// </summary>
        /// <d>Triggered by the EventSystem when the view accepts a drop.</d>
        public ViewAction Drop;

        /// <summary>
        /// Move view action.
        /// </summary>
        /// <d>Triggered by the EventSystem when a move event occurs.</d>
        public ViewAction Move;

        /// <summary>
        /// Click view action.
        /// </summary>
        /// <d>Triggered by the EventSystem when the user clicks/touches on the view.</d>
        public ViewAction Click;

        /// <summary>
        /// Drag view action.
        /// </summary>
        /// <d>Triggered by the EventSystem when the user presses mouse on and starts to drag over the view.</d>
        public ViewAction BeginDrag;

        /// <summary>
        /// End drag view action.
        /// </summary>
        /// <d>Triggered by the EventSystem when the user stops dragging mouse over the view.</d>
        public ViewAction EndDrag;

        /// <summary>
        /// Drag view action.
        /// </summary>
        /// <d>Triggered by the EventSystem as the user drags the mouse over the view.</d>
        public ViewAction Drag;

        /// <summary>
        /// Initialize potential drag view action.
        /// </summary>
        /// <d>Triggered by the EventSystem as the user initiates a potential drag over the view.</d>
        public ViewAction InitializePotentialDrag;

        /// <summary>
        /// Mouse down view action.
        /// </summary>
        /// <d>Triggered by the EventSystem when the mouse/touch presses down over the view.</d>
        public ViewAction MouseDown;

        /// <summary>
        /// Mouse enter view action.
        /// </summary>
        /// <d>Triggered by the EventSystem when the mouse enters the view.</d>
        public ViewAction MouseEnter;

        /// <summary>
        /// Mouse exit view action.
        /// </summary>
        /// <d>Triggered by the EventSystem when the mouse exits the view.</d>
        public ViewAction MouseExit;

        /// <summary>
        /// Mouse up view action.
        /// </summary>
        /// <d>Triggered by the EventSystem when the mouse/thouch releases over the view.</d>
        public ViewAction MouseUp;

        /// <summary>
        /// Scroll view action.
        /// </summary>
        /// <d>Triggered by the EventSystem when the user scrolls when the view is selected.</d>
        public ViewAction Scroll;

        /// <summary>
        /// Select view action.
        /// </summary>
        /// <d>Triggered by the EventSystem when the view is selected.</d>
        public ViewAction Select;

        /// <summary>
        /// Submit view action.
        /// </summary>
        /// <d>Triggered by the EventSystem when the user submits while view is selected.</d>
        public ViewAction Submit;

        /// <summary>
        /// Update selected view action.
        /// </summary>
        /// <d>Triggered by the EventSystem when the object associated with this EventTrigger is updated.</d>
        public ViewAction UpdateSelected;

        #endregion

        /// <summary>
        /// Indicates if this view is to be used as a template.
        /// </summary>
        /// <d>A template view is used to create dynamic instances of the view. Used by certain views such as the
        /// List and TabPanel.</d>
        public _bool IsTemplate;

        /// <summary>
        /// Activated view action.
        /// </summary>
        /// <d>Triggered every time the view is activated. Also triggered once the view is intialized if it starts
        /// out activated.</d>
        public ViewAction Activated;

        /// <summary>
        /// Deactivated view action.
        /// </summary>
        /// <d>Triggered every time the view is deactivated. Also triggered once the view is intialized if it starts
        /// out deactivated.</d>
        public ViewAction Deactivated;

        /// <summary>
        /// Indicates if the view has been destroyed by GameObject.Destroy().
        /// </summary>
        [NotSetFromXuml]
        public _bool IsDestroyed;

        /// <summary>
        /// Indicates if the view has been created dynamically. 
        /// </summary>
        [NotSetFromXuml]
        public _bool IsDynamic;

        private readonly string _viewTypeName;
        private ViewTypeData _viewTypeData;
        private List<ViewAction> _eventSystemViewActions;
        private HashSet<string> _changeHandlers;
        private Dictionary<string, MethodInfo> _changeHandlerMethods;
        private bool _isInitialized;
        private bool _isLayoutCalculating;
        private bool _isLayoutChanged;
        private LayoutCalculator _layoutCalculator;

        [SerializeField]
        private string _viewXumlName;

        [SerializeField]
        private List<ViewActionEntry> _viewActionEntries;

        [SerializeField]
        private ViewBindings _bindings;

        [SerializeField]
        private ViewFields _fields;

        [SerializeField]
        private ViewFieldStates _states;

#if UNITY_4_6 || UNITY_5_0
        private bool _eventSystemTriggersInitialized;
#endif

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public View()
        {
            _viewTypeName = GetType().Name;
            _viewActionEntries = new List<ViewActionEntry>();

            // initalize private data (also done in InitializeInternalDefaultValues because of being set
            // to null during deserialization)
            _bindings = new ViewBindings(this);
            _fields = new ViewFields(this);
            _states = new ViewFieldStates(this);
            _eventSystemViewActions = new List<ViewAction>();
            _changeHandlers = new HashSet<string>();
            _changeHandlerMethods = new Dictionary<string, MethodInfo>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called once at the end of a frame. Triggers queued change handlers.
        /// </summary>
        public virtual void LateUpdate()
        {
            TriggerChangeHandlers();

#if UNITY_4_6 || UNITY_5_0
            if (!_eventSystemTriggersInitialized)
            {
                InitEventSystemTriggers();
            }
#endif
        }

        /// <summary>
        /// Activates the view.
        /// </summary>
        public virtual void Activate()
        {
            IsActive.Value = true;
        }

        /// <summary>
        /// Activates the view and sends data to it.
        /// </summary>
        public virtual void Activate(object data)
        {
            IsActive.DirectValue = true;
            gameObject.SetActive(true);
            Activated.Trigger(data);
        }

        /// <summary>
        /// Deactivates the view.
        /// </summary>
        public void Deactivate()
        {
            IsActive.Value = false;
        }

        /// <summary>
        /// Changes the state of the view.
        /// </summary>
        public virtual void SetState(string stateName)
        {
            State.Value = stateName;
        }

        /// <summary>
        /// Moves the view to another view.
        /// </summary>
        public void MoveTo(View target, int childIndex = -1, bool updateLayoutParent = true)
        {
            if (LayoutParent != null)
            {
                LayoutParent.LayoutChildren.Remove(this);
            }

            transform.SetParent(target.transform, false);
            if (childIndex >= 0)
            {
                transform.SetSiblingIndex(childIndex);
            }

            if (updateLayoutParent)
            {
                Fields.SetValue(() => LayoutParent, target);
            }

            if (!target.LayoutChildren.Contains(this))
                target.LayoutChildren.Add(this);

            NotifyLayoutChanged();
        }

        /// <summary>
        /// Destroys the view and moves its content to a new parent.
        /// </summary>
        public void DestroyAndMoveContent(View newParent)
        {
            // move content
            MoveContent(newParent);

            // destroy
            this.Destroy();
        }

        /// <summary>
        /// Moves the view's content to a new parent.
        /// </summary>
        public void MoveContent(View newParent)
        {
            var children = Content.GetChildren<View>(false);
            foreach (var child in children)
            {
                child.MoveTo(newParent);
            }
        }

        /// <summary>
        /// Sets view action entry.
        /// </summary>
        public void SetViewActionEntry(ViewActionEntry entry)
        {
            // get view field data for binding target
            var viewFieldData = Fields.GetData(entry.FieldName);
            if (viewFieldData == null)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to assign view action handler \"{1}.{2}()\" to view action \"{3}\". "+
                    "View action not found.",
                    GameObjectName, Parent.ViewTypeName, entry.HandlerName, entry.FieldName));
                return;
            }

            bool hasValue;
            entry.SourceView = viewFieldData.SourceView;
            var viewAction = viewFieldData.GetValue(out hasValue) as ViewAction;
            if (hasValue)
            {
                viewAction.AddEntry(entry);
            }
        }

        /// <summary>
        /// Adds a view action handler for a certain view action.
        /// </summary>
        public void AddViewActionEntry(string actionFieldName, string actionHandlerName, View parent)
        {
            _viewActionEntries.Add(new ViewActionEntry
            {
                ParentView = parent,
                FieldName = actionFieldName,
                HandlerName = actionHandlerName
            });
        }

        /// <summary>
        /// Queues change handler to be called at the end of the frame.
        /// </summary>
        public void QueueChangeHandler(string handlerName)
        {
            _changeHandlers.Add(handlerName);

            // TODO optimize by caching this info in ViewTypeData
            if (!_changeHandlerMethods.ContainsKey(handlerName))
            {
                _changeHandlerMethods.Add(handlerName,
                    GetType()
                        .GetMethod(handlerName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
            }
        }

        /// <summary>
        /// Called after the view is initialized but before any XUML values are set.
        /// Used to set default values on the view.
        /// </summary>
        public virtual void SetDefaultValues()
        {
            GameObject = gameObject;
            State.DirectValue = DefaultStateName;
            IsActive.DirectValue = true;
        }

        /// <summary>
        /// Called when a field affecting the layout of the view has changed.
        /// </summary>
        public virtual void LayoutChanged()
        {
            NotifyLayoutChanged();
        }

        /// <summary>
        /// Notify the view that its layout has been changed. Should only be used when layout is
        /// not changed as a result of calling CalculateLayoutChanges.
        ///
        /// </summary>
        public void NotifyLayoutChanged()
        {
            _isLayoutChanged = true;
        }

        /// <summary>
        /// Calculate layout in response to NotifyLayoutChanged method called or because of propagation from
        /// another views NotifyLayoutChanged method being called.
        /// </summary>
        public virtual bool CalculateLayoutChanges(LayoutChangeContext context)
        {
            return false;
        }

        /// <summary>
        /// Called by LayoutChangeContext to notify that a child has recalculated its layout and as a result
        /// the child layout will change.
        /// </summary>
        public virtual void NotifyChildLayoutCalculated(View child, LayoutChangeContext context)
        {
            _isLayoutChanged = false;
            context.CalculateAsParent(this);
        }

        /// <summary>
        /// Called by LayoutChangeContext to notify that the parent has recalculated its layout and as a result
        /// the child layout will change.
        /// </summary>
        public virtual void NotifyParentLayoutCalculated(View parent, LayoutChangeContext context)
        {
            _isLayoutChanged = false;
            context.CalculateAsChild(this);
        }

        /// <summary>
        /// Called by LayoutChangeContext to render calculated layout.
        /// </summary>
        public virtual void RenderLayout()
        {
        }

        /// <summary>
        /// Called when a field affecting the behavior and visual appearance of the view has changed.
        /// </summary>
        public virtual void BehaviorChanged()
        {
        }

        /// <summary>
        /// Called when the Binding.Item is changed.
        /// </summary>
        public virtual void DataModelItemChanged(IObservableItem old, IObservableItem current)
        {
        }

        /// <summary>
        /// Called when the Id of the view changes.
        /// </summary>
        public virtual void IdChanged()
        {
            // set gameObject name to Id if set
            gameObject.name = GameObjectName;
        }

        /// <summary>
        /// Called when IsActive field has been changed.
        /// </summary>
        public virtual void IsActiveChanged()
        {
            gameObject.SetActive(IsActive.Value);

            if (IsActive.Value)
            {
                Activated.Trigger();
            }
            else
            {
                Deactivated.Trigger();
            }

            NotifyLayoutChanged();
        }

        /// <summary>
        /// Called when view state has been changed.
        /// </summary>
        public virtual void StateChanged()
        {
            States.NotifyStateChanged();
        }

        /// <summary>
        /// Creates a child view of specified type.
        /// </summary>
        public T CreateView<T>(int siblingIndex = -1, ValueConverterContext context = null,
                               string themeName = "", string id = "", string style = "",
                               IEnumerable<XElement> contentXuml = null) where T : View
        {
            var view = ViewData.CreateView<T>(this, this, context, themeName, Id, style);

            // set view sibling index
            if (siblingIndex > 0)
            {
                view.GameObject.transform.SetSiblingIndex(siblingIndex);
            }

            view.IsDynamic.DirectValue = true;
            return view;
        }

        /// <summary>
        /// Creates a view from a template and adds it to a parent at specified index.
        /// </summary>
        public static T CreateView<T>(T template, View layoutParent, int siblingIndex = -1,
                                      ViewPool viewPool = null) where T : View
        {
            GameObject go;

            // if pool isn't empty get an item from the pool
            if (viewPool != null && !viewPool.IsEmpty)
            {
                go = viewPool.GetView().gameObject;
            }
            else
            {
                // instantiate template
                go = Instantiate(template.gameObject);
            }

            go.hideFlags = UnityEngine.HideFlags.None;

            // set layout parent
            go.transform.SetParent(layoutParent.transform, false);

            // set view parent
            var view = go.GetComponent<T>();

            layoutParent.LayoutChildren.Add(view);

            if (siblingIndex > 0)
            {
                go.transform.SetSiblingIndex(siblingIndex);
            }

            view.IsTemplate.DirectValue = false;
            view.IsDynamic.DirectValue = true;
            //view.LayoutParent = layoutParent;
            return view;
        }

        /// <summary>
        /// Creates a child view from a template.
        /// </summary>
        public T CreateView<T>(T template, int siblingIndex = -1, ViewPool viewPool = null) where T : View
        {
            return CreateView(template, this, siblingIndex, viewPool);
        }

        /// <summary>
        /// Creates a pool of ready to be used views that can be drawn from when a new view is needed rather
        /// than creating them on-demand. Used to improve performance.
        /// </summary>
        public ViewPool GetViewPool(string poolName, View template, int poolSize, int maxPoolSize)
        {
            // does a view pool container exist for this template?
            var container = this.Find<ViewPoolContainer>(
                                x => x.Id == poolName && x.Template == template, false);

            if (container == null)
            {
                // no. create a new one
                container = CreateView<ViewPoolContainer>();
                container.Id = poolName;
                container.PoolSize.DirectValue = poolSize;
                container.MaxPoolSize.DirectValue = maxPoolSize;
                container.IsActive.DirectValue = false;
                container.Template = template;
                container.HideFlags.Value = UnityEngine.HideFlags.HideInHierarchy;
                // viewPoolContainer.HideFlags.Value = UnityEngine.HideFlags.HideAndDontSave; // TODO enable to only create during runtime
                container.InitializeViews();
            }
            else
            {
                // yes. just update pool size
                container.PoolSize.Value = poolSize;
                container.MaxPoolSize.Value = maxPoolSize;
                container.Template = template;
                container.UpdateViewPool();
            }

            return new ViewPool(container);
        }

        /// <summary>
        /// Initializes this view and all children. Used if the view is created dynamically and need to be
        /// called once to properly initialize the view.
        /// </summary>
        public void InitializeViews()
        {
            ViewPresenter.Instance.InitializeViews(this);
        }

        /// <summary>
        /// Called once to initialize the view. Called in reverse breadth-first order.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Initializes internal values to default values. Called once before InitializeInternal().
        /// Called in depth-first order.
        /// </summary>
        public virtual void InitializeInternalDefaultValues()
        {
            // initialize fields
            Fields.InitializeInternalDefaultValues();

            // initialize field states
            States.InitializeInternalDefaultValues();

            // initialize lists and dictionaries
            _eventSystemViewActions = new List<ViewAction>();
            _changeHandlers = new HashSet<string>();
            _changeHandlerMethods = new Dictionary<string, MethodInfo>();
            _layoutCalculator = DefaultLayoutCalculator.Instance;
        }

        /// <summary>
        /// Initializes the view internally.
        /// Called once before Initialize(). Called in depth-first order.
        /// </summary>
        public virtual void InitializeInternal()
        {
            // initialize bindings
            Bindings.InitializeInternal();

            // initialize fields
            Fields.InitializeInternal();

            // initialize field states
            States.InitializeInternal();

            // initialize action handlers
            foreach (var actionEntry in _viewActionEntries)
            {
                SetViewActionEntry(actionEntry);
            }

            // initialize change handlers
            var viewTypeData = ViewData.GetViewTypeData(ViewTypeName);
            foreach (var changeHandler in viewTypeData.ViewFieldChangeHandlers)
            {
                Fields.SetChangeHandler(changeHandler);
            }

            // initialize system event triggers
            if (Cancel.HasEntries) _eventSystemViewActions.Add(Cancel);
            if (Click.HasEntries) _eventSystemViewActions.Add(Click);
            if (Deselect.HasEntries) _eventSystemViewActions.Add(Deselect);
            if (BeginDrag.HasEntries) _eventSystemViewActions.Add(BeginDrag);
            if (Drag.HasEntries) _eventSystemViewActions.Add(Drag);
            if (Drop.HasEntries) _eventSystemViewActions.Add(Drop);
            if (EndDrag.HasEntries) _eventSystemViewActions.Add(EndDrag);
            if (InitializePotentialDrag.HasEntries) _eventSystemViewActions.Add(InitializePotentialDrag);
            if (Move.HasEntries) _eventSystemViewActions.Add(Move);
            if (MouseDown.HasEntries) _eventSystemViewActions.Add(MouseDown);
            if (MouseEnter.HasEntries) _eventSystemViewActions.Add(MouseEnter);
            if (MouseExit.HasEntries) _eventSystemViewActions.Add(MouseExit);
            if (MouseUp.HasEntries) _eventSystemViewActions.Add(MouseUp);
            if (Scroll.HasEntries) _eventSystemViewActions.Add(Scroll);
            if (Select.HasEntries) _eventSystemViewActions.Add(Select);
            if (Submit.HasEntries) _eventSystemViewActions.Add(Submit);
            if (UpdateSelected.HasEntries) _eventSystemViewActions.Add(UpdateSelected);

            InitEventSystemTriggers();
            IsInitialized = true;
        }

        /// <summary>
        /// Initializes unity event triggers.
        /// </summary>
        internal void InitEventSystemTriggers()
        {
            if (!_eventSystemViewActions.Any())
            {
#if UNITY_4_6 || UNITY_5_0
                _eventSystemTriggersInitialized = true;
#endif
                return;
            }

            var eventTrigger = GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();

#if UNITY_4_6 || UNITY_5_0
            var triggers = eventTrigger.delegates;
#else
            var triggers = eventTrigger.triggers;
#endif
            if (triggers == null)
            {
#if UNITY_4_6 || UNITY_5_0
                eventTrigger.delegates = new List<EventTrigger.Entry>();
#endif
                return;
            }

            triggers.Clear();

            foreach (var viewAction in _eventSystemViewActions)
            {
                var entry = new EventTrigger.Entry
                {
                    eventID = viewAction.EventTriggerType,
                    callback = new EventTrigger.TriggerEvent()
                };

                var eventViewAction = viewAction;
                var action = new UnityAction<BaseEventData>(eventData => eventViewAction.Trigger(eventData));
                entry.callback.AddListener(action);

                triggers.Add(entry);
            }

#if UNITY_4_6 || UNITY_5_0
            _eventSystemTriggersInitialized = true;
#endif
        }

        /// <summary>
        /// Calls InitializeInternalDefaultValues() and catches and prints any exception thrown.
        /// </summary>
        internal void TryInitializeInternalDefaultValues()
        {
#if !DISABLE_INIT_TRYCATCH
            try
            {
                InitializeInternalDefaultValues();
            }
            catch (Exception e)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: InitializeInternalDefaultValues() failed. Exception thrown: {1}",
                    GameObjectName, Utils.GetError(e)));
            }
#else
            InitializeInternalDefaultValues();
#endif
        }

        /// <summary>
        /// Calls InitializeInternalDefaultValues() and catches and prints any exception thrown if define is set.
        /// </summary>
        internal void TryInitializeInternal()
        {
#if !DISABLE_INIT_TRYCATCH
            try
            {
                InitializeInternal();
            }
            catch (Exception e)
            {
                Debug.LogError(String.Format("[MarkLight] {0}: InitializeInternal() failed. Exception thrown: {1}",
                    GameObjectName, Utils.GetError(e)));
            }
#else
            InitializeInternal();
#endif
        }

        /// <summary>
        /// Calls Initialize() and catches and prints any exception thrown.
        /// </summary>
        internal void TryInitialize()
        {
#if !DISABLE_INIT_TRYCATCH
            try
            {
                Initialize();
            }
            catch (Exception e)
            {
                Debug.LogError(String.Format("[MarkLight] {0}: Initialize() failed. Exception thrown: {1}",
                    GameObjectName, Utils.GetError(e)));
            }
#else
            Initialize();
#endif
        }

        /// <summary>
        /// Calls PropagateBindings() and catches and prints any exception thrown.
        /// </summary>
        internal void TryPropagateBindings()
        {
#if !DISABLE_INIT_TRYCATCH
            try
            {
                Bindings.Propagate();
            }
            catch (Exception e)
            {
                Debug.LogError(String.Format("[MarkLight] {0}: PropagateBindings() failed. Exception thrown: {1}",
                    GameObjectName, Utils.GetError(e)));
            }
#else
            Bindings.PropagateBindings();
#endif
        }

        /// <summary>
        /// Calls QueueAllChangeHandlers() and catches and prints any exception thrown.
        /// </summary>
        internal void TryQueueAllChangeHandlers()
        {
#if !DISABLE_INIT_TRYCATCH
            try
            {
                Fields.QueueAllChangeHandlers();
            }
            catch (Exception e)
            {
                Debug.LogError(String.Format("[MarkLight] {0}: QueueAllChangeHandlers() failed. Exception thrown: {1}",
                    GameObjectName, Utils.GetError(e)));
            }
#else
            QueueAllChangeHandlers();
#endif
        }

        /// <summary>
        /// Calls TriggerChangeHandlers() and catches and prints any exception thrown.
        /// </summary>
        internal void TryTriggerChangeHandlers()
        {
#if !DISABLE_INIT_TRYCATCH
            try
            {
                TriggerChangeHandlers();
            }
            catch (Exception e)
            {
                Debug.LogError(String.Format("[MarkLight] {0}: TriggerChangeHandlers() failed. Exception thrown: {1}",
                    GameObjectName, Utils.GetError(e)));
            }
#else
            TriggerChangeHandlers();
#endif
        }

        /// <summary>
        /// Returns string based on format string and parameters.
        /// </summary>
        public static string Format(string format, object arg)
        {
            return String.Format(format, arg ?? String.Empty);
        }

        /// <summary>
        /// Returns string based on format string and parameters.
        /// </summary>
        public static string Format1(string format, object arg)
        {
            return String.Format(format, arg ?? String.Empty);
        }

        /// <summary>
        /// Returns string based on format string and parameters.
        /// </summary>
        public static string Format2(string format, object arg1, object arg2)
        {
            return String.Format(format, arg1 ?? String.Empty, arg2 ?? String.Empty);
        }

        /// <summary>
        /// Returns string based on format string and parameters.
        /// </summary>
        public static string Format3(string format, object arg1, object arg2, object arg3)
        {
            return String.Format(format, arg1 ?? String.Empty, arg2 ?? String.Empty, arg3 ?? String.Empty);
        }

        /// <summary>
        /// Gets child view enumerator.
        /// </summary>
        public IEnumerator<View> GetEnumerator()
        {
            foreach (Transform child in gameObject.transform)
            {
                var childView = child.GetComponent<View>();
                if (childView == null)
                {
                    continue;
                }

                yield return childView;
            }
        }

        /// <summary>
        /// Gets child view enumerator.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Called when asset has been loaded or unloaded.
        /// </summary>
        public virtual void OnAssetChanged(UnityAsset asset)
        {
            //Utils.Log("Notifying Observer {0} that sprite {1} IsLoaded = {2}", GameObjectName, sprite.Path, sprite.IsLoaded);
        }

        /// <summary>
        /// Triggers queued change handlers.
        /// </summary>
        private void TriggerChangeHandlers()
        {
            if (_changeHandlers.Count > 0)
            {
                var triggeredChangeHandlers = new List<string>(_changeHandlers);
                _changeHandlers.Clear();

                foreach (var changeHandler in triggeredChangeHandlers)
                {
                    try
                    {
                        _changeHandlerMethods[changeHandler].Invoke(this, null);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(String.Format("[MarkLight] {0}: Exception thrown in change handler \"{1}\": {2}",
                            GameObjectName, changeHandler, Utils.GetError(e)));
                    }
                }
            }

            if (!_isLayoutChanged || _isLayoutCalculating)
                return;

            _isLayoutChanged = false;
            _isLayoutCalculating = true;

            // calculate new layout
            var change = new LayoutChangeContext();
            change.Calculate(this);

            _isLayoutCalculating = false;
            change.RenderLayout();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the views Type name.
        /// </summary>
        public string ViewTypeName
        {
            get { return _viewTypeName; }
        }

        /// <summary>
        /// Get the views Xuml tag name.
        /// </summary>
        public string ViewXumlName
        {
            get { return _viewXumlName; }
            set
            {
                if (_viewXumlName != null)
                    throw new InvalidOperationException("Cannot set ViewTypeName more than once.");

                _viewXumlName = value;
            }
        }

        /// <summary>
        /// Get the views binding manager.
        /// </summary>
        public ViewBindings Bindings
        {
            get { return _bindings; }
        }

        /// <summary>
        /// Get the views field manager.
        /// </summary>
        public ViewFields Fields
        {
            get { return _fields; }
        }

        /// <summary>
        /// Get the view field states manager.
        /// </summary>
        public ViewFieldStates States
        {
            get { return _states; }
        }

        /// <summary>
        /// Gets boolean indicating if this view is live (enabled and not destroyed).
        /// </summary>
        public bool IsLive
        {
            get { return IsActive && !IsDestroyed; }
        }

        /// <summary>
        /// Gets boolean indicating if the view has any queued change handlers.
        /// </summary>
        public bool HasQueuedChangeHandlers
        {
            get { return _changeHandlers != null && _changeHandlers.Count > 0; }
        }

        /// <summary>
        /// Gets list of currently queued change handlers.
        /// </summary>
        public List<string> QueuedChangeHandlers
        {
            get { return _changeHandlers != null ? _changeHandlers.ToList() : new List<string>(); }
        }

        /// <summary>
        /// Gets child count.
        /// </summary>
        public int ChildCount
        {
            get { return transform.childCount; }
        }

        /// <summary>
        /// Gets the child layout calculator.
        /// </summary>
        public LayoutCalculator LayoutCalculator
        {
            get { return _layoutCalculator; }
            protected set { _layoutCalculator = value; }
        }

        /// <summary>
        /// Gets GameObject name (usually view type + id). 
        /// </summary>
        public string GameObjectName
        {
            get
            {
                var viewName = ViewTypeName == "View" ? ViewXumlName : ViewTypeName;                
                return !String.IsNullOrEmpty(Id) ? String.Format("{0} ({1})", viewName, Id) : viewName;
            }
        }

        /// <summary>
        /// Gets view type data.
        /// </summary>
        public ViewTypeData ViewTypeData
        {
            get { return _viewTypeData ?? (_viewTypeData = ViewData.GetViewTypeData(ViewTypeName)); }
        }

        /// <summary>
        /// Gets or sets bool indicating if the view has been initialized.
        /// </summary>
        public bool IsInitialized
        {
            get { return _isInitialized; }
            set { _isInitialized = value; }
        }

        #endregion
    }
}
