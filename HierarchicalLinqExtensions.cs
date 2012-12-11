using System;
using System.Collections.Generic;
using System.Linq;
using HLinq.Utility;

namespace HLinq
{
    /// <summary>
    /// Linq Extension Methods
    /// </summary>
    public static class HierarchicalLinqExtensions
    {
        /// <summary>
        /// Under normal circumstances, it is assumed that a single parent will be returned for a particular object. If there are more matches an exception will be thrown. 
        /// This will select only that object, or return the object's default value.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="source">The list of objects to traverse</param>
        /// <param name="target">The context object in which you want to find a related object from</param>
        /// <param name="parentPredicate">The predicate function which determines if object b is infact the parent of object a
        /// Format - parentPredicate(target, potentialParent) <example>(c, p) => c.ParentId == p.Id</example></param>
        /// <returns>The parent object of the target from the source list.</returns>
        /// <exception cref="InvalidOperationException">Sequence contains more than one element</exception>
        public static T ParentSingleOrDefault<T>(this IEnumerable<T> source, T target, Func<T, T, bool> parentPredicate) where T : class
        {
            return source.SingleOrDefault(q => parentPredicate(target, q));
        }

        /// <summary>
        /// This will return all of the direct children of the current object that match the parent predicate.
        /// </summary>
        /// <typeparam name="T">The type of objects to enumerate.</typeparam>
        /// <param name="source">The list of objects to traverse</param>
        /// <param name="target">The context object in which you want to find a set of related objects from</param>
        /// <param name="parentPredicate">The predicate function which determines if object a is infact the child of object b
        /// Format - parentPredicate(potentialChild, target) <example>(c, p) => c.ParentId == p.Id</example></param>
        /// <returns></returns>
        public static IEnumerable<T> Children<T>(this IEnumerable<T> source, T target, Func<T, T, bool> parentPredicate) where T : class
        {
            return source.Where(q => parentPredicate(q, target));
        }

        /// <summary>
        /// In the eventuality that more than one object matches the parent predicate, this will return all objects
        /// </summary>
        /// <typeparam name="T">The type of objects to enumerate.</typeparam>
        /// <param name="source">The list of objects to traverse</param>
        /// <param name="target">The context object in which you want to find a set of related objects from</param>
        /// <param name="parentPredicate">The predicate function which determines if object b is infact the parent of object a
        /// Format - parentPredicate(target, potentialParent) <example>(c, p) => c.ParentId == p.Id</example></param>
        /// <returns>A number of parents from the source collection which match the parent predicate.</returns>
        public static IEnumerable<T> Parents<T>(this IEnumerable<T> source, T target, Func<T, T, bool> parentPredicate) where T : class
        {
            return source.Where(q => parentPredicate(target, q));
        }

        /// <summary>
        /// Gets all ancestors in ascending order of hierarchical distance from the target node (the first entry will be the immediate parent)
        /// This function assumes that only a single parent may exist
        /// </summary>
        /// <typeparam name="T">The type of objects to enumerate.</typeparam>
        /// <param name="source">The list of objects to traverse</param>
        /// <param name="target">The context object in which you want to find a set of related objects from</param>
        /// <param name="parentPredicate">The predicate function which determines if object b is infact the parent of object a
        /// Format - parentPredicate(target, potentialParent) <example>(c, p) => c.ParentId == p.Id</example></param>
        /// <returns>A filtered subset of the source collection containing the parents the target object in ascending order.</returns>
        /// <exception cref="InvalidOperationException">Sequence contains more than one element</exception>
        public static IEnumerable<T> AncestorsAllAscending<T>(this IEnumerable<T> source, T target, Func<T, T, bool> parentPredicate) where T : class
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            List<T> results = new List<T>();

            var getParentsRecursive = Functional.Y<T, IEnumerable<T>>(f =>
            {
                return y =>
                {
                    //we are assuming that this item only has one parent. Should there be more, Linq will throw an exception.
                    T parent = ParentSingleOrDefault(source, y, parentPredicate);
                    if (parent != null)
                    {
                        if (!results.Contains(parent))
                        {
                            results.Add(parent);
                            f(parent);
                        }
                    }
                    return results;
                };
            });

