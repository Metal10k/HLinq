using System.Linq;
using NUnit.Framework;

namespace Tree.Fluent
{
	class PairedTreeTests
	{
		[Test]
		public void ShouldFlattenTree()
		{
			var tree = new PairedTreeNode<int, double>()
			{
				Name = "D",
				Left = 4,
				Right = 4,
				Children = new[]
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
			};

			var result = new PairedTree<int, double>(new[]{tree}).Flatten();
			
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Count(), Is.EqualTo(3));
		}
	}
}