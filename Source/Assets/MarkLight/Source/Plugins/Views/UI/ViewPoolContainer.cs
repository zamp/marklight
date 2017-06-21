
namespace MarkLight.Views.UI
{
    /// <summary>
    /// Pools views for fast creation.
    /// </summary>
    [HideInPresenter]
    public class ViewPoolContainer : UIView
    {
        #region Fields

        public _int PoolSize;
        public _int MaxPoolSize;
        public View Template;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            UpdateViewPool();            
        }

        /// <summary>
        /// Updates the view pool based on settings.
        /// </summary>
        public void UpdateViewPool()
        {
            if (PoolSize > MaxPoolSize)
            {
                MaxPoolSize.Value = PoolSize.Value;
            }

            var templateTypeName = Template != null ? Template.ViewTypeName : null;
            var itemsToDestroy = BufferPools.ViewLists.Get();

            // remove any items not of the right type
            foreach (var child in this)
            {
                if (child.ViewTypeName != templateTypeName)
                {
                    itemsToDestroy.Add(child);
                }
            }

            itemsToDestroy.ForEach(x => x.Destroy());

            BufferPools.ViewLists.Recycle(itemsToDestroy);

            // fill remaining space of pool with views
            if (Template == null || ChildCount >= PoolSize)
            {
                return;
            }
            
            var addCount = PoolSize - ChildCount;
            for (var i = 0; i < addCount; ++i)
            {
                var item = CreateView(Template);
                item.Deactivate();
                item.InitializeViews();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Boolean indicating if pool container is full.
        /// </summary>
        public bool IsFull
        {
            get
            {
                return ChildCount >= MaxPoolSize;
            }
        }

        #endregion
    }
}
