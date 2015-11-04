using System.Collections;
using System.Collections.Generic;

namespace HLinq.Tree.Fluent
{
	public class PairedTree<T, U> : IEnumerable<PairedTreeNode<T, U>>
	{
		private readonly IEnumerable<PairedTreeNode<T, U>> _nodes;

		public PairedTree(IEnumerable<PairedTreeNode<T, U>> nodes)
		{
			_nodes = nodes;
		}

		public IEnumerator<PairedTreeNode<T, U>> GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		public PairedTree<T, U> Flatten()
		{
			var pairedTreeNodes = _Flatten(_nodes);
			return new PairedTree<T, U>(pairedTreeNodes);
		}

		private static IEnumerable<PairedTreeNode<T, U>> _Flatten(IEnumerable<PairedTreeNode<T, U>> tree)
		{
			foreach (var node in tree)
			{
				yield return node;
				foreach (var child in _Flatten(node.Children))
				{
					yield return child;
				}
			}
		}
	}
}