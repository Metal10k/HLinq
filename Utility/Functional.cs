using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLinq.Utility
{
    /// <summary>
    /// Static helper methods for functional programming paradigms.
    /// </summary>
    public static class Functional
    {
        /// <summary>
        /// The delegate represents a recursive encapsulation of the external function.
        /// </summary>
        /// <typeparam name="A">The recursive parameter found in the inner function.</typeparam>
        /// <typeparam name="R">The recursive return type found in the inner function.</typeparam>
        /// <param name="r">The inner recursive function to be processed by this function. This will normally be itself.</param>
        /// <returns>A function that relates this recursive delegate to itself, thus allowing recursion.</returns>
        private delegate Func<A, R> Recursive<A, R>(Recursive<A, R> r);

        /// <summary>
        /// Y Combinator.
        /// See <see cref="http://en.wikipedia.org/wiki/Fixed-point_combinator#Y_combinator"/> for more details.
        /// </summary>
        /// <typeparam name="A">The recursive parameter found in the inner function.</typeparam>
        /// <typeparam name="R">The recursive return type found in the inner function.</typeparam>
        /// <param name="f">The higher order function (this will itself return a function which will process the actual variable)
        /// This is in the form (f=> y => function to determine relationship between input variable y and higher order function)</param>
        /// <returns>A function object which can execute internal function recursively on demand</returns>
        /// <example>
        /// //Requires an expression in the form :
        /// f =>
        /// y => 
        /// { 
        ///    var myResult = myCustomFunction(y, parameters);
        ///    
        ///     //some logic goes here to process myResult
        ///     if(someRecursionCheck(myResult))
        ///     {
        ///         process(myResult);
        ///         //perform recursion
        ///         f(myResult.y);
        ///     }
        ///    return something;
        ///});
        /// </example>
        public static Func<A, R> Y<A, R>(Func<Func<A, R>, Func<A, R>> f)
        {
            Recursive<A, R> rec = r => a => f(r(r))(a);
            return rec(rec);
        }
    }
}
