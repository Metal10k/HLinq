using NUnit.Framework;

namespace Tree.Fluent
{
	class ProjectedTreeTests
	{
		[Test]
		public void ShouldJoinDisparateTrees()
		{
			var leftTree = new[]
			{
				new ProjectedTreeNode<int>()
				{
					Name="A",
					Item=1,
					Children = new ProjectedTreeNode<int>[0]
				}, 
				new ProjectedTreeNode<int>()
				{
					Name="B",
					Item=2,
					Children = new[]
					{
						new ProjectedTreeNode<int>()
						{
							Name="C",
							Item=3,
							Children = new ProjectedTreeNode<int>[0]
						},
					}
				},
				new ProjectedTreeNode<int>()
				{
					Name="D",
					Item=4,
					Children = new []
					{
						new ProjectedTreeNode<int>()
						{
							Name="E",
							Item=5,
							Children = new[]
							{
								new ProjectedTreeNode<int>()
								{
									Name="F",
									Item=6,
									Children = new ProjectedTreeNode<int>[0]
								},
							}
						},
						new ProjectedTreeNode<int>()
						{
							Name="N",
							Item=106,
							Children = new ProjectedTreeNode<int>[0]
						},
					}
				},
				new ProjectedTreeNode<int>()//should not match
				{
					Name="G",	
					Item=100,
					Children = new ProjectedTreeNode<int>[0]
				}, 
			};

			var rightTree = new[]
			{
				new ProjectedTreeNode<double>()
				{
					Name="A",
					Item=1,
					Children = new ProjectedTreeNode<double>[0]
				}, 
				new ProjectedTreeNode<double>()
				{
					Name="B",
					Item=2,
					Children = new[]
					{
						new ProjectedTreeNode<double>()
						{
							Name="C",
							Item=3,
							Children = new ProjectedTreeNode<double>[0]
						},
					}
				},
				new ProjectedTreeNode<double>()
				{
					Name="D",
					Item=4,
					Children = new []
					{
						new ProjectedTreeNode<double>()
						{
							Name="E",
							Item=5,
							Children = new[]
							{
								new ProjectedTreeNode<double>()
								{
									Name="F",
									Item=6,
									Children = new ProjectedTreeNode<double>[0]
								},
								new ProjectedTreeNode<double>()//This should not match as it exists in a different part of the tree
								{
									Name="N",	
									Item=104,
									Children = new ProjectedTreeNode<double>[0]
								},
							}
						}
					}
				},
				new ProjectedTreeNode<double>()//should not match
				{
					Name="H",	
					Item=100D,
					Children = new ProjectedTreeNode<double>[0]
				}, 
			};

			var result = new ProjectedTree<int>(leftTree)
				.InnerJoin(rightTree);

			Assert.That(result, Is.Not.Null);

			var expectedTree = new[]
			{
				new PairedTreeNode<int, double>()
				{
					Name="A",
					Left = 1,
					Right = 1,
					Children = new PairedTreeNode<int, double>[0],
				},
				new PairedTreeNode<int, double>()
				{
					Name="B",
					Left = 2,
					Right = 2,
					Children = new []
					{
						new PairedTreeNode<int, double>()
						{
							Name="C",
							Left = 3,
							Right = 3,
							Children = new PairedTreeNode<int, double>[0],
						}
					},
				},
				new PairedTreeNode<int, double>()
				{
					Name="D",
					Left = 4,
					Right = 4,
					Children = new []
					{
						new PairedTreeNode<int, double>()
						{
							Name="E",
							Left = 5,
							Right = 5,
							Children = new []
							{
								new PairedTreeNode<int, double>()
								{
									Name="F",
									Left = 6,
									Right = 6,
									Children = new PairedTreeNode<int, double>[0],
								}
							}
						}
					},
				},
			};

			Assert.That(result, new MemberwiseEqualConstraint(expectedTree, MemberwiseEqualityComparer.ComparisonMode.CompareEverything, null));
		}

		[Test]
		public void ShouldLeftJoinTrees()
		{
			var leftTree = new[]
			{
				new ProjectedTreeNode<int>()
				{
					Name="A",
					Item=1,
					Children = new ProjectedTreeNode<int>[0]
				},
				new ProjectedTreeNode<int>()
				{
					Name="B",
					Item=2,
					Children = new ProjectedTreeNode<int>[0]
				},
			};

			var rightTree = new[]
			{
				new ProjectedTreeNode<double>()
				{
					Name="A",
					Item=1,
					Children = new ProjectedTreeNode<double>[0]
				}
			};

			var result = new ProjectedTree<int>( leftTree).LeftJoin(rightTree);

			var expectedTree = new[]
			{
				new PairedTreeNode<int, double>()
				{
					Name = "A",
					Left = 1,
					Right = 1,
					Children = new PairedTreeNode<int, double>[]
					{
					},
				},
				new PairedTreeNode<int, double>()
				{
					Name = "B",
					Left = 2,
					Right = 0,
					Children = new PairedTreeNode<int, double>[]
					{
					}
				}
			};

			Assert.That(result, new MemberwiseEqualConstraint(expectedTree, MemberwiseEqualityComparer.ComparisonMode.CompareEverything, null));
		}
	}
}