using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLinq.Utility;

namespace HLinq
{
    public static class LinqExtensions
    {
        /// <summary>
        /// An extension method to overload the Linq Except() function allowing implicit use of a lambda expression, thus completely abstracting away the LambdaComparer class
        /// </summary>
        /// <typeparam name="TSource">The type of IEnumerable to use</typeparam>
        /// <param name="first">The source IEnumerable to operate on.</param>
        /// <param name="second">The IEnumerable to compare against.</param>
        /// <param name="comparer">A function in the form (a, b) => a.someKeyIdentifier == b.someKeyIdentifier to perform the except comparison against.</param>
        /// <returns>A filtered subset of the source IEnumerable, containing everything but any items in second that can be matched via the comparer.</returns>
        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first,
            IEnumerable<TSource> second, Func<TSource, TSource, bool> comparer)
        {
            return first.Except(second, new LambdaComparer<TSource>(comparer));
        }

        /// <summary>
        /// An extension method to overload the Linq Intersect() function allowing implicit use of a lambda expression, thus completely abstracting away the LambdaComparer class
        /// </summary>
        /// <typeparam name="TSource">The type of IEnumerable to use</typeparam>
        /// <param name="first">The source IEnumerable to operate on.</param>
        /// <param name="second">The IEnumerable to compare against.</param>
        /// <param name="comparer">A function in the form (a, b) => a.someKeyIdentifier == b.someKeyIdentifier to perform the intersect comparison against.</param>
        /// <returns>An intersection of the source IEnumerable and the second IEnumerable, where items can be matched on their comparer function.</returns>
        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first,
            IEnumerable<TSource> second, Func<TSource, TSource, bool> comparer)
        {
            return first.Intersect(second, new LambdaComparer<TSource>(comparer));
        }
    }
}
