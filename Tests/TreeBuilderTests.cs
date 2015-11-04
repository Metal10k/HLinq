using System;
using System.Linq;
using NUnit.Framework;

namespace Tree
{
	class TreeBuilderTests
	{
		[Test]
		public void ShouldBuildTree()
		{
			var b = new TreeBuilder();
			var apiPermissions = new []
			{
				new ApiPermission(){Group = "A"},
				new ApiPermission(){Group = "B"},
				new ApiPermission(){Group = "C"},
				new ApiPermission(){Group = "C.D"},
				new ApiPermission(){Group = "C.D.E"},
				new ApiPermission(){Group = "F.G"}
			};
			var permissions = b.Build(apiPermissions.Select(p => p.Group));
			Assert.That(permissions, Is.Not.Null);

			var expectedPermissions = new[]
			{
				new SimpleTreeNode()
				{
					Name= "A", Children = new SimpleTreeNode[0]
				},
				new SimpleTreeNode()
				{
					Name= "B", Children = new SimpleTreeNode[0]
				},
				new SimpleTreeNode()
				{
					Name = "C",
					Children = new[]
					{
						new SimpleTreeNode()
						{
							Name = "D",
							Children = new []
							{
								new SimpleTreeNode()
								{
									Name="E", Children = new SimpleTreeNode[0]
								}
							}
						}
					}
				},
				new SimpleTreeNode()
				{
					Name = "F",
					Children = new []
					{
						new SimpleTreeNode()
						{
							Name= "G", Children = new SimpleTreeNode[0]
						}
					}
				}
			};

			Assert.That(permissions, new MemberwiseEqualConstraint(expectedPermissions, MemberwiseEqualityComparer.ComparisonMode.CompareEverything, null));
		}
	}
}
