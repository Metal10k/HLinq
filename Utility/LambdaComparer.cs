using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLinq.Utility
{
    /// <summary>
    /// Allows an IEqualityComparer object to be used with a simple lambda expression.
    /// This fits well with more complex Linq extensions such as Except() where a simple inline comparer is preferred
    /// </summary>
    /// <typeparam name="T">Type of object being compared</typeparam>
    public class LambdaComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// The comparer function to compare two arbitrary objects.
        /// </summary>
        private readonly Func<T, T, bool> _lambdaComparer;

        /// <summary>
        /// The hash function used in the GetHashCode(T obj) 
        /// </summary>
        private readonly Func<T, int> _lambdaHash;

        /// <summary>
        /// Public constructor for the LambdaComparer class.
        /// </summary>
        /// <param name="lambdaComparer">Expects a Func to compare two objects of type T by some arbitrary expression.</param>
        public LambdaComparer(Func<T, T, bool> lambdaComparer) :
            this(lambdaComparer, o => 0)
        {
        }

        /// <summary>
        /// Public constructor for the LambdaComparer class.
        /// </summary>
        /// <param name="lambdaComparer">Expects a Func to compare two objects of type T by some arbitrary expression.</param>
        /// <param name="lambdaHash">The hash function used by the GetHashCode(T obj).</param>
        public LambdaComparer(Func<T, T, bool> lambdaComparer, Func<T, int> lambdaHash)
        {
            if (lambdaComparer == null)
            {
                throw new ArgumentNullException("lambdaComparer");
            }
            if (lambdaHash == null)
            {
                throw new ArgumentNullException("lambdaHash");
            }

            _lambdaComparer = lambdaComparer;
            _lambdaHash = lambdaHash;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(T x, T y)
        {
            return _lambdaComparer(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The System.Object for which a hash code is to be returned.</param>
        /// <exception cref="ArgumentNullException">The type of obj is a reference type and obj is null.</exception>
        /// <returns>Returns a hash code for the specified object.</returns>
        public int GetHashCode(T obj)
        {
            return _lambdaHash(obj);
        }
    }
}
