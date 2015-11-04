using System;
using System.Linq;
using NUnit.Framework;

namespace Tree.Fluent
{
	class SimpleTreeTests
	{
		[Test]
		public void ShouldJoinObjectsWithPaths()
		{
			var nodes = new []
			{
				new SimpleTreeNode()
				{
					Name="A"
				},
				new SimpleTreeNode()
				{
					Name="B",
					Children = new []
					{
						new SimpleTreeNode()
						{
							Name="C"
						}
					}
				},
				new SimpleTreeNode()
				{
					Name="D",
					Children=new[]
					{
						new SimpleTreeNode()
						{
							Name="E",
							Children = new []
							{
								new SimpleTreeNode()
								{
									Name = "F"
								},
								new SimpleTreeNode()
								{
									Name = "G"
								}
							}
						}
					}
				},
				new SimpleTreeNode()
				{
					Name="G",
					Children = new SimpleTreeNode[0]
				}
			};

			var paths = new []
			{
				new Tuple<string, int?>("A", 1),
				new Tuple<string, int?>("B.C", 2),
				new Tuple<string, int?>("D.E.F", 3),
				new Tuple<string, int?>("D.E.G", 4),
				new Tuple<string, int?>("D", 5)
			};

			var result = new SimpleTree(nodes).Join(paths).ToList();

			Assert.That(result, Is.Not.Null);

			var expected = new[]
			{
				new ProjectedTreeNode<int?>()
				{
					Name="A",
					Item=1,
					Children = new ProjectedTreeNode<int?>[0]
				}, 
				new ProjectedTreeNode<int?>()
				{
					Name="B",
					Item=null,
					Children = new[]
					{
						new ProjectedTreeNode<int?>()
						{
							Name="C",
							Item=2,
							Children = new ProjectedTreeNode<int?>[0]
						},
					}
				},
				new ProjectedTreeNode<int?>()
				{
					Name="D",
					Item=5,
					Children = new []
					{
						new ProjectedTreeNode<int?>()
						{
							Name="E",
							Item=null,
							Children = new[]
							{
								new ProjectedTreeNode<int?>()
								{
									Name="F",
									Item=3,
									Children = new ProjectedTreeNode<int?>[0]
								},
								new ProjectedTreeNode<int?>()
								{
									Name="G",
									Item=4,
									Children = new ProjectedTreeNode<int?>[0]
								},
							}
						},
					}
				}
			};

			Assert.That(result, new MemberwiseEqualConstraint(expected, MemberwiseEqualityComparer.ComparisonMode.CompareEverything, null));
		}
	}
}