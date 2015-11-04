using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HLinq.Tree.Fluent
{
	public class SimpleTree : IEnumerable<SimpleTreeNode>
	{
		private readonly IEnumerable<SimpleTreeNode> _nodes;

		public SimpleTree(IEnumerable<SimpleTreeNode> nodes)
		{
			_nodes = nodes;
		}

		public IEnumerator<SimpleTreeNode> GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}


		/// <summary>
		/// Creates a join of the nodes in question and their target paths
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="nodes"></param>
		/// <param name="objectsWithPaths"></param>
		/// <returns></returns>
		public ProjectedTree<T> Join<T>(IEnumerable<Tuple<string, T>> objectsWithPaths)
		{
			var projectedTreeNodes = objectsWithPaths.SelectMany(op =>
					_nodes.Select(n => _MapRec(n, op.Item1.Split('.'), op.Item2))
				).Where(q => q != null).ToList();
			var treeNodes = _AggregateRecursively(
					projectedTreeNodes
				).ToList();
			return new ProjectedTree<T>(treeNodes);
		}

		private static ProjectedTreeNode<T> _MapRec<T>(SimpleTreeNode node, string[] pathComponents, T item)
		{
			var first = pathComponents.First();

			if (node.Name == first)
			{
				if (pathComponents.Length == 1)	//if we are at the end of the path
					return new ProjectedTreeNode<T>()
					{
						Name = node.Name,
						Item = item,
						Children = new ProjectedTreeNode<T>[0]
					};

				var mappedChildren = node.Children
					.Select(c => _MapRec(c, pathComponents.Skip(1).ToArray(), item))
					.Where(q => q != null);
				return new ProjectedTreeNode<T>()
				{
					Children = mappedChildren,
					Item = default(T),
					Name = node.Name
				};
			}
			return null;
		}

		private static IEnumerable<ProjectedTreeNode<T>> _AggregateRecursively<T>(IEnumerable<ProjectedTreeNode<T>> permissions)
		{
			return permissions
				.GroupBy(g => g.Name)
				.Select(s =>
				{
					var unaggregatedProjectedNode = s.FirstOrDefault(q => q != null && q.Item != null);
					return new ProjectedTreeNode<T>()
					{
						Name = s.Key,
						Item = unaggregatedProjectedNode != null ? unaggregatedProjectedNode.Item : default(T),
						Children = _AggregateRecursively(
							s.SelectMany(s2 => s2.Children))
							.ToArray()
					};
				});
		}
	}
}