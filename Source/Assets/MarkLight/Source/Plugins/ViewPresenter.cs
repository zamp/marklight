using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marklight.Themes;
using MarkLight.Views.UI;
using MarkLight.Animation;
using MarkLight.ValueConverters;
#if !UNITY_4_6 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3_1 && !UNITY_5_3_2 && !UNITY_5_3_3
using UnityEngine.SceneManagement;
#endif

namespace MarkLight
{
    /// <summary>
    /// MarkLight Presentation Engine.
    /// </summary>
    [AddComponentMenu("MarkLight/View Presenter")]
    public class ViewPresenter : View
    {
        #region Fields

        [NonSerialized]
        public bool IsLayoutDirty;

        public List<ViewTypeData> ViewTypeDataList;
        public List<Theme> Themes;
        public List<ResourceDictionary> ResourceDictionaries;
        public string MainView;
        public string DefaultTheme;
        public string DefaultLanguage;
        public string DefaultPlatform;
        public List<string> ViewTypeNames;
        public List<string> ThemeNames;
        public GameObject RootView;
        public bool DisableAutomaticReload;
        public bool UpdateXsdSchema;

        public AssetDictionary AssetDictionary;

        private static ViewPresenter _instance;
        private static string _currentScene;
        private Dictionary<string, ValueConverter> _cachedValueConverters;
        private Dictionary<string, Type> _viewTypes;
        private Dictionary<string, ViewTypeData> _viewTypeDataDictionary;
        private Dictionary<string, Theme> _themeDictionary;
        private Dictionary<string, ResourceDictionary> _resourceDictionaries;
        private Dictionary<string, ValueConverter> _valueConvertersForType;
        private Dictionary<string, ValueConverter> _valueConverters;
        private Dictionary<string, ValueInterpolator> _valueInterpolatorsForType;
        private readonly LayoutChangeContext _layoutContext = new LayoutChangeContext();

        private Resolution _prevResolution;