            return getParentsRecursive(target);
        }

        /// <summary>
        /// Gets all ancestors in ascending order of hierarchical distance from the target node (the first entry will be the immediate parent) including itself as the first entry in the returned IEnumerable.
        /// This function assumes that only a single parent may exist
        /// </summary>
        /// <typeparam name="T">The type of objects to enumerate.</typeparam>
        /// <param name="source">The list of objects to traverse</param>
        /// <param name="target">The context object in which you want to find a set of related objects from</param>
        /// <param name="parentPredicate">The predicate function which determines if object b is infact the parent of object a
        /// Format - parentPredicate(target, potentialParent) <example>(c, p) => c.ParentId == p.Id</example></param>
        /// <returns>A filtered subset of the source collection containing the parents the target object in ascending order.</returns>
        /// <exception cref="InvalidOperationException">Sequence contains more than one element</exception>
        public static IEnumerable<T> AncestorsAllAscendingIncludingSelf<T>(this IEnumerable<T> source, T target, Func<T, T, bool> parentPredicate) where T : class
        {
            var ancestorsAllAscending = source.AncestorsAllAscending(target, parentPredicate).ToList();
            ancestorsAllAscending.Insert(0, target);
            return ancestorsAllAscending;
        }

        /// <summary>
        /// Get all of the descendants of the target object recursively that match the parent predicate. The collection is not sorted into any particular hierarchical order.
        /// </summary>
        /// <typeparam name="T">The type of the target object and source IEnumerable</typeparam>
        /// <param name="source">The source IEnumerable to search against</param>
        /// <param name="target">The target item or parent of the objects you wish to find.</param>
        /// <param name="parentPredicate">The predicate function which determines if object b is infact the parent of object a
        /// Format - parentPredicate(target, potentialParent) <example>(c, p) => c.ParentId == p.Id</example></param>
        /// <returns>A filtered subset of the source containing only children of the target object.</returns>
        public static IEnumerable<T> DescendantsAll<T>(this IEnumerable<T> source, T target, Func<T, T, bool> parentPredicate) where T : class
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            var getChildrenRecursive = Functional.Y<IEnumerable<T>, IEnumerable<T>>(f =>
            {
                return y =>
                {
                    List<T> ch = new List<T>();
                    foreach (var item in y)
                    {
                        var itemChildren = Children(source, item, parentPredicate);
                        ch.AddRange(itemChildren);

                        if (itemChildren.Count() > 0)
                            ch.AddRange(f(itemChildren));
                    }

                    return ch;
                };
            });

