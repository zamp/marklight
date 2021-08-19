
namespace MarkLight.Animation
{
    /// <summary>
    /// Margin value interpolator.
    /// </summary>
    public class MarginValueInterpolator : ValueInterpolator
    {
        #region Fields

        private readonly ElementSizeValueInterpolator _sizeInterpolator;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public MarginValueInterpolator()
        {
            _type = typeof(ElementMargin);
            _sizeInterpolator = new ElementSizeValueInterpolator();
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Interpolates between two colors based on a weight.
        /// </summary>
        public override object Interpolate(object from, object to, float weight)
        {
            var a = (ElementMargin)from;
            var b = (ElementMargin)to;

            return new ElementMargin(
                (ElementSize)_sizeInterpolator.Interpolate(a.Left, b.Left, weight),
                (ElementSize)_sizeInterpolator.Interpolate(a.Top, b.Top, weight),
                (ElementSize)_sizeInterpolator.Interpolate(a.Right, b.Right, weight),
                (ElementSize)_sizeInterpolator.Interpolate(a.Bottom, b.Bottom, weight));
        }

        #endregion
    }
}
