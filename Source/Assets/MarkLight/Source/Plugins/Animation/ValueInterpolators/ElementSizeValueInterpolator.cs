
namespace MarkLight.Animation
{
    /// <summary>
    /// Element size value interpolator.
    /// </summary>
    public class ElementSizeValueInterpolator : ValueInterpolator
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ElementSizeValueInterpolator()
        {
            _type = typeof(ElementSize);
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Interpolates between two element sizes based on a weight.
        /// </summary>
        public override object Interpolate(object from, object to, float weight)
        {
            var a = (ElementSize)from;
            var b = (ElementSize)to;

            if (a.Unit == ElementSizeUnit.Percents || b.Unit == ElementSizeUnit.Percents)
            {
                return a.Unit != b.Unit
                    ? from
                    : new ElementSize(Lerp(a.Percent, b.Percent, weight), ElementSizeUnit.Percents);
            }
            
            return new ElementSize(Lerp(a.Pixels, b.Pixels, weight), ElementSizeUnit.Pixels);
        }

        #endregion
    }
}
