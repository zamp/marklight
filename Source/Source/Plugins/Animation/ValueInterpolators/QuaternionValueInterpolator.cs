using UnityEngine;

namespace MarkLight.Animation
{
    /// <summary>
    /// Element size value interpolator.
    /// </summary>
    public class QuaternionValueInterpolator : ValueInterpolator
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public QuaternionValueInterpolator()
        {
            _type = typeof(Quaternion);
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Interpolates between two element sizes based on a weight.
        /// </summary>
        public override object Interpolate(object from, object to, float weight)
        {
            var q1 = (Quaternion)from;
            var q2 = (Quaternion)to;
                      
            return Quaternion.Lerp(q1, q2, weight);
        }

        #endregion
    }
}