            return getChildrenRecursive(new List<T>() { target });
        }

        /// <summary>
        /// Gets all of the descendants recursively in order of ascending hierarchical depth from the target object or parent.
        /// </summary>
        /// <typeparam name="T">The type of the target object and source IEnumerable</typeparam>
        /// <param name="source">The source IEnumerable to search against</param>
        /// <param name="target">The target item or parent of the objects you wish to find.</param>
        /// <param name="parentPredicate">The predicate function which determines if object b is infact the parent of object a
        /// Format - parentPredicate(target, potentialParent) <example>(c, p) => c.ParentId == p.Id</example></param>
        /// <returns>A filtered subset of the source containing only children of the target object ordered into ascending hierarchical order of depth.</returns>
        public static IEnumerable<T> DescendantsAllAscending<T>(this IEnumerable<T> source, T target, Func<T, T, bool> parentPredicate) where T : class
        {
            return source.DescendantsAllAscendingWithDepth(target, parentPredicate).Select(s => s.Key);
        }

        /// <summary>
        /// Gets all of the descendants recursively in order of ascending hierarchical depth from the target object or parent including itself as the first entry in the returned IEnumerable.
        /// </summary>
        /// <typeparam name="T">The type of the target object and source IEnumerable</typeparam>
        /// <param name="source">The source IEnumerable to search against</param>
        /// <param name="target">The target item or parent of the objects you wish to find.</param>
        /// <param name="parentPredicate">The predicate function which determines if object b is infact the parent of object a
        /// Format - parentPredicate(target, potentialParent) <example>(c, p) => c.ParentId == p.Id</example></param>
        /// <returns>A filtered subset of the source containing the target object and the children of the target object ordered into ascending hierarchical order of depth.</returns>
        public static IEnumerable<T> DescendantsAllAscendingIncludingSelf<T>(this IEnumerable<T> source, T target, Func<T, T, bool> parentPredicate) where T : class
        {
            List<T> decendantsAllAscending = source.DescendantsAllAscending(target, parentPredicate).ToList();
            decendantsAllAscending.Insert(0, target);
            return decendantsAllAscending;
        }


        /// <summary>
        /// Gets all of the descendants recursively in order of ascending hierarchical depth from the target object or parent.
        /// This is a more verbose alternative to <see cref="DescendantsAllAscending"/>, which returns a list of key value pairs in the form { (T)object, (int)depth }
        /// </summary>
        /// <typeparam name="T">The type of the target object and source IEnumerable.</typeparam>
        /// <param name="source">The source IEnumerable to search against.</param>
        /// <param name="target">The target item or parent of the objects you wish to find.</param>
        /// <param name="parentPredicate">The predicate function which determines if object b is infact the parent of object a
        /// Format - parentPredicate(target, potentialParent) <example>(c, p) => c.ParentId == p.Id</example></param>
        /// <returns>A filtered subset of the source containing only children of the target object ordered into ascending hierarchical order of depth.
        /// This is a list of key value pairs in the form { (T)object, (int)depth }</returns>
        public static IEnumerable<KeyValuePair<T, int>> DescendantsAllAscendingWithDepth<T>(this IEnumerable<T> source, T target, Func<T, T, bool> parentPredicate) where T : class
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            var getChildrenRecursive = Functional.Y<IEnumerable<KeyValuePair<T, int>>, IEnumerable<KeyValuePair<T, int>>>(f =>
            {
                return y =>
                {
                    List<KeyValuePair<T, int>> ch = new List<KeyValuePair<T, int>>();
                    foreach (var item in y)
                    {
                        var itemChildren = Children(source, item.Key, parentPredicate).Select(s => new KeyValuePair<T, int>(s, item.Value + 1));
                        ch.AddRange(itemChildren);

                        if (itemChildren.Count() > 0)
                            ch.AddRange(f(itemChildren));
                    }

                    return ch;
                };
            });

            return getChildrenRecursive(new List<KeyValuePair<T, int>>() { new KeyValuePair<T, int>(target, 0) }).OrderBy(o => o.Value);
        }

        /// <summary>
        /// Gets all of the descendants recursively in order of ascending hierarchical depth from the target object or parent and includes itself.
        /// This is a more verbose alternative to <see cref="DescendantsAllAscending"/>, which returns a list of key value pairs in the form { (T)object, (int)depth }
        /// </summary>
        /// <typeparam name="T">The type of the target object and source IEnumerable.</typeparam>
        /// <param name="source">The source IEnumerable to search against.</param>
        /// <param name="target">The target item or parent of the objects you wish to find.</param>
        /// <param name="parentPredicate">The predicate function which determines if object b is infact the parent of object a
        /// Format - parentPredicate(target, potentialParent) <example>(c, p) => c.ParentId == p.Id</example></param>
        /// <returns>A filtered subset of the source containing only children of the target object ordered into ascending hierarchical order of depth.
        /// This is a list of key value pairs in the form { (T)object, (int)depth }</returns>
        public static IEnumerable<KeyValuePair<T, int>> DescendantsAllAscendingWithDepthIncludingSelf<T>(this IEnumerable<T> source, T target, Func<T, T, bool> parentPredicate) where T : class
        {
            var decendantsAllAscendingWithDepth = source.DescendantsAllAscendingWithDepth(target, parentPredicate).ToList();
            decendantsAllAscendingWithDepth.Insert(0, new KeyValuePair<T, int>(target, 0));
            return decendantsAllAscendingWithDepth;
        }
    }
}