        private static readonly ViewSearchArgs CalculateAndRenderViewSearchArgs = new ViewSearchArgs(true)
        {
            TraversalAlgorithm = TraversalAlgorithm.ReverseBreadthFirst,
            SkipInactive = true
        };

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ViewPresenter()
        {
            AssetDictionary = new AssetDictionary();
            ViewTypeDataList = new List<ViewTypeData>();
            Themes = new List<Theme>();
            ResourceDictionaries = new List<ResourceDictionary>();
            ViewTypeNames = new List<string>();
            ThemeNames = new List<string>();
            BaseDirectory = ValueConverterContext.DefaultBaseDirectory;
            UnitSize = ValueConverterContext.DefaultUnitSize;
            HexColorType = ValueConverterContext.DefaultHexColorType;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called once at startup.
        /// </summary>
        public void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Unity late update message pump.
        /// </summary>
        public void LateUpdate() {

            // check for resolution size change
            var cam = Camera.main;
            var isResolutionChanged = false;

            if (cam != null)
            {
                var width = cam.pixelWidth;
                var height = cam.pixelHeight;

                isResolutionChanged = _prevResolution.width != width || _prevResolution.height != height;

                if (isResolutionChanged)
                {
                    _prevResolution = new Resolution
                    {
                        width = width,
                        height = height
                    };
                }
            }

            if (isResolutionChanged)
            {
                this.ForThisAndEachChild<UIView>(x =>
                {
                    x.Layout.IsDirty = true;
                    x.NotifyLayoutChanged();
                    x.ResolutionChanged();
                }, ViewSearchArgs.Default);
            }

            var isRenderRequired = !isResolutionChanged && IsLayoutDirty;

            this.ForThisAndEachChild<View>(x =>
            {
                x.TriggerChangeHandlers();

#if UNITY_4_6 || UNITY_5_0
                if (!x._eventSystemTriggersInitialized)
                {
                    x.InitEventSystemTriggers();
                }
#endif

                if (isRenderRequired)
                {
                    x.CalculateAndRenderLayout(_layoutContext);
                }

            }, CalculateAndRenderViewSearchArgs);

            if (isRenderRequired)
                _layoutContext.RenderLayout();

            _layoutContext.Reset();
        }

        /// <summary>
        /// Called once to initialize views and runtime data.
        /// </summary>
        public override void Initialize()
        {
            UpdateInstance();

            // initialize resource dictionary
            ResourceDictionary.Language = DefaultLanguage;
            ResourceDictionary.Platform = DefaultPlatform;
            ResourceDictionary.Initialize();

            // initialize all views in the scene
            InitializeViews(RootView);
        }

        /// <summary>
        /// Initializes the views. Called once on root view at the start of the scene. Need to be called on any views
        /// created dynamically.
        /// </summary>
        public void InitializeViews(GameObject rootView)
        {
            if (rootView == null)
                return;

            InitializeViews(rootView.GetComponent<View>());
        }

        /// <summary>
        /// Initializes the views. Called once on root view at the start of the scene. Need to be called on any views
        /// created dynamically.
        /// </summary>
        public void InitializeViews(View rootView)
        {
            if (rootView == null || rootView.IsInitialized)
            {
                return;
            }

            // uncomment to log initialization performance
            //Stopwatch sw = new Stopwatch();
            //sw.Start();

            rootView.ForThisAndEachChild<View>(x => x.TryInitializeInternalDefaultValues(), ViewSearchArgs.Default);
            rootView.ForThisAndEachChild<View>(x => x.TryInitializeInternal(), ViewSearchArgs.Default);
            rootView.ForThisAndEachChild<View>(x => x.TryInitialize(), ViewSearchArgs.ReverseBreadthFirst);
            rootView.ForThisAndEachChild<View>(x => x.TryPropagateBindings(), ViewSearchArgs.BreadthFirst);
            rootView.ForThisAndEachChild<View>(x => x.TryQueueAllChangeHandlers(), ViewSearchArgs.ReverseBreadthFirst);

            // notify dictionary observers
            ResourceDictionary.NotifyObservers();

            // trigger change handlers
            int pass = 0;
            while (rootView.Find<View>(x => x.HasQueuedChangeHandlers, ViewSearchArgs.Default) != null)
            {
                if (pass >= 1000)
                {
                    PrintTriggeredChangeHandlerOverflowError(pass, rootView);
                    break;
                }

                var changeContext = new LayoutChangeContext();

                // as long as there are change handlers queued, go through all views and trigger them
                rootView.ForThisAndEachChild<View>(x =>
                {
                    x.TryTriggerChangeHandlers();
                    x.CalculateAndRenderLayout(changeContext, true);
                }, ViewSearchArgs.ReverseBreadthFirst);

                changeContext.RenderLayout();

                ++pass;
            }

            // uncomment to log initialization performance
            //sw.Stop();
            //if (rootView.gameObject == RootView)
            //{
            //    Utils.Log("Initialization time: {0}", sw.ElapsedMilliseconds);
            //}
        }

        /// <summary>
        /// Prints triggered change handler overflow error message.
        /// </summary>
        private void PrintTriggeredChangeHandlerOverflowError(int pass, View rootView)
        {
            var sb = BufferPools.StringBuilders.Get();
            var triggeredViews =
                rootView.GetChildren<View>(x => x.HasQueuedChangeHandlers, ViewSearchArgs.Default);

            for (var i = 0; i < triggeredViews.Count; i++)
            {
                var triggeredView = triggeredViews[i];
                sb.AppendFormat("{0}: ", triggeredView.GameObjectName);
                sb.AppendLine();

                for (var j = 0; j < triggeredView.QueuedChangeHandlers.Count; j++)
                {
                    var triggeredChangeHandler = triggeredView.QueuedChangeHandlers[j];
                    sb.AppendFormat("\t{0}", triggeredChangeHandler);
                    sb.AppendLine();
                }
            }

            Debug.LogError(String.Format(
                "[MarkLight] Error initializing views. Stack overflow when triggering change handlers. "+
                "Make sure your change handlers doesn't trigger each other in a loop. "+
                "The following change handlers were still triggered after {0} passes:{1}{2}",
                pass, Environment.NewLine, sb));

            BufferPools.StringBuilders.Recycle(sb);
        }

        /// <summary>
        /// Removes all view data from presenter and clears the scene.
        /// </summary>
        public void Clear()
        {
            Themes.Clear();
            ViewTypeDataList.Clear();
            ResourceDictionaries.Clear();
            AssetDictionary.Clear();

            _viewTypeDataDictionary = null;
            _themeDictionary = null;
            _resourceDictionaries = null;
            _viewTypes = null;

            if (RootView != null)
            {
                DestroyImmediate(RootView);
            }
        }

        /// <summary>
        /// Gets view type data.
        /// </summary>
        public ViewTypeData GetViewTypeData(string viewTypeName)
        {
            if (_viewTypeDataDictionary == null)
            {
                LoadViewTypeDataDictionary();
            }

            ViewTypeData viewTypeData;
            if (!_viewTypeDataDictionary.TryGetValue(viewTypeName, out viewTypeData))
            {
                Debug.LogError(String.Format("[MarkLight] Can't find view type \"{0}\".", viewTypeName));
                return null;
            }

            return viewTypeData;
        }

        /// <summary>
        /// Loads the view type data dictionary.
        /// </summary>
        private void LoadViewTypeDataDictionary()
        {
            _viewTypeDataDictionary = new Dictionary<string, ViewTypeData>();
            foreach (var viewTypeData in ViewTypeDataList)
            {
                foreach (var viewName in viewTypeData.ViewNameAliases)
                {
                    if (_viewTypeDataDictionary.ContainsKey(viewName))
                    {
                        Debug.LogError(String.Format(
                            "[MarkLight] Can't map view-model \"{0}\" to view \"{1}\" because it is already mapped "+
                            "to view-model \"{2}\". If you want to replace another view-model use the "+
                            "ReplaceViewModel class attribute. Otherwise choose a different view name that is "+
                            "available.",
                            viewTypeData.ViewTypeName, viewName, _viewTypeDataDictionary[viewName].ViewTypeName));
                        continue;
                    }

                    _viewTypeDataDictionary.Add(viewName, viewTypeData);
                }
            }

            // check if view-models should be replaced
            foreach (var viewTypeData in ViewTypeDataList.Where(x => !String.IsNullOrEmpty(x.ReplacesViewModel)))
            {
                // find the view type it replaces
                var replacedViewTypeData = ViewTypeDataList.FirstOrDefault(x => String.Equals(x.ViewTypeName,
                    viewTypeData.ReplacesViewModel, StringComparison.OrdinalIgnoreCase));

                if (replacedViewTypeData == null)
                    continue;

                // replace the view-model
                foreach (var kv in _viewTypeDataDictionary.ToList())
                {
                    if (kv.Value == replacedViewTypeData)
                    {
                        _viewTypeDataDictionary[kv.Key] = viewTypeData;
                    }
                }
            }
        }

        /// <summary>
        /// Gets theme data.
        /// </summary>
        public Theme GetTheme(string themeName)
        {
            if (_themeDictionary == null)
            {
                _themeDictionary = new Dictionary<string, Theme>();
                foreach (var theme in Themes)
                {
                    _themeDictionary.Add(theme.Name, theme);
                }
            }

            return _themeDictionary.Get(themeName);
        }

        /// <summary>
        /// Gets resource dictionary.
        /// </summary>
        public ResourceDictionary GetResourceDictionary(string dictionaryName)
        {
            if (_resourceDictionaries == null)
            {
                _resourceDictionaries = new Dictionary<string, ResourceDictionary>();
                foreach (var resourceDictionary in ResourceDictionaries)
                {
                    _resourceDictionaries.Add(resourceDictionary.Name, resourceDictionary);
                }
            }

            return _resourceDictionaries.Get(dictionaryName);
        }

        /// <summary>
        /// Gets pre-loaded asset from path.
        /// </summary>
        public UnityAsset GetAsset(string assetPath)
        {
            return AssetDictionary.Get(assetPath);
        }

        /// <summary>
        /// Gets pre-loaded asset from path.
        /// </summary>
        public UnityAsset GetAsset(UnityEngine.Object asset)
        {
            return AssetDictionary.Get(asset);
        }

        /// <summary>
        /// Gets asset path from asset.
        /// </summary>
        public string GetAssetPath(UnityEngine.Object asset)
        {
            var unityAsset = AssetDictionary.Get(asset);
            return unityAsset != null ? unityAsset.Path : String.Empty;
        }        

        /// <summary>
        /// Adds asset to list of loaded assets.
        /// </summary>
        public UnityAsset AddAsset(string path, UnityEngine.Object asset)
        {
            if (AssetDictionary.ContainsKey(path))
                return AssetDictionary[path];

            var unityAsset = new UnityAsset(path, asset);
            AssetDictionary.Add(unityAsset);
            return unityAsset;
        }

        /// <summary>
        /// Gets view type from view type name.
        /// </summary>
        public Type GetViewType(string viewTypeName)
        {
            if (_viewTypes != null)
                return _viewTypes.Get(viewTypeName);

            _viewTypes = new Dictionary<string, Type>();
            foreach (var viewType in TypeHelper.FindDerivedTypes(typeof(View)))
            {
                _viewTypes.Add(viewType.Name, viewType);
            }

            return _viewTypes.Get(viewTypeName);
        }

        /// <summary>
        /// Gets value converter for view field type.
        /// </summary>
        public ValueConverter GetValueConverterForType(string viewFieldType)
        {
            if (_valueConvertersForType != null)
                return _valueConvertersForType.Get(viewFieldType);

            _valueConvertersForType = new Dictionary<string, ValueConverter>
            {
                {"Object", new ValueConverter()},
                {"Single", new FloatValueConverter()},
                {"Int32", IntValueConverter.Instance},
                {"Boolean", new BoolValueConverter()},
                {"Color", ColorValueConverter.Instance},
                {"ElementSize", new ElementSizeValueConverter()},
                {"Enum", new EnumValueConverter()},
                {"Component", new ComponentValueConverter()},
                {"Font", new FontValueConverter()},
                {"ElementMargin", new MarginValueConverter()},
                {"Material", new MaterialValueConverter()},
                {"Quaternion", new QuaternionValueConverter()},
                {"Sprite", new SpriteValueConverter()},
                {"UnityAsset", new AssetValueConverter()},
                {"SpriteAsset", new SpriteAssetValueConverter()},
                {"String", new StringValueConverter()},
                {"Vector2", new Vector2ValueConverter()},
                {"Vector3", new Vector3ValueConverter()},
                {"Vector4", new Vector4ValueConverter()},
                {"ElementAspectRatio", new ElementAspectRatioConverter()}
            };

            // cache standard converters to improve load performance

            foreach (var valueConverterType in TypeHelper.FindDerivedTypes(typeof(ValueConverter)))
            {
                if (CachedValueConverters.ContainsKey(valueConverterType.Name))
                    continue;

                var valueConverter = TypeHelper.CreateInstance(valueConverterType) as ValueConverter;
                if (valueConverter.Type != null)
                {
                    var valueTypeName = valueConverter.Type.Name;
                    if (!_valueConvertersForType.ContainsKey(valueTypeName))
                    {
                        _valueConvertersForType.Add(valueTypeName, valueConverter);
                    }
                }
            }

            return _valueConvertersForType.Get(viewFieldType);
        }

        /// <summary>
        /// Gets value converter.
        /// </summary>
        public ValueConverter GetValueConverter(string valueConverterTypeName)
        {
            if (_valueConverters == null)
            {
                _valueConverters = new Dictionary<string, ValueConverter>();

                // cache standard converters to improve load performance
                foreach (var cachedConverter in CachedValueConverters)
                {
                    _valueConverters.Add(cachedConverter.Key, cachedConverter.Value);
                }

                foreach (var valueConverterType in TypeHelper.FindDerivedTypes(typeof(ValueConverter)))
                {
                    if (_valueConverters.ContainsKey(valueConverterType.Name))
                        continue;

                    var valueConverter = TypeHelper.CreateInstance(valueConverterType) as ValueConverter;
                    _valueConverters.Add(valueConverterType.Name, valueConverter);
                }
            }

            return _valueConverters.Get(valueConverterTypeName);
        }

        /// <summary>
        /// Gets value interpolator for view field type.
        /// </summary>
        public ValueInterpolator GetValueInterpolatorForType(string viewFieldType)
        {
            if (_valueInterpolatorsForType == null)
            {
                _valueInterpolatorsForType = new Dictionary<string, ValueInterpolator>();
                foreach (var valueInterpolatorType in TypeHelper.FindDerivedTypes(typeof(ValueInterpolator)))
                {
                    var valueInterpolator = TypeHelper.CreateInstance(valueInterpolatorType) as ValueInterpolator;
                    if (valueInterpolator.Type != null)
                    {
                        var valueTypeName = valueInterpolator.Type.Name;
                        if (!_valueInterpolatorsForType.ContainsKey(valueTypeName))
                        {
                            _valueInterpolatorsForType.Add(valueTypeName, valueInterpolator);
                        }
                    }
                }
            }

            return _valueInterpolatorsForType.Get(viewFieldType);
        }

        /// <summary>
        /// Loads asset at path.
        /// </summary>
        public UnityAsset LoadAsset(string path, UnityEngine.Object asset)
        {
            var unityAsset = GetAsset(path);
            if (unityAsset == null)
            {
                unityAsset = AddAsset(path, asset);
            }
            else
            {
                unityAsset.Asset = asset;
                unityAsset.NotifyObservers();
            }

            return unityAsset;
        }

        /// <summary>
        /// Unloads the asset at path.
        /// </summary>
        public void UnloadAsset(string path)
        {
            var unityAsset = GetAsset(path);
            if (unityAsset == null)
            {
                Debug.LogError(String.Format("[MarkLight] Unable to unload asset \"{0}\". Asset not found.", path));
                return;
            }

            unityAsset.Unload();
        }

        /// <summary>
        /// Refreshes and updates the view presenter instance.
        /// </summary>
        public static void UpdateInstance()
        {
#if !UNITY_4_6 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3_1 && !UNITY_5_3_2 && !UNITY_5_3_3
            var sceneName = SceneManager.GetActiveScene().name;
#else
            var sceneName = Application.loadedLevelName;
#endif
            if (_instance == null || sceneName != _currentScene)
            {
                _instance = UnityEngine.Object.FindObjectOfType(typeof(ViewPresenter)) as ViewPresenter;
                _currentScene = sceneName;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets global presentation engine instance.
        /// </summary>
        public static ViewPresenter Instance
        {
            get
            {
                UpdateInstance();
                return _instance;
            }
        }

        /// <summary>
        /// Gets cached value converters.
        /// </summary>
        private Dictionary<string, ValueConverter> CachedValueConverters
        {
            get
            {
                return _cachedValueConverters ?? (_cachedValueConverters = new Dictionary<string, ValueConverter>
                {
                    {"ValueConverter", new ValueConverter()},
                    {"FloatValueConverter", new FloatValueConverter()},
                    {"IntValueConverter", IntValueConverter.Instance},
                    {"BoolValueConverter", new BoolValueConverter()},
                    {"ColorValueConverter", ColorValueConverter.Instance},
                    {"ElementSizeValueConverter", new ElementSizeValueConverter()},
                    {"ComponentValueConverter", new ComponentValueConverter()},
                    {"EnumValueConverter", new EnumValueConverter()},
                    {"FontValueConverter", new FontValueConverter()},
                    {"MarginValueConverter", new MarginValueConverter()},
                    {"MaterialValueConverter", new MaterialValueConverter()},
                    {"QuaternionValueConverter", new QuaternionValueConverter()},
                    {"SpriteValueConverter", new SpriteValueConverter()},
                    {"SpriteAssetValueConverter", new SpriteAssetValueConverter()},
                    {"AssetValueConverter", new AssetValueConverter()},
                    {"StringValueConverter", new StringValueConverter()},
                    {"Vector2ValueConverter", new Vector2ValueConverter()},
                    {"Vector3ValueConverter", new Vector3ValueConverter()},
                    {"Vector4ValueConverter", new Vector4ValueConverter()}
                });
            }
        }

        #endregion
    }
}
