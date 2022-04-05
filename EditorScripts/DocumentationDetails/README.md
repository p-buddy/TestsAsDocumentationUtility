# Files
[DocumentationSnippet](./DocumentationSnippet.cs)

- A [DocumentationGroup](./DocumentationGroup.cs) is a collection of one or more [DocumentationSnippets](./DocumentationSnippet.cs). 
  - In the case of a [DocumentationGroup](./DocumentationGroup.cs) with only a single [DocumentationSnippet](./DocumentationSnippet.cs), 
  it is likely that that the documenting member was marked with [DemonstratesAttribute](../Attributes/DemonstratesAttribute.cs) that indicated it's [Grouping](./GroupInfo.cs) as `None`
- A [DocumentationCollection](./DocumentationCollection.cs) is a collection of one or more [DocumentationGroups](./DocumentationGroup.cs), which represents **all** of the pieces documenting a given type / member
