using System.Collections.Generic;
using System.Linq;
using HLinq.Tree.Fluent;

namespace HLinq.Tree
{
	public class TreeBuilder
	{
		public SimpleTree Build(IEnumerable<string> delimitedPaths)
		{
			var splitPermissionGroups = delimitedPaths.Select(p => p.Split('.'));
			var groups = splitPermissionGroups
				.Where(q => q.Length > 0)	//guard is probably not necessary
				.Select(q =>
				{
					return q.Reverse()
						.Aggregate(new SimpleTreeNode() { }, (child, group) =>
							new SimpleTreeNode()
							{
								Name = group,
								Children = child.Name != null ? new[] { child } : new SimpleTreeNode[0]
							});
				});

			var simpleTreeNodes = _AggregateRecursively(groups)
				.ToArray();
			return new SimpleTree(simpleTreeNodes);
		}

		private static IEnumerable<SimpleTreeNode> _AggregateRecursively (IEnumerable<SimpleTreeNode> permissions)
		{
			return permissions
				.GroupBy(g => g.Name)
				.Select(s => new SimpleTreeNode()
				{
					Name = s.Key,
					Children = _AggregateRecursively(
						s.SelectMany(s2 => s2.Children))
						.ToArray()
				});
		}

	}

	public class SimpleTreeNode
	{
		public string Name { get; set; }
		public IEnumerable<SimpleTreeNode> Children { get; set; }
	}

	public class ProjectedTreeNode<T>
	{
		public string Name { get; set; }
		public T Item { get; set; }
		public IEnumerable<ProjectedTreeNode<T>> Children { get; set; }
	}

	public class PairedTreeNode<T, U>
	{
		public string Name { get; set; }
		public IEnumerable<PairedTreeNode<T, U>> Children { get; set; } 
		public T Left{get;set;}
		public U Right { get; set; }
	}
}