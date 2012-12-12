Update

Key expiration and listing of columns is now supported.


HLinq is a simple set of LINQ extension methods and tools to allow traversing a hierarchy of related objects via some expression.
An object is normally defined as a start point along with an expression defining the parent/child relationship of the objects. 

	 Expressions like the following are then possible
	 
	 IEnumerable<MyHierarchicalType> list = flatList.DescendantsAll(targetItem, (c, p) => c.ParentId == p.ID && p.ID != c.ID);

	 IEnumerable<MyHierarchicalType> list2 = flatList.DescendantsAllAscendingIncludingSelf(targetItem, (c, p) => c.ParentId == p.ID && p.ID != c.ID);
	 
	 IEnumerable<MyHierarchicalType> list3 = flatList.AncestorsAllAscending(targetItem, (c, p) => c.ParentId == p.ID && p.ID != c.ID);
 
 We can also get the direct children of an item among other things
 
	var directChildren = flatList.Children(targetItem, (c, p) => c.ParentId == p.ID && p.ID != c.ID);