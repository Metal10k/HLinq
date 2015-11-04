using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HLinq.Tree.Fluent
{
	public class ProjectedTree<T>: IEnumerable<ProjectedTreeNode<T>>
	{
		private readonly IEnumerable<ProjectedTreeNode<T>> _nodes;

		public ProjectedTree(IEnumerable<ProjectedTreeNode<T>> nodes)
		{
			_nodes = nodes;
		}

		public IEnumerator<ProjectedTreeNode<T>> GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _nodes.GetEnumerator();
		}

		public PairedTree<T,U> InnerJoin<U>(IEnumerable<ProjectedTreeNode<U>> secondTree)
		{
			return new PairedTree<T, U>(_InnerJoin(_nodes, secondTree));
		}

		public PairedTree<T, U> LeftJoin<U>(IEnumerable<ProjectedTreeNode<U>> secondTree)
		{
			return new PairedTree<T, U>(_LeftJoin(_nodes, secondTree));
		}

		/// <summary>
		/// Performs a recursive inner join on two typed tree's
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="U"></typeparam>
		/// <param name="firstTree"></param>
		/// <param name="secondTree"></param>
		/// <returns></returns>
		private static IEnumerable<PairedTreeNode<T, U>> _InnerJoin<U>(IEnumerable<ProjectedTreeNode<T>> firstTree, IEnumerable<ProjectedTreeNode<U>> secondTree)
		{
			var z = from q1 in firstTree
					join q2 in secondTree on q1.Name equals q2.Name
					select new PairedTreeNode<T, U>()
					{
						Name = q1.Name,
						Children = _InnerJoin(
							q1.Children,
							q2.Children),
						Left = q1.Item,
						Right = q2.Item
					};

			return z;
		}

		private static IEnumerable<PairedTreeNode<T, U>> _LeftJoin<U>(IEnumerable<ProjectedTreeNode<T>> firstTree, IEnumerable<ProjectedTreeNode<U>> secondTree)
		{
			var z = from q1 in firstTree
					join qTemp in secondTree on q1.Name equals qTemp.Name into rightTemp
					from q2 in rightTemp.DefaultIfEmpty()	//this is a left join! //https://msdn.microsoft.com/en-us/library/bb397895.aspx
					select new PairedTreeNode<T, U>()
					{
						Name = q1.Name,
						Children = _LeftJoin(
							q1.Children,
							q2 != null ? q2.Children : new ProjectedTreeNode<U>[0]),
						Left = q1.Item,
						Right = q2 != null ? q2.Item : default(U)
					};

			return z;
		}
	}
}